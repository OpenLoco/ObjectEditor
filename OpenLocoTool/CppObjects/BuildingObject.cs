using System.Runtime.InteropServices;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xBE)]
	struct BuildingObject
	{
		//static constexpr auto kObjectType = ObjectType::building;

		public string_id name { get; set; }                     // 0x0
		public uint32_t image { get; set; }                     // 0x2
		public uint8_t var_06 { get; set; }                     // 0x6
		public uint8_t numVariations { get; set; }              // 0x7
		public readonly unsafe uint8_t* varationHeights;     // 0x8
		public readonly unsafe uint16_t* var_0C;             // 0xC
		public unsafe fixed int variationsArr10[32]; // 0x10 //byte*  // 32 32-bit pointers pointers
		public uint32_t colours { get; set; }                   // 0x90
		public uint16_t designedYear { get; set; }              // 0x94
		public uint16_t obsoleteYear { get; set; }              // 0x96
		public BuildingObjectFlags flags { get; set; }          // 0x98
		public uint8_t clearCostIndex { get; set; }             // 0x99
		public uint16_t clearCostFactor { get; set; }           // 0x9A
		public uint8_t scaffoldingSegmentType { get; set; }     // 0x9C
		public Colour scaffoldingColour { get; set; }           // 0x9D
		public unsafe fixed uint8_t pad_9E[0xA0 - 0x9E];
		public unsafe fixed uint8_t producedQuantity[2];    // 0xA0
		public unsafe fixed uint8_t producedCargoType[2];    // 0xA2
		public unsafe fixed uint8_t var_A4[2];               // 0xA4 Some type of Cargo
		public unsafe fixed uint8_t var_A6[2];               // 0xA6
		public unsafe fixed uint8_t var_A8[2];               // 0xA8
		public int16_t demolishRatingReduction { get; set; } // 0XAA
		public uint8_t var_AC { get; set; }                  // 0xAC
		public uint8_t var_AD { get; set; }                  // 0XAD
		public unsafe fixed int var_AE[4];        // 0XAE ->0XB2->0XB6->0XBA->0XBE (4 byte pointers)

		public unsafe uint8_t* getVariation(int idx) => (uint8_t*)variationsArr10[idx];
		public unsafe void setVariation(int idx, uint8_t* val) => variationsArr10[idx] = (int)val;

		public unsafe byte* getVarAE(int idx) => (byte*)var_AE[idx];
		public unsafe void setVarAE(int idx, byte* val) => var_AE[idx] = (int)val;
	}
}
