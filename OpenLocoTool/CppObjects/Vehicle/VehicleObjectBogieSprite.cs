using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x12)]
	public struct VehicleObjectBogieSprite
	{
		uint8_t rollStates;      // 0x0 valid values 1, 2, 4 related to bogie->var_46 (identical in value to numRollSprites)
		BogieSpriteFlags flags;  // 0x1 BogieSpriteFlags
		uint8_t width;           // 0x2 sprite width
		uint8_t heightNegative;  // 0x3 sprite height negative
		uint8_t heightPositive;  // 0x4 sprite height positive
		uint8_t numRollSprites;  // 0x5
		uint32_t flatImageIds;   // 0x6 flat sprites
		uint32_t gentleImageIds; // 0xA gentle sprites
		uint32_t steepImageIds;  // 0xE steep sprites
	};
}
