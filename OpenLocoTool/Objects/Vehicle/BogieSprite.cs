using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public record BogieSprite() : ILocoStruct
	{
		public static int ObjectStructSize => 0x12;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
