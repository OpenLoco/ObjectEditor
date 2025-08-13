using Definitions.Database;
using Definitions.ObjectModels.Types;
using System.Text.Json.Serialization;

namespace Definitions.SourceData;

[method: JsonConstructor]
public record SC5FileJsonRecord(
	string Name,
	string? Description,
	List<string> Authors,
	List<string> Tags,
	string? Licence,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	ObjectSource ObjectSource) : IDbDates;
