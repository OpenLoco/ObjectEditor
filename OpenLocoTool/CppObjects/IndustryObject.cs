using System.Runtime.InteropServices;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x2)]
	struct BuildingPartAnimation
	{
		public uint8_t numFrames { get; set; }      // 0x0 Must be a power of 2 (0 = no part animation, could still have animation sequence)
		public uint8_t animationSpeed { get; set; } // 0x1 Also encodes in bit 7 if the animation is position modified
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x2)]
	struct IndustryObjectUnk38
	{
		public uint8_t var_00 { get; set; }
		public uint8_t var_01 { get; set; }
	};

	[Flags]
	enum IndustryObjectFlags : uint32_t
	{
		none = 0,
		builtInClusters = 1 << 0,
		builtOnHighGround = 1 << 1,
		builtOnLowGround = 1 << 2,
		builtOnSnow = 1 << 3,        // above summer snow line
		builtBelowSnowLine = 1 << 4, // below winter snow line
		builtOnFlatGround = 1 << 5,
		builtNearWater = 1 << 6,
		builtAwayFromWater = 1 << 7,
		builtOnWater = 1 << 8,
		builtNearTown = 1 << 9,
		builtAwayFromTown = 1 << 10,
		builtNearTrees = 1 << 11,
		builtRequiresOpenSpace = 1 << 12,
		oilfield = 1 << 13,
		mines = 1 << 14,
		canBeFoundedByPlayer = 1 << 16,
		requiresAllCargo = 1 << 17,
		unk18 = 1 << 18,
		unk19 = 1 << 19,
		hasShadows = 1 << 21,
		unk23 = 1 << 23,
		builtInDesert = 1 << 24,
		builtNearDesert = 1 << 25,
		unk27 = 1 << 27,
		flag_28 = 1 << 28,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xF4)]
	struct IndustryObject
	{
		//static constexpr auto kObjectType = ObjectType::industry;

		public string_id name { get; set; }               // 0x0
		public string_id var_02 { get; set; }             // 0x2
		public string_id nameClosingDown { get; set; }    // 0x4
		public string_id nameUpProduction { get; set; }   // 0x6
		public string_id nameDownProduction { get; set; } // 0x8
		public uint16_t nameSingular { get; set; }        // 0x0A
		public uint16_t namePlural { get; set; }          // 0x0C
		public uint32_t var_0E { get; set; }              // 0x0E shadows image id base
		public uint32_t var_12 { get; set; }              // 0x12 Base image id for building 0
		public uint32_t var_16 { get; set; }
		public uint32_t var_1A { get; set; }
		public uint8_t var_1E { get; set; }
		public uint8_t var_1F { get; set; }
		public unsafe uint8_t* buildingPartHeight { get; set; }                   // 0x20 This is the height of a building image
		public unsafe BuildingPartAnimation* buildingPartAnimations { get; set; } // 0x24
		public unsafe fixed int animationSequences[4];                // 0x28 Access with getAnimationSequence helper method
		public unsafe IndustryObjectUnk38* var_38;                   // 0x38 Access with getUnk38 helper method
		public unsafe fixed int buildingParts[32];                    // 0x3C Access with getBuildingParts helper method
		public uint8_t var_BC { get; set; }
		public uint8_t var_BD { get; set; }
		public unsafe uint8_t* var_BE { get; set; }
		public uint32_t var_C2 { get; set; }
		public uint32_t buildingSizeFlags { get; set; } // 0xC6 flags indicating the building types size 1:large4x4, 0:small1x1
		public uint16_t designedYear { get; set; }      // 0xCA start year
		public uint16_t obsoleteYear { get; set; }      // 0xCC end year
														// Total industries of this type that can be created in a scenario
														// Note: this is not directly comparabile to total industries and vaires based
														// on scenario total industries cap settings. At low industries cap this value is ~3x the
														// amount of industries in a scenario.
		public uint8_t totalOfTypeInScenario { get; set; }  // 0xCE
		public uint8_t costIndex { get; set; }              // 0xCF
		public int16_t costFactor { get; set; }             // 0xD0
		public int16_t clearCostFactor { get; set; }        // 0xD2
		public uint8_t scaffoldingSegmentType { get; set; } // 0xD4
		public Colour scaffoldingColour { get; set; }       // 0xD5
		public uint16_t var_D6 { get; set; }
		public unsafe fixed uint8_t pad_D8[0xDA - 0xD8];
		public uint16_t var_DA;
		public unsafe fixed uint8_t pad_DC[0xDE - 0xDC];
		public unsafe fixed uint8_t producedCargoType[2]; // 0xDE (0xFF = null)
		public unsafe fixed uint8_t requiredCargoType[3]; // 0xE0 (0xFF = null)
		public uint8_t pad_E3 { get; set; }
		public IndustryObjectFlags flags { get; set; } // 0xE4
		public uint8_t var_E8 { get; set; }
		public uint8_t var_E9 { get; set; }
		public uint8_t var_EA { get; set; }
		public uint8_t var_EB { get; set; }
		public uint8_t var_EC { get; set; }       // Used by Livestock cow shed count??
		public unsafe fixed uint8_t wallTypes[4]; // 0xED There can be up to 4 different wall types for an industry
												  // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2
		public uint8_t var_F1 { get; set; }
		public uint8_t var_F2 { get; set; }
		public uint8_t var_F3 { get; set; }

		public unsafe BuildingPartAnimation* getAnimationSequences(int idx) => (BuildingPartAnimation*)animationSequences[idx];
		public unsafe void setAnimationSequences(int idx, BuildingPartAnimation* val) => animationSequences[idx] = (int)val;

		public unsafe uint8_t* getBuildingParts(int idx) => (uint8_t*)buildingParts[idx];
		public unsafe void setBildingParts(int idx, uint8_t* val) => buildingParts[idx] = (int)val;
	}
}
