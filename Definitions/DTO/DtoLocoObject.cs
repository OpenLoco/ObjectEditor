using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public record DtoLocoObject(
		int TblLocoObjectId,
		string Name,
		string OriginalName,
		uint OriginalChecksum,
		string? OriginalBytes, // base64-encoded
		bool IsVanilla,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string? Description,
		ICollection<TblAuthor> Authors,
		DateTimeOffset? CreationDate,
		DateTimeOffset? LastEditDate,
		DateTimeOffset? UploadDate,
		ICollection<TblTag> Tags,
		ICollection<TblModpack> Modpacks,
		ObjectAvailability Availability,
		TblLicence? Licence);
}
