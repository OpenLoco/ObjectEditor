using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoItemPackDescriptor<T>(
		DbKey Id,
		string Name,
		string? Description,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate,
		ICollection<T> Items,
		ICollection<DtoAuthorEntry> Authors,
		ICollection<DtoTagEntry> Tags,
		DtoLicenceEntry? Licence) : IHasId;
}
