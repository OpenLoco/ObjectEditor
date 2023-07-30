using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x6)]
	public struct VehicleObjectUnk
	{
		uint8_t length; // 0x00
		uint8_t var_01;
		uint8_t frontBogieSpriteInd; // 0x02 index of bogieSprites struct
		uint8_t backBogieSpriteInd;  // 0x03 index of bogieSprites struct
		uint8_t bodySpriteInd;       // 0x04 index of a bodySprites struct
		uint8_t var_05;
	};
}
