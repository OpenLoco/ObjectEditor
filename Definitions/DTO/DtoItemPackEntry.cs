using Definitions;

namespace OpenLoco.Definitions.DTO
{
	public record DtoItemPackEntry(
		int Id,
		string Name,
		string? Description,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate,
		DtoLicenceEntry? Licence) : IHasId;
}
