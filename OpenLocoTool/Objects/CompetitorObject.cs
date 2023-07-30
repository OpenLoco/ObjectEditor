using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x38)]
	struct CompetitorObject
	{
		//static constexpr auto kObjectType = ObjectType::competitor;

		public string_id var_00 { get; set; }        // 0x00
		public string_id var_02 { get; set; }        // 0x02
		public uint32_t var_04 { get; set; }         // 0x04
		public uint32_t var_08 { get; set; }         // 0x08
		public uint32_t emotions { get; set; }       // 0x0C
		public unsafe fixed uint32_t images[9];      // 0x10
		public uint8_t intelligence { get; set; }    // 0x34
		public uint8_t aggressiveness { get; set; }  // 0x35
		public uint8_t competitiveness { get; set; } // 0x36
		public uint8_t var_37 { get; set; }          // 0x37
	}
}
