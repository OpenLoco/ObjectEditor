using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public record BodySprite() : ILocoStruct
	{
		public static int ObjectStructSize => 0x1E;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
