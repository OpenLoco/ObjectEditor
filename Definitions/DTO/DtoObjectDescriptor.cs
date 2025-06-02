using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectDescriptor(
		int Id,
		string UniqueName,
		string? Description,
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ICollection<TblAuthor> Authors,
		ICollection<TblTag> Tags,
		ICollection<TblLocoObjectPack> ObjectPacks,
		ICollection<DtoDatObjectEntry> LinkedDatObjects,
		TblLicence? Licence);
}
