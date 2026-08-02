using System.Text.Json.Serialization;

namespace Core;

public class VersionCheckBody
{
	[JsonPropertyName("tag_name")]
	public required string TagName { get; set; }
}
