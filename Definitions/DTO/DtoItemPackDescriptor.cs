using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoItemPackDescriptor<T>(
		int Id,
		string Name,
		string? Description,
		ICollection<T> Items,
		ICollection<TblAuthor> Authors,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ICollection<TblTag> Tags,
		TblLicence? Licence);
}
