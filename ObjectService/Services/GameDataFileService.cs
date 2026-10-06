using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Index;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

/// <summary>The outcome of processing a single game-data file detected on disk.</summary>
public enum GameDataImportStatus
{
	Added,
	Updated,

	/// <summary>The file is gone; its database row was kept and marked
	/// <see cref="ObjectAvailability.Unavailable"/> (never deleted).</summary>
	Removed,

	/// <summary>The file's content (<c>xxHash3</c>) is already present: it is a duplicate of an existing
	/// file, so the oldest one is kept and this one is ignored.</summary>
	Duplicate,
	Unavailable,
	Skipped,
	Failed,
}

public record GameDataImportResult(GameDataImportStatus Status, string Message, ObjectIndexEntry? Entry = null);

/// <summary>
/// <para>
/// Contract implemented by each of the seven per-folder services. Each service owns the import and
/// removal logic for exactly one GameData entity type - game objects, scenarios, landscapes,
/// tutorials, sound effects, music and graphics are all distinct entities, so they each have their
/// own service rather than sharing one.
/// </para>
/// </summary>
public interface IGameDataFileService
{
	/// <summary>Checks the file and adds/updates it in whatever store this entity type owns.</summary>
	Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct);

	/// <summary>Called when the file is deleted or moved away.</summary>
	Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct);

	/// <summary>Reassesses the whole folder against whatever store this entity type owns.</summary>
	Task ReconcileAsync(CancellationToken ct);
}

/// <summary>
/// Shared plumbing for the per-folder services: the database context, the folder manager, the
/// logger and small file helpers. Entity-specific behaviour lives only in the concrete service.
/// </summary>
public abstract class GameDataFolderServiceBase : IGameDataFileService
{
	protected GameDataFolderServiceBase(LocoDbContext db, ServerFolderManager sfm, ILogger logger)
	{
		Db = db;
		Sfm = sfm;
		Logger = logger;
	}

	protected LocoDbContext Db { get; }

	protected ServerFolderManager Sfm { get; }

	protected ILogger Logger { get; }

	public abstract Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct);

	public abstract Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct);

	public abstract Task ReconcileAsync(CancellationToken ct);

	protected static bool IsDatFile(string path)
		=> Path.GetExtension(path).Equals(".dat", StringComparison.OrdinalIgnoreCase);

	protected static bool IsSc5File(string path)
		=> Path.GetExtension(path).Equals(".sc5", StringComparison.OrdinalIgnoreCase);

	protected static IEnumerable<string> EnumerateFiles(string folder, Func<string, bool> predicate)
		=> Directory.Exists(folder)
			? Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
				.Where(path => !ServerFolderManager.IsUnderRemovedFolder(folder, path))
				.Where(predicate)
			: [];

	/// <summary>
	/// Runs one file's import or removal during a reconciliation pass. Failures are logged against the
	/// offending file and swallowed rather than propagated, so a single malformed file (or a data
	/// conflict such as a duplicate object name) can never abort the whole reconcile and leave the
	/// remaining files unprocessed.
	/// </summary>
	protected async Task<GameDataImportResult?> TryReconcileFileAsync(
		string absolutePath,
		Func<Task<GameDataImportResult>> action,
		CancellationToken ct)
	{
		try
		{
			var result = await action().ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Failed)
			{
				Logger.LogWarning("Failed to reconcile \"{Path}\": {Message}", absolutePath, result.Message);
			}
			else if (result.Status is GameDataImportStatus.Duplicate)
			{
				Logger.LogDebug("Ignored duplicate file \"{Path}\": {Message}", absolutePath, result.Message);
			}

			return result;
		}
		catch (OperationCanceledException) when (ct.IsCancellationRequested)
		{
			// Normal shutdown - let it propagate.
			throw;
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Failed to reconcile \"{Path}\"; continuing with the remaining files", absolutePath);
			return null;
		}
	}

	/// <summary>
	/// Determines the object source of a file from the Original/OpenLoco/Custom subfolder it lives in.
	/// </summary>
	protected static ObjectSource GetObjectSource(string absolutePath, string categoryFolder)
	{
		if (IsUnder(absolutePath, Path.Combine(categoryFolder, ServerFolderManager.OpenLocoFolderName)))
		{
			return ObjectSource.OpenLoco;
		}

		if (IsUnder(absolutePath, Path.Combine(categoryFolder, ServerFolderManager.OriginalFolderName)))
		{
			return ObjectSource.LocomotionSteam;
		}

		return ObjectSource.Custom;
	}

	protected static bool IsUnder(string path, string folder)
	{
		var relative = Path.GetRelativePath(folder, path);
		return !relative.StartsWith("..", StringComparison.Ordinal) && !Path.IsPathRooted(relative);
	}

	/// <summary>
	/// Reconciles the availability of this entity's database rows against the files on disk. The
	/// database is the source of truth, so a row is never deleted: a row whose file is missing is
	/// marked <see cref="ObjectAvailability.Unavailable"/> and kept (curated metadata such as tags,
	/// authors and licence, plus any pack/scenario references, survive), and a row whose file is
	/// present is (re)marked <see cref="ObjectAvailability.Available"/>. A row is only considered when
	/// <paramref name="ownsName"/> accepts its name and the name resolves to a path inside
	/// <paramref name="scanFolder"/>: scenarios and landscapes share the <c>Scenarios</c> table but
	/// store their names relative to different roots, so each service must ignore rows that belong to
	/// a sibling folder. Rows with an unresolvable or unsafe name are left untouched.
	/// </summary>
	/// <returns>The number of rows whose availability changed.</returns>
	protected async Task<int> SyncAvailabilityWithDiskAsync<TEntity>(
		DbSet<TEntity> set,
		string nameRoot,
		string scanFolder,
		Func<string, bool> ownsName,
		CancellationToken ct)
		where TEntity : DbFileObject
	{
		var changed = 0;

		foreach (var row in await set.ToListAsync(ct).ConfigureAwait(false))
		{
			if (string.IsNullOrWhiteSpace(row.Name)
				|| !ownsName(row.Name)
				|| !RouteHelpers.TryGetSafeRelativePathUnderRoot(nameRoot, row.Name, out var fullPath, out _)
				|| !IsUnder(fullPath, scanFolder))
			{
				continue;
			}

			var availability = ObjectAvailabilityRules.ForFile(row.ObjectSource, File.Exists(fullPath));
			if (row.Availability != availability)
			{
				row.Availability = availability;
				changed++;
			}
		}

		if (changed > 0)
		{
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
		}

		return changed;
	}
}
