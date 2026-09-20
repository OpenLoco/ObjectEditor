using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// The service for <c>GameData/SoundEffects</c>. Sound effects are their own entity type and are
/// stored in the <c>SoundEffects</c> table.
/// </summary>
public sealed class SoundEffectsFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<SoundEffectsFolderService> logger)
	: GameDataFolderServiceBase(db, sfm, logger)
{
	public override async Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		var name = Path.GetRelativePath(Sfm.SoundEffectsFolder, absolutePath);
		var modified = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absolutePath));

		var existing = await Db.SoundEffects.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			if (existing.ModifiedDate == modified)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Sound effect {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated sound effect {name}");
		}

		_ = await Db.SoundEffects.AddAsync(new TblSoundEffect
		{
			Name = name,
			ObjectSource = GetObjectSource(absolutePath, Sfm.SoundEffectsFolder),
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
		}, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Added sound effect {Name} to the database", name);

		return new GameDataImportResult(GameDataImportStatus.Added, $"Sound effect {name} added");
	}

	public override async Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
	{
		var name = Path.GetRelativePath(Sfm.SoundEffectsFolder, absolutePath);
		var existing = await Db.SoundEffects.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Sound effect was not present in the database");
		}

		_ = Db.SoundEffects.Remove(existing);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Removed sound effect {Name} from the database", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Sound effect {name} removed");
	}

	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var changed = 0;

		foreach (var file in EnumerateFiles(Sfm.SoundEffectsFolder, _ => true))
		{
			var result = await ImportAsync(file, ct).ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed++;
			}
		}

		Logger.LogInformation("SoundEffects reconciliation complete: {Count} file(s) added/updated", changed);
	}
}
