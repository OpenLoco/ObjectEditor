global using uint8_t = System.Byte;
global using int8_t = System.SByte;
global using uint16_t = System.UInt16;
global using int16_t = System.Int16;
global using int32_t = System.Int32;
global using uint32_t = System.UInt32;
global using string_id = System.UInt16;
global using Speed16 = System.Int16;
global using MicroZ = System.Byte;
global using SoundObjectId = System.Byte;
using OpenLocoTool.DatFileParsing;
using System.ComponentModel;

[TypeConverter(typeof(ExpandableObjectConverter))]
public record struct Pos2(int16_t X, int16_t y) : ILocoStruct
{
	public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
	public ReadOnlySpan<byte> Write() => throw new NotImplementedException();

	public static int ObjectStructSize => 0x04;
}
