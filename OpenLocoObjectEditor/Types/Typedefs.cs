global using uint8_t = System.Byte;
global using int8_t = System.SByte;
global using uint16_t = System.UInt16;
global using int16_t = System.Int16;
global using int32_t = System.Int32;
global using uint32_t = System.UInt32;
global using string_id = System.UInt16;
global using image_id = System.UInt32;
global using object_id = System.Byte;
global using Speed16 = System.Int16;
global using Speed32 = System.Int32;
global using MicroZ = System.Byte;
global using SoundObjectId = System.Byte;
using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record Pos2(
		[property: LocoStructOffset(0x00)] int16_t X,
		[property: LocoStructOffset(0x02)] int16_t Y
		) : ILocoStruct;
}
