using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

internal sealed record ImageTableGroupDefinition(
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("start")] int Start
);

internal sealed record ImageTableGroupConfiguration(
	[property: JsonPropertyName("objectType")] string ObjectType,
	[property: JsonPropertyName("groups")] List<ImageTableGroupDefinition> Groups
);
