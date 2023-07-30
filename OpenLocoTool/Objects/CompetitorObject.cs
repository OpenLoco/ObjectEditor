using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x38)]
	struct CompetitorObject
	{
		//static constexpr auto kObjectType = ObjectType::competitor;

		string_id var_00;        // 0x00
		string_id var_02;        // 0x02
		uint32_t var_04;         // 0x04
		uint32_t var_08;         // 0x08
		uint32_t emotions;       // 0x0C
		unsafe fixed uint32_t images[9];      // 0x10
		uint8_t intelligence;    // 0x34
		uint8_t aggressiveness;  // 0x35
		uint8_t competitiveness; // 0x36
		uint8_t var_37;          // 0x37
	}
}
