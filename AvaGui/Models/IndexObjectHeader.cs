using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System;

namespace AvaGui.Models
{
	public record IndexObjectHeader(string Name, ObjectType ObjectType, UInt32 Checksum, VehicleType? VehicleType);
}