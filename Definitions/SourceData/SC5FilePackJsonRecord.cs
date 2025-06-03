using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record SC5FilePackJsonRecord(
		string Name,
		string? Description,
		List<string> Authors,
		List<string> Tags,
		string? Licence,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate);
}
