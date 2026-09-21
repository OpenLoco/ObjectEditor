using Index;
using System.Text.Json;

namespace ObjectService;

public interface IServerFolderManager
{
	//string RootDirectory { get; init; }
}

public class TestServerFolderManager : IServerFolderManager
{
	//string RootDirectory { get; init; }
}

/// <summary>
/// <para>This class represents the folder structure for game data on the server.</para>
/// <para>
/// === structure ===
/// - GameData
///   - Objects
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///     - objectIndex.json
///   - Landscapes
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///   - Scenarios
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///   - Tutorials
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///   - SoundEffects
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///   - Music
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
///   - Graphics
///     - Original
///     - Custom
///     - OpenLoco
///     - Removed
/// </para>
/// <para>
/// Every category folder also contains a <c>Removed</c> subfolder. Files "deleted" through the API are
/// moved there rather than deleted so a removal is recoverable, and <c>Removed</c> is ignored by the file
/// watchers so a parked file is never re-indexed. Parked files keep their relative path, e.g.
/// <c>GameData/Objects/Custom/x.dat</c> becomes <c>GameData/Objects/Removed/Custom/x.dat</c>.
/// </para>
///
/// </summary>
public class ServerFolderManager : IServerFolderManager
{
	string RootDirectory { get; init; }

