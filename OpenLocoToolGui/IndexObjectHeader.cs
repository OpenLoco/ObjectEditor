using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoToolGui
{
	public record IndexObjectHeader(string Name, ObjectType ObjectType, VehicleType? VehicleType);
}