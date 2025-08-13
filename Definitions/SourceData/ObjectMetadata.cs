using Definitions.Database;
using Definitions.ObjectModels.Types;

namespace Definitions.SourceData;

public record ObjectMetadata(
	string InternalName,
	string? Description,
	List<string> Authors,
	List<string> Tags,
	List<string> ObjectPacks,
	string? Licence,
	ObjectAvailability Availability,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	ObjectSource ObjectSource) : IDbDates;
