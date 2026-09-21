using Definitions.Database;
using Definitions.ObjectModels.Types;
using Index;

namespace ObjectService.Services;

/// <summary>The outcome of processing a single game-data file detected on disk.</summary>
public enum GameDataImportStatus
{
	Added,
	Updated,
	Removed,
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
}
