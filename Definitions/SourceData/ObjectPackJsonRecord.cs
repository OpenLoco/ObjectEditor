using OpenLoco.Definitions.Database;
using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.SourceData
{
	[method: JsonConstructor]
	public record ObjectPackJsonRecord(
		string Name,
		string? Description,
		List<string> Authors,
		List<string> Tags,
		string? Licence,
		DateOnly? CreatedDate,
		DateOnly? ModifiedDate,
		DateOnly UploadedDate) : IDbDates;
}
