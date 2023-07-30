using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x3)]
	public struct VehicleObjectSimpleAnimation
	{
		uint8_t objectId;         // 0x00 (object loader fills this in)
		uint8_t height;           // 0x01
		SimpleAnimationType type; // 0x02
	};
}
