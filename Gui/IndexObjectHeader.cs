using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;

namespace OpenLoco.ObjectEditor.Gui
{
	public record IndexObjectHeader(string Name, ObjectType ObjectType, UInt32 Checksum, VehicleType? VehicleType);
}