	public ServerFolderManager(string rootDirectory)
	{
		if (!Directory.Exists(rootDirectory))
		{
			throw new DirectoryNotFoundException($"The specified root directory does not exist: {rootDirectory}");
		}

		RootDirectory = rootDirectory;

		// Build the full GameData folder structure up-front so the object index can be
		// loaded/created and files can be written without any further existence checks.
		CreateDataFolders();

		ILogger logger = new Common.Logging.Logger();

		var indexDirectory = ObjectsFolder;
		try
		{
			ObjectIndex = ObjectIndex.LoadOrCreateIndex(indexDirectory, logger)!;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
		{
			// Index file is corrupt or otherwise unreadable. Log the original failure
			// before destroying the file so we can diagnose recurring corruption.
			logger.LogError(ex, "Failed to load object index at \"{IndexFile}\"; deleting and recreating.", IndexFile);
			try
			{
				File.Delete(IndexFile);
			}
			catch (Exception deleteEx) when (deleteEx is IOException or UnauthorizedAccessException)
			{
				logger.LogError(deleteEx, "Failed to delete corrupt index file \"{IndexFile}\".", IndexFile);
				throw;
			}

			try
			{
				ObjectIndex = ObjectIndex.LoadOrCreateIndex(indexDirectory, logger)!;
			}
			catch (Exception retryEx) when (retryEx is IOException or UnauthorizedAccessException or JsonException)
			{
				logger.LogError(retryEx, "Failed to recreate object index at \"{IndexFile}\".", IndexFile);
				throw;
			}
		}
	}

	/// <summary>
	/// Creates the <see cref="GameDataFolder"/> and the uniform <c>Original</c>, <c>Custom</c>
	/// and <c>OpenLoco</c> subfolders that every category folder must contain.
	/// </summary>
	private void CreateDataFolders()
	{
		string[] categoryFolders =
		[
			ObjectsFolder,
			LandscapesFolder,
			ScenariosFolder,
			TutorialsFolder,
			SoundEffectsFolder,
			MusicFolder,
			GraphicsFolder,
		];

		foreach (var categoryFolder in categoryFolders)
		{
			EnsureDirectoryExists(categoryFolder);
			EnsureDirectoryExists(Path.Combine(categoryFolder, OriginalFolderName));
			EnsureDirectoryExists(Path.Combine(categoryFolder, CustomFolderName));
			EnsureDirectoryExists(Path.Combine(categoryFolder, OpenLocoFolderName));
			EnsureDirectoryExists(Path.Combine(categoryFolder, RemovedFolderName));
		}
	}

	/// <summary>
	/// Ensures the directory at <paramref name="path"/> (and any missing parents) exists.
	/// </summary>
	private static void EnsureDirectoryExists(string path)
		=> _ = Directory.CreateDirectory(path);

	public ObjectIndex ObjectIndex { get; init; }

	public const string GameDataFolderName = "GameData";

	public const string ObjectsFolderName = "Objects";
	public const string LandscapesFolderName = "Landscapes";
	public const string ScenariosFolderName = "Scenarios";
	public const string TutorialsFolderName = "Tutorials";
	public const string SoundEffectsFolderName = "SoundEffects";
	public const string MusicFolderName = "Music";
	public const string GraphicsFolderName = "Graphics";

	public const string OriginalFolderName = "Original";
	public const string CustomFolderName = "Custom";
	public const string OpenLocoFolderName = "OpenLoco";

	/// <summary>
	/// Subfolder every category folder gets for files that have been "deleted" through the API. Files are
	/// moved here (rather than deleted) so a removal is recoverable, and the folder is ignored by the file
	/// watchers so removed files are never re-indexed.
	/// </summary>
	public const string RemovedFolderName = "Removed";

	static readonly StringComparison PathComparison = OperatingSystem.IsWindows()
		? StringComparison.OrdinalIgnoreCase
		: StringComparison.Ordinal;

	/// <summary>
	/// Builds the path, relative to <see cref="ObjectsFolder"/>, that an uploaded game object is stored
	/// at. Object-index entries always use relative paths so the index stays portable between machines.
	/// </summary>
	public static string GetCustomObjectRelativeFileName(Guid uuid)
		=> Path.Combine(CustomFolderName, $"{uuid}.dat");

	/// <summary>
	/// Moves a game-data file into the category's <c>Removed</c> subfolder, preserving the path relative to
	/// the category folder. The <c>Removed</c> folder is ignored by the file watchers, so the file stops
	/// being indexed but is kept for recovery.
	/// </summary>
	/// <returns>The file's new absolute path, or <see langword="null"/> when it is outside the category,
	/// already removed, or does not exist.</returns>
	public static string? MoveToRemovedFolder(string categoryFolder, string absolutePath)
	{
		if (string.IsNullOrWhiteSpace(absolutePath) || !File.Exists(absolutePath))
		{
			return null;
		}

		var categoryRoot = GetFolderFullPath(categoryFolder);
		var sourcePath = Path.GetFullPath(absolutePath);
		if (!sourcePath.StartsWith(categoryRoot, PathComparison))
		{
			return null;
		}

		if (IsUnderRemovedFolder(categoryRoot, sourcePath))
		{
			return null; // already removed
		}

		var destination = Path.Combine(categoryRoot, RemovedFolderName, Path.GetRelativePath(categoryRoot, sourcePath));
		_ = Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

		// Never overwrite a file that was removed earlier.
		var unique = destination;
		for (var i = 1; File.Exists(unique); i++)
		{
			unique = Path.Combine(
				Path.GetDirectoryName(destination)!,
				$"{Path.GetFileNameWithoutExtension(destination)} ({i}){Path.GetExtension(destination)}");
		}

		File.Move(sourcePath, unique);
		return unique;
	}

	/// <summary>
	/// Returns <see langword="true"/> when <paramref name="path"/> is inside the <c>Removed</c> subfolder of
	/// <paramref name="categoryFolder"/>.
	/// </summary>
	public static bool IsUnderRemovedFolder(string categoryFolder, string path)
		=> Path.GetFullPath(path).StartsWith(GetFolderFullPath(Path.Combine(categoryFolder, RemovedFolderName)), PathComparison);

	static string GetFolderFullPath(string folder)
	{
		var fullPath = Path.GetFullPath(folder);
		return fullPath.EndsWith(Path.DirectorySeparatorChar)
			? fullPath
			: fullPath + Path.DirectorySeparatorChar;
	}

	#region GameData

	public string GameDataFolder => Path.Combine(RootDirectory, GameDataFolderName);

	#endregion

	#region Objects

	public string IndexFile => Path.Combine(ObjectsFolder, ObjectIndex.DefaultIndexFileName);
	public string ObjectsFolder => Path.Combine(GameDataFolder, ObjectsFolderName);
	public string ObjectsOriginalFolder => Path.Combine(ObjectsFolder, OriginalFolderName);
	public string ObjectsCustomFolder => Path.Combine(ObjectsFolder, CustomFolderName);
	public string ObjectsOpenLocoFolder => Path.Combine(ObjectsFolder, OpenLocoFolderName);
	public string ObjectsRemovedFolder => Path.Combine(ObjectsFolder, RemovedFolderName);

	#endregion

	#region Landscapes

	public string LandscapesFolder => Path.Combine(GameDataFolder, LandscapesFolderName);
	public string LandscapesOriginalFolder => Path.Combine(LandscapesFolder, OriginalFolderName);
	public string LandscapesCustomFolder => Path.Combine(LandscapesFolder, CustomFolderName);
	public string LandscapesOpenLocoFolder => Path.Combine(LandscapesFolder, OpenLocoFolderName);
	public string LandscapesRemovedFolder => Path.Combine(LandscapesFolder, RemovedFolderName);

	#endregion

	#region Scenarios

	public string ScenariosFolder => Path.Combine(GameDataFolder, ScenariosFolderName);
	public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
	public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);
	public string ScenariosOpenLocoFolder => Path.Combine(ScenariosFolder, OpenLocoFolderName);
	public string ScenariosRemovedFolder => Path.Combine(ScenariosFolder, RemovedFolderName);

