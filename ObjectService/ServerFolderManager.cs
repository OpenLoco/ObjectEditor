using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using Zenith.Core;

namespace OpenLoco.ObjectService
{
	/// <summary>
	/// This class represents the folder structure for objects on the server.
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
			Verify.AreEqual(true, Directory.Exists(ScenariosOriginalFolder), message: $"ScenariosOriginalFolder: {ScenariosOriginalFolder} didn't exist");
			Verify.AreEqual(true, Directory.Exists(ScenariosCustomFolder), message: $"ScenariosCustomFolder: {ScenariosCustomFolder} didn't exist");
		}

		public ObjectIndex ObjectIndex { get; init; }

		public const string ObjectsFolderName = "Objects";
		public const string ScenariosFolderName = "Scenarios";

		public const string OriginalFolderName = "Original";
		public const string CustomFolderName = "Custom";

		public string IndexFile => Path.Combine(RootDirectory, ObjectsFolderName, ObjectIndex.DefaultIndexFileName);
		public string ObjectsFolder => Path.Combine(RootDirectory, ObjectsFolderName);
		public string ObjectsOriginalFolder => Path.Combine(ObjectsFolder, OriginalFolderName);
		public string ObjectsCustomFolder => Path.Combine(ObjectsFolder, CustomFolderName);

		public string ScenariosFolder => Path.Combine(RootDirectory, ScenariosFolderName);
		public string ScenariosOriginalFolder => Path.Combine(ScenariosFolder, OriginalFolderName);
		public string ScenariosCustomFolder => Path.Combine(ScenariosFolder, CustomFolderName);

		// === structure ===
		// - Objects
		//   - objectIndex.json
		//   - Custom
		//       - ...
		//   - Original
		//       - Steam
		//         - ...
		//       - GoG
		//         - ...
		// - Scenarios
		//   - Custom
		//       - ...
		//   - Original
		//       - Steam
		//         - ...
		//       - GoG
		//         - ...
	}
}
