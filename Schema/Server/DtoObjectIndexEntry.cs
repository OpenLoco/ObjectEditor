using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Schema.Server
{
	public record DtoObjectIndexEntry(
		int UniqueId,
		string ObjectName,
		ObjectType ObjectType,
		bool IsVanilla,
		uint Checksum,
		VehicleType? VehicleType);
}
