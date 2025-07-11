using Dat.Data;
using Definitions.Database;

namespace Definitions.SourceData
{
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
}
