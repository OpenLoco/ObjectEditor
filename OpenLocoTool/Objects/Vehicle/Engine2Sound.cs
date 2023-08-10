using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public record Engine2SoundObject() : ILocoStruct
	{
		public static int ObjectStructSize => 0x1B;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
