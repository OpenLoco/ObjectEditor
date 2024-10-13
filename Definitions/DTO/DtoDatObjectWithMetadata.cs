using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoDatObjectWithMetadata(
		int Id,
		string UniqueName,
		string DatName,
		uint DatChecksum,
		string? DatBytes, // base64-encoded
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string? Description,
		ICollection<TblAuthor> Authors,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset UploadDate,
		ICollection<TblTag> Tags,
		ICollection<TblModpack> Modpacks,
		ObjectAvailability Availability,
		TblLicence? Licence);
}
