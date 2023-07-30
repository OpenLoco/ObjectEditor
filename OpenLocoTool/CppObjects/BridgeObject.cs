using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[Flags]
	enum BuildingObjectFlags : uint8_t
	{
		none = 0,
		largeTile = 1 << 0, // 2x2 tile
		miscBuilding = 1 << 1,
		undestructible = 1 << 2,
		isHeadquarters = 1 << 3,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x2C)]
	public struct BridgeObject
	{
		//static constexpr auto kObjectType = ObjectType::bridge;

		public string_id name { get; set; }
		public uint8_t noRoof { get; set; } // 0x02
		public unsafe fixed uint8_t pad_03[0x06 - 0x03];
		public uint16_t var_06 { get; set; }            // 0x06
		public uint8_t spanLength { get; set; }         // 0x08
		public uint8_t pillarSpacing { get; set; }      // 0x09
		public Speed16 maxSpeed { get; set; }           // 0x0A
		public MicroZ maxHeight { get; set; }    // 0x0C MicroZ!
		public uint8_t costIndex { get; set; }          // 0x0D
		public int16_t baseCostFactor { get; set; }     // 0x0E
		public int16_t heightCostFactor { get; set; }   // 0x10
		public int16_t sellCostFactor { get; set; }     // 0x12
		public uint16_t disabledTrackCfg { get; set; }  // 0x14
		public uint32_t image { get; set; }             // 0x16
		public uint8_t trackNumCompatible { get; set; } // 0x1A
		public unsafe fixed uint8_t trackMods[7];       // 0x1B
		public uint8_t roadNumCompatible { get; set; }  // 0x22
		public unsafe fixed uint8_t roadMods[7];        // 0x23
		public uint16_t designedYear { get; set; }      // 0x2A
	}
}