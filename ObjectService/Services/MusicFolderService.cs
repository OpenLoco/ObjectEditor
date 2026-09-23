using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// The service for <c>GameData/Music</c>. Music is its own entity type and is stored in the
/// <c>Music</c> table.
/// </summary>
public sealed class MusicFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<MusicFolderService> logger)
	: GameDataFolderServiceBase(db, sfm, logger)
{
	public override async Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		var name = Path.GetRelativePath(Sfm.MusicFolder, absolutePath);
		var modified = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absolutePath));

		var existing = await Db.Music.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			if (existing.ModifiedDate == modified)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Music {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated music {name}");
		}

		_ = await Db.Music.AddAsync(new TblMusic
		{
			Name = name,
			ObjectSource = GetObjectSource(absolutePath, Sfm.MusicFolder),
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
		}, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Added music {Name} to the database", name);

		return new GameDataImportResult(GameDataImportStatus.Added, $"Music {name} added");
	}

	public override async Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
	{
		var name = Path.GetRelativePath(Sfm.MusicFolder, absolutePath);
		var existing = await Db.Music.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Music was not present in the database");
		}

		_ = Db.Music.Remove(existing);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Removed music {Name} from the database", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Music {name} removed");
	}

	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var changed = 0;

		foreach (var file in EnumerateFiles(Sfm.MusicFolder, _ => true))
		{
			var result = await TryReconcileFileAsync(file, () => ImportAsync(file, ct), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed++;
			}
		}

		Logger.LogInformation("Music reconciliation complete: {Count} file(s) added/updated", changed);
	}
}
