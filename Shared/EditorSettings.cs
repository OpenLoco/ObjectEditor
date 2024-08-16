namespace OpenLoco.Dat.Settings
{
	public class EditorSettings
	{
		public string ObjDataDirectory
		{
			get => objectDirectory;
			set
			{
				objectDirectory = value;
				ObjDataDirectories ??= [];
				_ = ObjDataDirectories.Add(objectDirectory);
			}
		}
		string objectDirectory;

		public HashSet<string> ObjDataDirectories
		{
			get => objDataDirectories ??= [];
			set => objDataDirectories = value;
		}
		HashSet<string> objDataDirectories;

		public string DataDirectory
		{
			get => dataDirectory;
			set
			{
				dataDirectory = value;
				DataDirectories ??= [];
				_ = DataDirectories.Add(dataDirectory);
			}
		}
		string dataDirectory;

		public HashSet<string> DataDirectories
		{
			get => dataDirectories ??= [];
			set => dataDirectories = value;
		}
		HashSet<string> dataDirectories;

		public string SCV5Directory
		{
			get => scv5Directory;
			set
			{
				scv5Directory = value;
				SCV5Directories ??= [];
				_ = SCV5Directories.Add(scv5Directory);
			}
		}
		string scv5Directory;

		public HashSet<string> SCV5Directories
		{
			get => scv5Directories ??= [];
			set => scv5Directories = value;
		}
		HashSet<string> scv5Directories;

		public string PaletteFile { get; set; } = "palette.png";
		public string MetadataFileName { get; set; } = "objectMetadata.json";
		public string IndexFileName { get; set; } = "objectIndex.json";
		public string G1DatFileName { get; set; } = "g1.DAT";
		public string GetObjDataFullPath(string fileName) => Path.Combine(ObjDataDirectory, fileName);
		public string GetDataFullPath(string fileName) => Path.Combine(DataDirectory, fileName);
	}
}
