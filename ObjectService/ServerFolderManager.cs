using Definitions;
using Definitions.Database;
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
/// <para>This class represents the folder structure for objects on the server.</para>
/// <para>
/// === structure ===
/// - GameData
///   - Graphics
///   - Music
///   - SoundEffects
///   - Tutorials
/// - Objects
///   - Custom
///       - ...
///   - Original
///       - Steam
///         - ...
///       - GoG
///         - ...
/// - Scenarios
///   - Custom
///       - ...
///   - Original
///       - Steam
///         - ...
///       - GoG
///         - ...
/// </para>
///
/// </summary>
public class ServerFolderManager : IServerFolderManager
{
	string RootDirectory { get; init; }

	public ServerFolderManager(string rootDirectory, string databasePath)
	{
		if (!Directory.Exists(rootDirectory))
		{
			throw new DirectoryNotFoundException($"The specified root directory does not exist: {rootDirectory}");
		}

		RootDirectory = rootDirectory;
		DatabasePath = databasePath;

		ILogger logger = new Common.Logging.Logger();

		try
		{
			using var db = BaseLocoDbContext.GetDbFromFile(databasePath)
				?? throw new FileNotFoundException($"Database file not found: {databasePath}");
			ObjectIndex = ObjectIndex.FromDb(db);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to build object index from database \"{DatabasePath}\".", databasePath);
			throw;
		}

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ObjectsOriginalFolder), nameof(ObjectsOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ObjectsCustomFolder), nameof(ObjectsCustomFolder));

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(GraphicsOriginalFolder), nameof(GraphicsOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(GraphicsCustomFolder), nameof(GraphicsCustomFolder));

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(MusicOriginalFolder), nameof(MusicOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(MusicCustomFolder), nameof(MusicCustomFolder));

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(SoundEffectsOriginalFolder), nameof(SoundEffectsOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(SoundEffectsCustomFolder), nameof(SoundEffectsCustomFolder));

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(TutorialsOriginalFolder), nameof(TutorialsOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(TutorialsCustomFolder), nameof(TutorialsCustomFolder));

		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosOriginalFolder), nameof(ScenariosOriginalFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosCustomFolder), nameof(ScenariosCustomFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosGoGFolder), nameof(ScenariosGoGFolder));
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosSteamFolder), nameof(ScenariosSteamFolder));
	}

	public ObjectIndex ObjectIndex { get; init; }

	public const string ObjectsFolderName = "Objects";

	public const string LandscapesFolderName = "Landscapes";
	public const string ScenariosFolderName = "Scenarios";
	public const string GameDataFolderName = "GameData";

	public const string GraphicsFolderName = "Graphics";
	public const string MusicFolderName = "Music";
	public const string SoundEffectsFolderName = "SoundEffects";
	public const string TutorialsFolderName = "Tutorials";

	public const string OriginalFolderName = "Original";
	public const string CustomFolderName = "Custom";

	public const string SteamFolderName = "Steam";
	public const string GoGFolderName = "GoG";


	public string DatabasePath { get; init; }
	#region Objects

	public string IndexFile => DatabasePath;
	public string ObjectsFolder => Path.Combine(RootDirectory, ObjectsFolderName);
	public string ObjectsOriginalFolder => Path.Combine(ObjectsFolder, OriginalFolderName);
	public string ObjectsCustomFolder => Path.Combine(ObjectsFolder, CustomFolderName);

	#endregion

	#region GameData

	public string GameDataFolder => Path.Combine(RootDirectory, GameDataFolderName);

	public string GraphicsOriginalFolder => Path.Combine(GameDataFolder, GraphicsFolderName, OriginalFolderName);
	public string GraphicsCustomFolder => Path.Combine(GameDataFolder, GraphicsFolderName, CustomFolderName);
	public string MusicOriginalFolder => Path.Combine(GameDataFolder, MusicFolderName, OriginalFolderName);
	public string MusicCustomFolder => Path.Combine(GameDataFolder, MusicFolderName, CustomFolderName);
	public string SoundEffectsOriginalFolder => Path.Combine(GameDataFolder, SoundEffectsFolderName, OriginalFolderName);
	public string SoundEffectsCustomFolder => Path.Combine(GameDataFolder, SoundEffectsFolderName, CustomFolderName);
	public string TutorialsOriginalFolder => Path.Combine(GameDataFolder, TutorialsFolderName, OriginalFolderName);
	public string TutorialsCustomFolder => Path.Combine(GameDataFolder, TutorialsFolderName, CustomFolderName);

	#endregion

	#region SCV5

	public string LandscapesFolder => Path.Combine(RootDirectory, LandscapesFolderName);
	public string ScenariosFolder => Path.Combine(RootDirectory, ScenariosFolderName);

	public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
	public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);
	public string ScenariosGoGFolder => Path.Combine(ScenariosOriginalFolder, GoGFolderName);
	public string ScenariosSteamFolder => Path.Combine(ScenariosOriginalFolder, SteamFolderName);

	#endregion

	// Creates the entire folder tree expected by ServerFolderManager under the given root.
	// Idempotent: existing folders are left untouched. Use this to bootstrap a fresh local
	// objects root (eg the GUI's embedded backend) before constructing the manager.
	public static void EnsureFolderStructure(string rootDirectory)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);

		string[] required =
		[
			Path.Combine(rootDirectory, ObjectsFolderName, OriginalFolderName),
			Path.Combine(rootDirectory, ObjectsFolderName, CustomFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, GraphicsFolderName, OriginalFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, GraphicsFolderName, CustomFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, MusicFolderName, OriginalFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, MusicFolderName, CustomFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, SoundEffectsFolderName, OriginalFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, SoundEffectsFolderName, CustomFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, TutorialsFolderName, OriginalFolderName),
			Path.Combine(rootDirectory, GameDataFolderName, TutorialsFolderName, CustomFolderName),
			Path.Combine(rootDirectory, LandscapesFolderName),
			Path.Combine(rootDirectory, ScenariosFolderName, OriginalFolderName, GoGFolderName),
			Path.Combine(rootDirectory, ScenariosFolderName, OriginalFolderName, SteamFolderName),
			Path.Combine(rootDirectory, ScenariosFolderName, CustomFolderName),
		];

		foreach (var path in required)
		{
			_ = Directory.CreateDirectory(path);
		}
	}
}
