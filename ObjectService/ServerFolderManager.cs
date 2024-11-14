using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using Zenith.Core;

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

			Verify.AreEqual(true, Directory.Exists(ObjectsOriginalFolder), message: $"OrignalObjectsFolder: {ObjectsOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(ObjectsCustomFolder), message: $"ObjectsCustomFolder: {ObjectsCustomFolder} didn't exist");

			Verify.AreEqual(true, Directory.Exists(GraphicsOriginalFolder), message: $"GraphicsOriginalFolder: {GraphicsOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(GraphicsCustomFolder), message: $"GraphicsCustomFolder: {GraphicsCustomFolder} didn't exist");

			Verify.AreEqual(true, Directory.Exists(MusicOriginalFolder), message: $"MusicOriginalFolder: {MusicOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(MusicCustomFolder), message: $"MusicCustomFolder: {MusicCustomFolder} didn't exist");

			Verify.AreEqual(true, Directory.Exists(SoundEffectsOriginalFolder), message: $"SoundEffectsOriginalFolder: {SoundEffectsOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(SoundEffectsCustomFolder), message: $"SoundEffectsCustomFolder: {SoundEffectsCustomFolder} didn't exist");

			Verify.AreEqual(true, Directory.Exists(TutorialsOriginalFolder), message: $"TutorialsOriginalFolder: {TutorialsOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(TutorialsCustomFolder), message: $"TutorialsCustomFolder: {TutorialsCustomFolder} didn't exist");

			Verify.AreEqual(true, Directory.Exists(ScenariosOriginalFolder), message: $"ScenariosOriginalFolder: {ScenariosOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(ScenariosCustomFolder), message: $"ScenariosCustomFolder: {ScenariosCustomFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(ScenariosGoGFolder), message: $"ScenariosGoGFolder: {ScenariosGoGFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(ScenariosSteamFolder), message: $"ScenariosSteamFolder: {ScenariosSteamFolder} didn't exist");
		}

		public ObjectIndex ObjectIndex { get; init; }

		public const string ObjectsFolderName = "Objects";

		public const string GameDataFolderName = "GameData";
		public const string GraphicsFolderName = "Graphics";
		public const string MusicFolderName = "Music";
		public const string SoundEffectsFolderName = "SoundEffects";
		public const string TutorialsFolderName = "Tutorials";

		public const string SCV5FolderName = "SCV5";
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

		public string SCV5Folder => Path.Combine(RootDirectory, SCV5FolderName);

		public string LandscapesFolder => Path.Combine(SCV5Folder, LandscapesFolderName);
		public string SaveGamesFolder => Path.Combine(SCV5Folder, SaveGamesFolderName);
		public string ScenariosFolder => Path.Combine(SCV5Folder, ScenariosFolderName);

		public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
		public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);
		public string ScenariosGoGFolder => Path.Combine(ScenariosOriginalFolder, GoGFolderName);
		public string ScenariosSteamFolder => Path.Combine(ScenariosOriginalFolder, SteamFolderName);

		#endregion
	}
}
