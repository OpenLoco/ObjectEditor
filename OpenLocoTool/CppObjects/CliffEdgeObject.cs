using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x6)]
	struct CliffEdgeObject
	{
		//static constexpr auto kObjectType = ObjectType::cliffEdge;

		public string_id name { get; set; }
		public uint32_t image { get; set; } // 0x02
	}
}
