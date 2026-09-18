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
///     - objectIndex.json
///   - Landscapes
///     - Original
///     - Custom
///     - OpenLoco
///   - Scenarios
///     - Original
///     - Custom
///     - OpenLoco
///   - Tutorials
///     - Original
///     - Custom
///     - OpenLoco
///   - SoundEffects
///     - Original
///     - Custom
///     - OpenLoco
///   - Music
///     - Original
///     - Custom
///     - OpenLoco
///   - Graphics
///     - Original
///     - Custom
///     - OpenLoco
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

	#region GameData

	public string GameDataFolder => Path.Combine(RootDirectory, GameDataFolderName);

	#endregion

	#region Objects

	public string IndexFile => Path.Combine(ObjectsFolder, ObjectIndex.DefaultIndexFileName);
	public string ObjectsFolder => Path.Combine(GameDataFolder, ObjectsFolderName);
	public string ObjectsOriginalFolder => Path.Combine(ObjectsFolder, OriginalFolderName);
	public string ObjectsCustomFolder => Path.Combine(ObjectsFolder, CustomFolderName);
	public string ObjectsOpenLocoFolder => Path.Combine(ObjectsFolder, OpenLocoFolderName);

	#endregion

	#region Landscapes

	public string LandscapesFolder => Path.Combine(GameDataFolder, LandscapesFolderName);
	public string LandscapesOriginalFolder => Path.Combine(LandscapesFolder, OriginalFolderName);
	public string LandscapesCustomFolder => Path.Combine(LandscapesFolder, CustomFolderName);
	public string LandscapesOpenLocoFolder => Path.Combine(LandscapesFolder, OpenLocoFolderName);

	#endregion

	#region Scenarios

	public string ScenariosFolder => Path.Combine(GameDataFolder, ScenariosFolderName);
	public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
	public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);
	public string ScenariosOpenLocoFolder => Path.Combine(ScenariosFolder, OpenLocoFolderName);

	#endregion

	#region Tutorials

	public string TutorialsFolder => Path.Combine(GameDataFolder, TutorialsFolderName);
	public string TutorialsOriginalFolder => Path.Combine(TutorialsFolder, OriginalFolderName);
	public string TutorialsCustomFolder => Path.Combine(TutorialsFolder, CustomFolderName);
	public string TutorialsOpenLocoFolder => Path.Combine(TutorialsFolder, OpenLocoFolderName);

	#endregion

	#region SoundEffects

	public string SoundEffectsFolder => Path.Combine(GameDataFolder, SoundEffectsFolderName);
	public string SoundEffectsOriginalFolder => Path.Combine(SoundEffectsFolder, OriginalFolderName);
	public string SoundEffectsCustomFolder => Path.Combine(SoundEffectsFolder, CustomFolderName);
	public string SoundEffectsOpenLocoFolder => Path.Combine(SoundEffectsFolder, OpenLocoFolderName);

	#endregion

	#region Music

	public string MusicFolder => Path.Combine(GameDataFolder, MusicFolderName);
	public string MusicOriginalFolder => Path.Combine(MusicFolder, OriginalFolderName);
	public string MusicCustomFolder => Path.Combine(MusicFolder, CustomFolderName);
	public string MusicOpenLocoFolder => Path.Combine(MusicFolder, OpenLocoFolderName);

	#endregion

	#region Graphics

	public string GraphicsFolder => Path.Combine(GameDataFolder, GraphicsFolderName);
	public string GraphicsOriginalFolder => Path.Combine(GraphicsFolder, OriginalFolderName);
	public string GraphicsCustomFolder => Path.Combine(GraphicsFolder, CustomFolderName);
	public string GraphicsOpenLocoFolder => Path.Combine(GraphicsFolder, OpenLocoFolderName);

	#endregion
}
