using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public record VehicleObject() : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.vehicle;
		public static int ObjectStructSize => 0x15E;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
