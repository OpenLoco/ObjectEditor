using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Schema.Server
{
	public record ObjectIndexEntryDTO(
		string Filename,
		string ObjectName,
		ObjectType ObjectType,
		SourceGame SourceGame,
		uint Checksum,
		VehicleType? VehicleType);
}
