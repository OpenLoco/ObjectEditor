
using System.Text.Json.Serialization;

namespace OpenLocoToolGui
{
	public class GuiSettings
	{
		public string ObjDataDirectory
		{
			get => objectDirectory;
			set
			{
				objectDirectory = value;
				ObjDataDirectories ??= new();
				ObjDataDirectories.Add(objectDirectory);
			}
		}
		string objectDirectory;

		public HashSet<string> ObjDataDirectories { get; set; }

		public string DataDirectory
		{
			get => dataDirectory;
			set
			{
				dataDirectory = value;
				DataDirectories ??= new();
				DataDirectories.Add(dataDirectory);
			}
		}
		string dataDirectory;

		public HashSet<string> DataDirectories { get; set; }

		public string PaletteFile { get; set; } = "palette.png";

		public string IndexFileName { get; set; } = "objectIndex.json";

		public string G1DatFileName { get; set; } = "g1.DAT";

		[JsonIgnore]
		public string IndexFilePath => Path.Combine(ObjDataDirectory, IndexFileName);

		[JsonIgnore]
		public string G1Path => Path.Combine(DataDirectory, G1DatFileName);
	}
}
