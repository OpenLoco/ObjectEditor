using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoToolGui
{
	public record IndexObjectHeader(string Name, ObjectType ObjectType, UInt32 Checksum, VehicleType? VehicleType);
}