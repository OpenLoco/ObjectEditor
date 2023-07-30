using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	enum DockObjectFlags : uint16_t
	{
		none = 0,
		unk01 = 1 << 0,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x28)]
	struct DockObject
	{
		//static constexpr auto kObjectType = ObjectType::dock;

		public string_id name;
		public int16_t buildCostFactor; // 0x02
		public int16_t sellCostFactor;  // 0x04
		public uint8_t costIndex;       // 0x06
		public uint8_t var_07;
		public uint32_t image; // 0x08
		public uint32_t var_0C;
		public DockObjectFlags flags; // 0x10
		public uint8_t numAux01;      // 0x12
		public uint8_t numAux02Ent;   // 0x13 must be 1 or 0
		public unsafe uint8_t* var_14;
		public unsafe uint16_t* var_18;
		public unsafe fixed int var_1C[1]; // odd that this is size 1 but that is how its used
		public uint16_t designedYear;    // 0x20
		public uint16_t obsoleteYear;    // 0x22
		public Pos2 boatPosition; // 0x24

		public unsafe uint8_t* getVar1C(int idx) => (uint8_t*)var_1C[idx];
		public unsafe void setVar1C(int idx, uint8_t* val) => var_1C[idx] = (int)val;
	}
}
