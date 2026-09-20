using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// The service for <c>GameData/Graphics</c>. Graphics files are their own entity type and are stored
/// in the <c>Graphics</c> table.
/// </summary>
public sealed class GraphicsFolderService(LocoDbContext db, ServerFolderManager sfm, ILogger<GraphicsFolderService> logger)
	: GameDataFolderServiceBase(db, sfm, logger)
{
	public override async Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		var name = Path.GetRelativePath(Sfm.GraphicsFolder, absolutePath);
		var modified = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absolutePath));

		var existing = await Db.Graphics.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing != null)
		{
			if (existing.ModifiedDate == modified)
			{
				return new GameDataImportResult(GameDataImportStatus.Skipped, $"Graphics file {name} is already up to date");
			}

			existing.ModifiedDate = modified;
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return new GameDataImportResult(GameDataImportStatus.Updated, $"Updated graphics file {name}");
		}

		_ = await Db.Graphics.AddAsync(new TblGraphics
		{
			Name = name,
			ObjectSource = GetObjectSource(absolutePath, Sfm.GraphicsFolder),
			CreatedDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(absolutePath)),
			ModifiedDate = modified,
		}, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Added graphics file {Name} to the database", name);

		return new GameDataImportResult(GameDataImportStatus.Added, $"Graphics file {name} added");
	}

	public override async Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
	{
		var name = Path.GetRelativePath(Sfm.GraphicsFolder, absolutePath);
		var existing = await Db.Graphics.FirstOrDefaultAsync(x => x.Name == name, ct).ConfigureAwait(false);
		if (existing == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Graphics file was not present in the database");
		}

		_ = Db.Graphics.Remove(existing);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Removed graphics file {Name} from the database", name);

		return new GameDataImportResult(GameDataImportStatus.Removed, $"Graphics file {name} removed");
	}

	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var changed = 0;

		foreach (var file in EnumerateFiles(Sfm.GraphicsFolder, _ => true))
		{
			var result = await ImportAsync(file, ct).ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed++;
			}
		}

		Logger.LogInformation("Graphics reconciliation complete: {Count} file(s) added/updated", changed);
	}
}