	#endregion

	#region Tutorials

	public string TutorialsFolder => Path.Combine(GameDataFolder, TutorialsFolderName);
	public string TutorialsOriginalFolder => Path.Combine(TutorialsFolder, OriginalFolderName);
	public string TutorialsCustomFolder => Path.Combine(TutorialsFolder, CustomFolderName);
	public string TutorialsOpenLocoFolder => Path.Combine(TutorialsFolder, OpenLocoFolderName);
	public string TutorialsRemovedFolder => Path.Combine(TutorialsFolder, RemovedFolderName);

	#endregion

	#region SoundEffects

	public string SoundEffectsFolder => Path.Combine(GameDataFolder, SoundEffectsFolderName);
	public string SoundEffectsOriginalFolder => Path.Combine(SoundEffectsFolder, OriginalFolderName);
	public string SoundEffectsCustomFolder => Path.Combine(SoundEffectsFolder, CustomFolderName);
	public string SoundEffectsOpenLocoFolder => Path.Combine(SoundEffectsFolder, OpenLocoFolderName);
	public string SoundEffectsRemovedFolder => Path.Combine(SoundEffectsFolder, RemovedFolderName);

	#endregion

	#region Music

	public string MusicFolder => Path.Combine(GameDataFolder, MusicFolderName);
	public string MusicOriginalFolder => Path.Combine(MusicFolder, OriginalFolderName);
	public string MusicCustomFolder => Path.Combine(MusicFolder, CustomFolderName);
	public string MusicOpenLocoFolder => Path.Combine(MusicFolder, OpenLocoFolderName);
	public string MusicRemovedFolder => Path.Combine(MusicFolder, RemovedFolderName);

	#endregion

	#region Graphics

	public string GraphicsFolder => Path.Combine(GameDataFolder, GraphicsFolderName);
	public string GraphicsOriginalFolder => Path.Combine(GraphicsFolder, OriginalFolderName);
	public string GraphicsCustomFolder => Path.Combine(GraphicsFolder, CustomFolderName);
	public string GraphicsOpenLocoFolder => Path.Combine(GraphicsFolder, OpenLocoFolderName);
	public string GraphicsRemovedFolder => Path.Combine(GraphicsFolder, RemovedFolderName);

	#endregion
}
