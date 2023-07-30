using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xE)]
	struct HillShapesObject
	{
		//static constexpr auto kObjectType = ObjectType::hillShapes;

		public string_id name { get; set; }
		public uint8_t hillHeightMapCount { get; set; }     // 0x02
		public uint8_t mountainHeightMapCount { get; set; } // 0x03
		public uint32_t image { get; set; }                 // 0x04
		public uint32_t var_08 { get; set; }                // 0x08
		public unsafe fixed uint8_t pad_0C[0x0E - 0x0C];
	}
}
