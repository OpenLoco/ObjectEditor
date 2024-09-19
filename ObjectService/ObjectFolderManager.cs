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
			Index = ObjectIndex.LoadOrCreateIndex(rootDirectory)!;
		}

		public ObjectIndex Index { get; init; }

		public string IndexFile => Path.Combine(RootDirectory, ObjectIndex.DefaultIndexFileName);
		public string OriginalObjectFolder => Path.Combine(RootDirectory, "Original");
		public string CustomObjectFolder => Path.Combine(RootDirectory, "Custom");

		// === structure ===
		// - objectIndex.json
		// - CustomObjects
		//     - ...
		// - OriginalObjects
		//     - Steam
		//       - ...
		//     - GoG
		//       - ...
	}
}
