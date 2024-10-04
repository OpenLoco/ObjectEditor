using OpenLoco.Dat;
using Zenith.Core;

namespace OpenLoco.ObjectService
{
	/// <summary>
	/// This class represents the folder structure for objects on the server.
	/// </summary>
	public class ObjectFolderManager
	{
		public string RootDirectory { get; init; }

		public ObjectFolderManager(string rootDirectory)
		{
			RootDirectory = rootDirectory;
			ObjectIndex = ObjectIndex.LoadOrCreateIndex(Path.Combine(rootDirectory, ObjectsFolderName))!;

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
		public string ObjectsOriginalFolder => Path.Combine(RootDirectory, ObjectsFolderName, OriginalFolderName);
		public string ObjectsCustomFolder => Path.Combine(RootDirectory, ObjectsFolderName, CustomFolderName);

		public string ScenariosOriginalFolder => Path.Combine(RootDirectory, ScenariosFolderName, OriginalFolderName);
		public string ScenariosCustomFolder => Path.Combine(RootDirectory, ScenariosFolderName, CustomFolderName);

		// === structure ===
		// - Scenarios
		//   - Original
		//   - Custom
		// - Objects
		//   - objectIndex.json
		//   - Custom
		//       - ...
		//   - Original
		//       - Steam
		//         - ...
		//       - GoG
		//         - ...
	}
}
