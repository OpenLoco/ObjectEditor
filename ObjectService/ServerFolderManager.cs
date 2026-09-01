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
/// <para>This class represents the folder structure for objects on the server.</para>
/// <para>
/// === structure ===
/// - GameData
///   - Graphics
///   - Music
///   - SoundEffects
///   - Tutorials
/// - Objects
///   - objectIndex.json
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

public ServerFolderManager(string rootDirectory)
{
if (!Directory.Exists(rootDirectory))
{
throw new DirectoryNotFoundException($"The specified root directory does not exist: {rootDirectory}");
}

RootDirectory = rootDirectory;

ILogger logger = new Common.Logging.Logger();

var indexFile = Path.Combine(rootDirectory, ObjectsFolderName);
try
{
ObjectIndex = ObjectIndex.LoadOrCreateIndex(indexFile, logger)!;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
{
// Index file is corrupt or otherwise unreadable. Log the original failure
// before destroying the file so we can diagnose recurring corruption.
logger.LogError(ex, "Failed to load object index at \"{IndexFile}\"; deleting and recreating.", indexFile);
try
{
File.Delete(indexFile);
}
catch (Exception deleteEx) when (deleteEx is IOException or UnauthorizedAccessException)
{
logger.LogError(deleteEx, "Failed to delete corrupt index file \"{IndexFile}\".", indexFile);
throw;
}

try
{
ObjectIndex = ObjectIndex.LoadOrCreateIndex(indexFile, logger)!;
}
catch (Exception retryEx) when (retryEx is IOException or UnauthorizedAccessException or JsonException)
{
logger.LogError(retryEx, "Failed to recreate object index at \"{IndexFile}\".", indexFile);
throw;
}
}

EnsureDirectoryExists(ObjectsOriginalFolder);
EnsureDirectoryExists(ObjectsCustomFolder);

EnsureDirectoryExists(GraphicsOriginalFolder);
EnsureDirectoryExists(GraphicsCustomFolder);

EnsureDirectoryExists(MusicOriginalFolder);
EnsureDirectoryExists(MusicCustomFolder);

EnsureDirectoryExists(SoundEffectsOriginalFolder);
EnsureDirectoryExists(SoundEffectsCustomFolder);

EnsureDirectoryExists(TutorialsOriginalFolder);
EnsureDirectoryExists(TutorialsCustomFolder);

EnsureDirectoryExists(ScenariosOriginalFolder);
EnsureDirectoryExists(ScenariosCustomFolder);
EnsureDirectoryExists(ScenariosGoGFolder);
EnsureDirectoryExists(ScenariosSteamFolder);
}

private static void EnsureDirectoryExists(string path)
{
if (!Directory.Exists(path))
{
throw new DirectoryNotFoundException($"Required server directory not found: {path}");
}
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

#region Objects

public string IndexFile => Path.Combine(RootDirectory, ObjectsFolderName, ObjectIndex.DefaultIndexFileName);
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
}
