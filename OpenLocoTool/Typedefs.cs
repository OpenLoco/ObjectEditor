global using string_id = System.UInt16;

global using uint8_t = System.Byte;
global using uint16_t = System.UInt16;
global using int16_t = System.Int16;
global using uint32_t = System.UInt32;

global using Speed16 = System.Int16;
global using SoundObjectId_t = System.Byte;

global using SmallZ = System.Byte;
global using MicroZ = System.Byte;

using System.Drawing;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x28)]
struct Pos2
{
	public int16_t x { get; set; }
	public int16_t y { get; set; }
}
