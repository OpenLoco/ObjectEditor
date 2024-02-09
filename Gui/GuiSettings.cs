namespace OpenLocoObjectEditorGui
{
	public class GuiSettings
	{
		public string ObjDataDirectory
		{
			get => objectDirectory;
			set
			{
				objectDirectory = value;
				_ = ObjDataDirectories.Add(objectDirectory);
			}
		}
		string objectDirectory = string.Empty;

		public HashSet<string> ObjDataDirectories { get; } = [];

		public string DataDirectory
		{
			get => dataDirectory;
			set
			{
				dataDirectory = value;
				_ = DataDirectories.Add(dataDirectory);
			}
		}
		string dataDirectory = string.Empty;

		public HashSet<string> DataDirectories { get; } = [];

		public string PaletteFile { get; set; } = "palette.png";

		public string IndexFileName { get; set; } = "objectIndex.json";

		public string G1DatFileName { get; set; } = "g1.DAT";

		public string GetObjDataFullPath(string fileName) => Path.Combine(ObjDataDirectory, fileName);
		public string GetDataFullPath(string fileName) => Path.Combine(DataDirectory, fileName);
	}
}
