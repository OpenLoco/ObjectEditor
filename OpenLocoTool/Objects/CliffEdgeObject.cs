using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x6)]
	struct CliffEdgeObject
	{
		//static constexpr auto kObjectType = ObjectType::cliffEdge;

		string_id name;
		uint32_t image; // 0x02
	}
}
