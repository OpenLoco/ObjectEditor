using Dat.FileParsing;
using Dat.Types.SCV5;
using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// The services for <c>GameData/Scenarios</c> and <c>GameData/Landscapes</c>. Scenarios and
/// landscapes are the same entity type (S5 scenario files) and share the <c>Scenarios</c> table, so
/// they share this implementation but are still distinct services, one per folder.
/// </summary>
public abstract class ScenarioFolderServiceBase : GameDataFolderServiceBase
{
	private readonly string _scanFolder;
	private readonly string _nameRoot;

	protected ScenarioFolderServiceBase(
		LocoDbContext db,
		ServerFolderManager sfm,
		ILogger logger,
		string scanFolder,
		string nameRoot)
		: base(db, sfm, logger)
	{
		_scanFolder = scanFolder;
		_nameRoot = nameRoot;
	}

	public override async Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		if (!IsSc5File(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Not an SC5 file");
		}

		byte[] bytes;
		try
		{
			bytes = await File.ReadAllBytesAsync(absolutePath, ct).ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			return new GameDataImportResult(GameDataImportStatus.Failed, $"Could not read file: {ex.Message}");
		}

		if (!TryReadHeader(bytes, out var header))
		{
			return new GameDataImportResult(GameDataImportStatus.Failed, "Invalid S5 file header");
		}

		// Savegames are not part of the repository (see TblScenario), so ignore them.
		if (header!.Type is S5FileType.SavedGame)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Savegames are not added to the database");
		}

		var name = Path.GetRelativePath(_nameRoot, absolutePath);
		var modified = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absolutePath));
		var source = GetObjectSource(absolutePath);

		var existing = await Db.Scenarios.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			// The file is present (we are importing it); only Custom content can be available.
			var availability = ObjectAvailabilityRules.ForFile(existing.ObjectSource, fileExists: true);
			var dateChanged = existing.ModifiedDate != modified;
			var availabilityChanged = existing.Availability != availability;

			if (!dateChanged && !availabilityChanged)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Scenario {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			existing.Availability = availability;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated scenario {name}");
		}

		var tbl = new TblScenario
		{
			Name = name,
			Description = null,
			ObjectSource = source,
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
			UploadedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
			Availability = ObjectAvailabilityRules.ForFile(source, fileExists: true),
			Authors = [],
			Tags = [],
			ScenarioPacks = [],
		};

		_ = await Db.Scenarios.AddAsync(tbl, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Added {Type} file {Name} to the database", header.Type, name);

		return new GameDataImportResult(GameDataImportStatus.Added, $"Scenario {name} added");
	}

	public override async Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
	{
		var name = Path.GetRelativePath(_nameRoot, absolutePath);
		var existing = await Db.Scenarios.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Scenario was not present in the database");
		}

		// The file is gone, but the database is the source of truth: keep the row and mark it
		// unavailable so metadata and pack references survive.
		existing.Availability = ObjectAvailability.Unavailable;
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Marked scenario {Name} unavailable (its file was removed)", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Scenario {name} marked unavailable");
	}

	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var changed = 0;

		foreach (var file in EnumerateFiles(_scanFolder, IsSc5File))
		{
			var result = await TryReconcileFileAsync(file, () => ImportAsync(file, ct), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed++;
			}
		}

		// Scenarios and landscapes share the Scenarios table but store names relative to different
		// roots, so only rows that resolve into this service's scan folder and that this service owns
		// are considered. A row is never deleted: its availability simply reflects whether its file is
		// present.
		var availabilityChanged = await SyncAvailabilityWithDiskAsync(Db.Scenarios, _nameRoot, _scanFolder, OwnsScenarioRow, ct).ConfigureAwait(false);

		Logger.LogInformation("Scenario reconciliation complete: {Count} file(s) added/updated, {Changed} row(s) had their availability updated", changed, availabilityChanged);
	}

	static bool TryReadHeader(byte[] bytes, out S5FileHeader? header)
	{
		header = null;
		if (bytes.Length < S5FileHeader.StructLength)
		{
			return false;
		}

		try
		{
			var span = (ReadOnlySpan<byte>)bytes;
			header = SawyerStreamReader.ReadChunk<S5FileHeader>(ref span);
			return header != null;
		}
		catch (Exception)
		{
			// Corrupt or unsupported S5 header - treat as invalid.
			return false;
		}
	}

	ObjectSource GetObjectSource(string absolutePath)
		=> GameDataFolderServiceBase.GetObjectSource(absolutePath, _scanFolder);

	/// <summary>
	/// Whether a <c>Scenarios</c>-table row named <paramref name="name"/> belongs to this service.
	/// Scenarios and landscapes share the table; scenario names are relative to the Scenarios folder,
	/// but a landscape is named <c>Landscapes/...</c> relative to GameData. The default accepts every
	/// name (landscapes are already filtered by their scan-folder path check); the scenario service
	/// overrides this to skip the landscape prefix so it never deletes a landscape row.
	/// </summary>
	protected virtual bool OwnsScenarioRow(string name) => true;
}

/// <summary>The service for <c>GameData/Scenarios</c>.</summary>
public sealed class ScenariosFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<ScenariosFolderService> logger)
	: ScenarioFolderServiceBase(db, sfm, logger, sfm.ScenariosFolder, sfm.ScenariosFolder)
{
	protected override bool OwnsScenarioRow(string name)
		=> !name.Replace('\\', '/').StartsWith(ServerFolderManager.LandscapesFolderName + "/", StringComparison.OrdinalIgnoreCase);
}

/// <summary>
/// The service for <c>GameData/Landscapes</c>. Names are stored relative to GameData (rather than
/// the Landscapes folder) so they can never collide with a scenario of the same relative name.
/// </summary>
public sealed class LandscapesFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<LandscapesFolderService> logger)
	: ScenarioFolderServiceBase(db, sfm, logger, sfm.LandscapesFolder, sfm.GameDataFolder);
