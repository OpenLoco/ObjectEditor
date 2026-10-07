using Definitions.Database;
using System.Text.Json.Serialization;

namespace Definitions.SourceData;

[method: JsonConstructor]
public record ScenarioPackJsonRecord(
	string Name,
	string? Description,
	List<string> Authors,
	List<string> Tags,
	string? Licence,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate) : IDbDates;
