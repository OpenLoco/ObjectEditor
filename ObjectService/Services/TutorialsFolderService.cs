using Definitions;
using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// The service for <c>GameData/Tutorials</c>. Tutorials are their own entity type and are stored in
/// the <c>Tutorials</c> table.
/// </summary>
public sealed class TutorialsFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<TutorialsFolderService> logger)
	: GameDataFolderServiceBase(db, sfm, logger)
{
	public override async Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		var name = Path.GetRelativePath(Sfm.TutorialsFolder, absolutePath);
		var modified = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absolutePath));
		var source = GetObjectSource(absolutePath, Sfm.TutorialsFolder);

		var existing = await Db.Tutorials.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			// The file is present (we are importing it); only Custom content can be available.
			var availability = ObjectAvailabilityRules.ForFile(existing.ObjectSource, fileExists: true);
			var dateChanged = existing.ModifiedDate != modified;
			var availabilityChanged = existing.Availability != availability;

			if (!dateChanged && !availabilityChanged)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Tutorial {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			existing.Availability = availability;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated tutorial {name}");
		}

		_ = await Db.Tutorials.AddAsync(new TblTutorial
		{
			Name = name,
			ObjectSource = source,
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
			Availability = ObjectAvailabilityRules.ForFile(source, fileExists: true),
		}, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Added tutorial {Name} to the database", name);

		return new GameDataImportResult(GameDataImportStatus.Added, $"Tutorial {name} added");
	}

	public override async Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
	{
		var name = Path.GetRelativePath(Sfm.TutorialsFolder, absolutePath);
		var existing = await Db.Tutorials.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Tutorial was not present in the database");
		}

		// The file is gone, but the database is the source of truth: keep the row and mark it
		// unavailable so metadata and references survive.
		existing.Availability = ObjectAvailability.Unavailable;
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Marked tutorial {Name} unavailable (its file was removed)", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Tutorial {name} marked unavailable");
	}

	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var changed = 0;

		foreach (var file in EnumerateFiles(Sfm.TutorialsFolder, _ => true))
		{
			var result = await TryReconcileFileAsync(file, () => ImportAsync(file, ct), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed++;
			}
		}

		var availabilityChanged = await SyncAvailabilityWithDiskAsync(Db.Tutorials, Sfm.TutorialsFolder, Sfm.TutorialsFolder, _ => true, ct).ConfigureAwait(false);

		Logger.LogInformation("Tutorials reconciliation complete: {Count} file(s) added/updated, {Changed} row(s) had their availability updated", changed, availabilityChanged);
	}
}
