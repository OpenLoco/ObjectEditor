using System.Text.Json.Serialization;

namespace OpenLoco.Common
{
	public class VersionCheckBody
	{
		[JsonPropertyName("tag_name")]
		public string TagName { get; set; }
	}
}
