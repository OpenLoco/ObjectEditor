
using System.Text.Json.Serialization;

namespace OpenLocoToolGui
{
	public class GuiSettings
	{
		public string ObjDataDirectory { get; set; }

		public string DataDirectory { get; set; }

		public string PaletteFile { get; set; } = "palette.png";

		public string IndexFileName { get; set; } = "objectIndex.json";

		[JsonIgnore]
		public string IndexFilePath => Path.Combine(ObjDataDirectory, IndexFileName);
	}
}
