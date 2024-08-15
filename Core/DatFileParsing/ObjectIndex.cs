using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	public record ObjectIndex(string Filename, string ObjectName, ObjectType ObjectType, SourceGame SourceGame, uint32_t Checksum, VehicleType? VehicleType = null);
}
