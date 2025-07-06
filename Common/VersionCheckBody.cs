using System.Text.Json.Serialization;

namespace OpenLoco.Common
{
	public class VersionCheckBody
	{
		[JsonPropertyName("tag_name")]
		public required string TagName { get; set; }
	}
}
