using Definitions.Database;

namespace Definitions.DTO
{
	public record DtoItemPackDescriptor<T>(
		UniqueObjectId Id,
		string Name,
		string? Description,
		DateOnly? CreatedDate,
		DateOnly? ModifiedDate,
		DateOnly UploadedDate,
		ICollection<T> Items,
		ICollection<DtoAuthorEntry> Authors,
		ICollection<DtoTagEntry> Tags,
		DtoLicenceEntry? Licence) : IHasId, IDbDates;
}
