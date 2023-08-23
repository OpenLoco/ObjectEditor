
using System.Text.Json.Serialization;

namespace OpenLocoToolGui
{
	public class OpenLocoToolGuiSettings
	{
		public string ObjectDirectory { get; set; }

		public string PaletteFile { get; set; } = "palette.png";

		public string IndexFileName { get; set; } = "objectIndex.json";

		[JsonIgnore]
		public string IndexFilePath => Path.Combine(ObjectDirectory, IndexFileName);
	}
}
