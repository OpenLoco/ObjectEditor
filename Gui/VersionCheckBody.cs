using System.Text.Json.Serialization;

namespace OpenLoco.ObjectEditor.Gui
{
	public class VersionCheckBody
	{
		[JsonPropertyName("tag_name")]
		public string TagName { get; set; }
	}
}
