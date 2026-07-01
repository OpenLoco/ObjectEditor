using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics.ImageTable;

public sealed record ImageTableGroupDefinitionJson(
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("start")] int Start,
	[property: JsonPropertyName("chunkSize")] int? ChunkSize = null
);

public sealed record ImageTableGroupConfigurationJson(
	[property: JsonPropertyName("version"),] string Version,
	[property: JsonPropertyName("definitions")] Dictionary<string, List<ImageTableGroupDefinitionJson>> Definitions
);
