using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record SC5FileJsonRecord(string Name, string? Description, List<string> Authors);
}
