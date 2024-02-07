using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Objects;

namespace OpenLocoObjectEditorGui
{
	public record IndexObjectHeader(string Name, ObjectType ObjectType, UInt32 Checksum, VehicleType? VehicleType);
}