using OpenLoco.Common.Logging;
using OpenLoco.Dat;

namespace OpenLoco.ObjectService
{
	/// <summary>
	/// This class represents the folder structure for objects on the server.
	///
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
	/// 
	/// </summary>
	public class ServerFolderManager
	{
		string RootDirectory { get; init; }

		public ServerFolderManager(string rootDirectory)
		{
			RootDirectory = rootDirectory;
			var logger = new Logger();
			ObjectIndex = ObjectIndex.LoadOrCreateIndex(Path.Combine(rootDirectory, ObjectsFolderName), logger)!;

			// note these don't work on the linux box/server
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ObjectsOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ObjectsCustomFolder));

			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(GraphicsOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(GraphicsCustomFolder));

			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(MusicOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(MusicCustomFolder));

			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(SoundEffectsOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(SoundEffectsCustomFolder));

			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(TutorialsOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(TutorialsCustomFolder));

			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosOriginalFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosCustomFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosGoGFolder));
			// ArgumentOutOfRangeException.ThrowIfNotEqual(true, Directory.Exists(ScenariosSteamFolder));
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

		public const string LandscapesFolderName = "Landscapes";
		public const string ScenariosFolderName = "Scenarios";
		public const string SaveGamesFolderName = "SaveGames";

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
		public string SaveGamesFolder => Path.Combine(RootDirectory, SaveGamesFolderName);
		public string ScenariosFolder => Path.Combine(RootDirectory, ScenariosFolderName);

		public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
		public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);
		public string ScenariosGoGFolder => Path.Combine(ScenariosOriginalFolder, GoGFolderName);
		public string ScenariosSteamFolder => Path.Combine(ScenariosOriginalFolder, SteamFolderName);

		#endregion
	}
}
