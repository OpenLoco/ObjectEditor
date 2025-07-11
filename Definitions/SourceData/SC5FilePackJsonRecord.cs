using Definitions.Database;
using System.Text.Json.Serialization;

namespace Definitions.SourceData;

[method: JsonConstructor]
public record SC5FilePackJsonRecord(
	string Name,
	string? Description,
	List<string> Authors,
	List<string> Tags,
	string? Licence,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate) : IDbDates;
