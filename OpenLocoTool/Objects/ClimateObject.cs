using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xA)]
	struct ClimateObject
	{
		// static constexpr auto kObjectType = ObjectType::climate;

		public string_id name { get; set; }          // 0x00
		public uint8_t firstSeason { get; set; }     // 0x02
		public unsafe fixed uint8_t seasonLength[4]; // 0x03
		public uint8_t winterSnowLine { get; set; }  // 0x07
		public uint8_t summerSnowLine { get; set; }  // 0x08
		public uint8_t pad_09 { get; set; }
	};
}
