using Dat.FileParsing;
using Dat.Types.SCV5;
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

		var existing = await Db.Scenarios.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			if (existing.ModifiedDate == modified)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Scenario {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated scenario {name}");
		}

		var tbl = new TblScenario
		{
			Name = name,
			Description = null,
			ObjectSource = GetObjectSource(absolutePath),
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
			UploadedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
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

		_ = Db.Scenarios.Remove(existing);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Removed scenario {Name} from the database after its file was deleted", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Scenario {name} removed");
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

		Logger.LogInformation("Scenario reconciliation complete: {Count} file(s) added/updated", changed);
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
	{
		if (IsUnder(absolutePath, Path.Combine(_scanFolder, ServerFolderManager.OpenLocoFolderName)))
		{
			return ObjectSource.OpenLoco;
		}

		if (IsUnder(absolutePath, Path.Combine(_scanFolder, ServerFolderManager.OriginalFolderName)))
		{
			return ObjectSource.LocomotionSteam;
		}

		return ObjectSource.Custom;
	}
}

/// <summary>The service for <c>GameData/Scenarios</c>.</summary>
public sealed class ScenariosFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<ScenariosFolderService> logger)
	: ScenarioFolderServiceBase(db, sfm, logger, sfm.ScenariosFolder, sfm.ScenariosFolder);

/// <summary>
/// The service for <c>GameData/Landscapes</c>. Names are stored relative to GameData (rather than
/// the Landscapes folder) so they can never collide with a scenario of the same relative name.
/// </summary>
public sealed class LandscapesFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<LandscapesFolderService> logger)
	: ScenarioFolderServiceBase(db, sfm, logger, sfm.LandscapesFolder, sfm.GameDataFolder);
