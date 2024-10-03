using OpenLoco.Dat;

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
		}
		public ObjectIndex ObjectIndex { get; init; }

		public const string ObjectsFolderName = "Objects";
		public const string OriginalObjectsFolderName = "Original";
		public const string CustomObjectFolderName = "Custom";
		public const string ScenariosFolderName = "Scenarios";

		public string IndexFile => Path.Combine(RootDirectory, ObjectsFolderName, ObjectIndex.DefaultIndexFileName);
		public string OriginalObjectFolder => Path.Combine(RootDirectory, ObjectsFolderName, OriginalObjectsFolderName);
		public string CustomObjectFolder => Path.Combine(RootDirectory, ObjectsFolderName, CustomObjectFolderName);

		// === structure ===
		// - Scenarios
		//   - ...
		// - Objects
		//   - objectIndex.json
		//   - CustomObjects
		//       - ...
		//   - OriginalObjects
		//       - Steam
		//         - ...
		//       - GoG
		//         - ...
	}
}
