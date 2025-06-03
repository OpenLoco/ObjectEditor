using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoItemPackDescriptor<T>(
		int Id,
		string Name,
		string? Description,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate,
		ICollection<T> Items,
		ICollection<TblAuthor> Authors,
		ICollection<TblTag> Tags,
		TblLicence? Licence);
}
