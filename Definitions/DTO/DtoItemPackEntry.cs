using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoItemPackEntry(
		int Id,
		string Name,
		string? Description,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		TblLicence? Licence) : IHasId;
}
