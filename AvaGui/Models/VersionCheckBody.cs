using System.Text.Json.Serialization;

namespace AvaGui.Models
{
	public class VersionCheckBody
	{
		[JsonPropertyName("tag_name")]
		public string TagName { get; set; }
	}
}
