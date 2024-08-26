using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public record DtoObjectIndexEntry(
		int UniqueId,
		string ObjectName,
		ObjectType ObjectType,
		bool IsVanilla,
		uint Checksum,
		VehicleType? VehicleType);
}
