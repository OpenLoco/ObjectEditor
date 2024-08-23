using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Schema.Server
{
	public record DtoObjectIndexEntry(
		string Filename,
		string ObjectName,
		ObjectType ObjectType,
		bool IsVanilla,
		uint Checksum,
		VehicleType? VehicleType);
}
