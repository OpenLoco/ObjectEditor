using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Dat.FileParsing
{
	public record ObjectIndex(string Filename, string ObjectName, ObjectType ObjectType, SourceGame SourceGame, uint32_t Checksum, VehicleType? VehicleType = null);
}
