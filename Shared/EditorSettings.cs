namespace OpenLoco.ObjectEditor.Settings
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
				ObjDataDirectories.Add(objectDirectory);
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
				DataDirectories.Add(dataDirectory);
			}
		}
		string dataDirectory;

		public HashSet<string> DataDirectories
		{
			get => dataDirectories ??= [];
			set => dataDirectories = value;
		}
		HashSet<string> dataDirectories;

		public string PaletteFile { get; set; } = "palette.png";

		public string IndexFileName { get; set; } = "objectIndex.json";
		public string G1DatFileName { get; set; } = "g1.DAT";
		public string GetObjDataFullPath(string fileName) => Path.Combine(ObjDataDirectory, fileName);
		public string GetDataFullPath(string fileName) => Path.Combine(DataDirectory, fileName);
	}
}
