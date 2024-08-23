using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Schema.Database;

namespace OpenLoco.Schema.Server
{
	public record TblLocoObjectDTO(
		int TblLocoObjectId,
		string Name,
		string OriginalName,
		uint OriginalChecksum,
		byte[]? OriginalBytes,
		SourceGame SourceGame,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string? Description,
		TblAuthor? Author,
		DateTime? CreationDate,
		DateTime? LastEditDate,
		ICollection<TblTag> Tags,
		ICollection<TblModpack> Modpacks,
		ObjectAvailability Availability,
		TblLicence? Licence);
}
