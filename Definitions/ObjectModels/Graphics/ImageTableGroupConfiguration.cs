using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

internal sealed record ImageTableGroupDefinition(
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("chunkSize")] int? ChunkSize = null
);

internal sealed record ImageTableGroupConfigurationType(
	[property: JsonPropertyName("objectType")] string ObjectType,
	[property: JsonPropertyName("groups")] List<ImageTableGroupDefinition> Groups
);

internal sealed record ImageTableGroupConfiguration(
	[property: JsonPropertyName("version"),] string Version,
	[property: JsonPropertyName("definitions")] List<ImageTableGroupConfigurationType> Definitions
);
