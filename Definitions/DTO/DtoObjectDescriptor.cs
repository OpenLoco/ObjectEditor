using Definitions;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectDescriptor(
		int Id,
		string InternalName,
		string? Description,
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		DateTimeOffset UploadedDate,
		ICollection<DtoAuthorEntry> Authors,
		ICollection<DtoTagEntry> Tags,
		ICollection<DtoItemPackEntry> ObjectPacks,
		ICollection<DtoDatObjectEntry> DatObjects,
		DtoLicenceEntry? Licence) : IHasId;
}
