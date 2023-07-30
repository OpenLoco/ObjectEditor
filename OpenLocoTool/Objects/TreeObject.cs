using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	enum TreeObjectFlags : uint16_t
	{
		none = 0,
		hasSnowVariation = 1 << 0,
		unk1 = 1 << 1,
		veryHighAltitude = 1 << 2,
		highAltitude = 1 << 3,
		requiresWater = 1 << 4,
		unk5 = 1 << 5,
		droughtResistant = 1 << 6,
		hasShadow = 1 << 7,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32 * sizeof(int))] // 32 32-bit pointers pointers

	struct TreeObject
	{
		//static constexpr auto kObjectType = ObjectType::tree;

		public string_id name { get; set; }                  // 0x00
		public uint8_t var_02 { get; set; }                  // 0x02
		public uint8_t height { get; set; }                  // 0x03
		public uint8_t var_04 { get; set; }                  // 0x04
		public uint8_t var_05 { get; set; }                  // 0x05
		public uint8_t numRotations { get; set; }            // 0x06 (1,2,4)
		public uint8_t growth { get; set; }                  // 0x07 (number of tree size images)
		public TreeObjectFlags flags { get; set; }           // 0x08
		public unsafe fixed uint32_t sprites[6];             // 0x0A
		public unsafe fixed uint32_t snowSprites[6];         // 0x22
		public uint16_t shadowImageOffset { get; set; }      // 0x3A
		public uint8_t var_3C { get; set; }                  // 0x3C
		public uint8_t seasonState { get; set; }             // 0x3D (index for sprites, seasons + dying)
		public uint8_t var_3E { get; set; }                  // 0x3E
		public uint8_t costIndex { get; set; }               // 0x3F
		public int16_t buildCostFactor { get; set; }         // 0x40
		public int16_t clearCostFactor { get; set; }         // 0x42
		public uint32_t colours { get; set; }                // 0x44
		public int16_t rating { get; set; }                  // 0x48
		public int16_t demolishRatingReduction { get; set; } // 0x4A
	}
}
