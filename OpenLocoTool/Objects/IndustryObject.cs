using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum IndustryObjectFlags : uint32_t
	{
		None = 0,
		BuiltInClusters = 1 << 0,
		BuiltOnHighGround = 1 << 1,
		BuiltOnLowGround = 1 << 2,
		BuiltOnSnow = 1 << 3,        // above summer snow line
		BuiltBelowSnowLine = 1 << 4, // below winter snow line
		BuiltOnFlatGround = 1 << 5,
		BuiltNearWater = 1 << 6,
		BuiltAwayFromWater = 1 << 7,
		BuiltOnWater = 1 << 8,
		BuiltNearTown = 1 << 9,
		BuiltAwayFromTown = 1 << 10,
		BuiltNearTrees = 1 << 11,
		BuiltRequiresOpenSpace = 1 << 12,
		Oilfield = 1 << 13,
		Mines = 1 << 14,
		unk15 = 1 << 15,
		CanBeFoundedByPlayer = 1 << 16,
		RequiresAllCargo = 1 << 17,
		unk18 = 1 << 18,
		unk19 = 1 << 19,
		unk20 = 1 << 20,
		HasShadows = 1 << 21,
		unk22 = 1 << 22,
		unk23 = 1 << 23,
		BuiltInDesert = 1 << 24,
		BuiltNearDesert = 1 << 25,
		unk26 = 1 << 26,
		unk27 = 1 << 27,
		unk28 = 1 << 28,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]
	public record BuildingPartAnimation(
		[property: LocoStructProperty(0x00)] uint8_t numFrames,     // Must be a power of 2 (0 = no part animation, could still have animation sequence)
		[property: LocoStructProperty(0x01)] uint8_t animationSpeed // Also encodes in bit 7 if the animation is position modified
		) : ILocoStruct
	{
		public static int StructLength => 0x2;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]
	public record IndustryObjectUnk38(
		[property: LocoStructProperty(0x00)] uint8_t var_00,
		[property: LocoStructProperty(0x01)] uint8_t var_01
		) : ILocoStruct
	{
		public static int StructLength => 0x02;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record IndustryObjectProductionRateRange(
		[property: LocoStructProperty(0x00)] uint16_t min,
		[property: LocoStructProperty(0x02)] uint16_t max
		) : ILocoStruct
	{
		public static int StructLength => 0x04;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xF4)]
	public record IndustryObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] string_id var_02,
		[property: LocoStructProperty(0x04)] string_id NameClosingDown,
		[property: LocoStructProperty(0x06)] string_id NameUpProduction,
		[property: LocoStructProperty(0x08)] string_id NameDownProduction,
		[property: LocoStructProperty(0x0A)] uint16_t NameSingular,
		[property: LocoStructProperty(0x0C)] uint16_t NamePlural,
		[property: LocoStructProperty(0x0E)] uint32_t var_0E, // shadows image id base
		[property: LocoStructProperty(0x12)] uint32_t var_12, // Base image id for building 0
		[property: LocoStructProperty(0x16)] uint32_t var_16,
		[property: LocoStructProperty(0x1A)] uint32_t var_1A,
		[property: LocoStructProperty(0x1E)] uint8_t var_1E,
		[property: LocoStructProperty(0x1F)] uint8_t var_1F,
		//[property: LocoStructProperty(0x20)] const uint8_t* buildingPartHeight,    // This is the height of a building image
		//[property: LocoStructProperty(0x24)] const BuildingPartAnimation* buildingPartAnimations, 
		//[property: LocoStructProperty(0x28)] const uint8_t* animationSequences[4], // Access with getAnimationSequence helper method
		//[property: LocoStructProperty(0x38)] const IndustryObjectUnk38* var_38,    // Access with getUnk38 helper method
		//[property: LocoStructProperty(0x3C)] const uint8_t* buildingParts[32],     // Access with getBuildingParts helper method
		[property: LocoStructProperty(0xBC)] uint8_t MinNumBuildings,
		[property: LocoStructProperty(0xBD)] uint8_t MaxNumBuildings,
		//[property: LocoStructProperty(0xBE)] const uint8_t* buildings,
		[property: LocoStructProperty(0xC2)] uint32_t AvailableColours,  // bitset
		[property: LocoStructProperty(0xC6)] uint32_t BuildingSizeFlags, // flags indicating the building types size 1:large4x4, 0:small1x1
		[property: LocoStructProperty(0xCA)] uint16_t DesignedYear,
		[property: LocoStructProperty(0xCC)] uint16_t ObsoleteYear,
		[property: LocoStructProperty(0xCE)] uint8_t TotalOfTypeInScenario, // Total industries of this type that can be created in a scenario Note: this is not directly comparabile to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
		[property: LocoStructProperty(0xCF)] uint8_t CostIndex,
		[property: LocoStructProperty(0xD0)] int16_t CostFactor,
		[property: LocoStructProperty(0xD2)] int16_t ClearCostFactor,
		[property: LocoStructProperty(0xD4)] uint8_t ScaffoldingSegmentType,
		[property: LocoStructProperty(0xD5)] Colour ScaffoldingColour,
		[property: LocoStructProperty(0xD6), LocoArrayLength(2)] IndustryObjectProductionRateRange[] InitialProductionRate,
		[property: LocoStructProperty(0xDE), LocoArrayLength(2)] uint8_t[] ProducedCargoType,                               // (0xFF = null)
		[property: LocoStructProperty(0xE0), LocoArrayLength(3)] uint8_t[] RequiredCargoType,                               // (0xFF = null)
		[property: LocoStructProperty(0xE3)] uint8_t pad_E3,
		[property: LocoStructProperty(0xE4)] IndustryObjectFlags Flags,
		[property: LocoStructProperty(0xE8)] uint8_t var_E8,
		[property: LocoStructProperty(0xE9)] uint8_t var_E9,
		[property: LocoStructProperty(0xEA)] uint8_t var_EA,
		[property: LocoStructProperty(0xEB)] uint8_t var_EB,
		[property: LocoStructProperty(0xEC)] uint8_t var_EC, // Used by Livestock cow shed count??
		[property: LocoStructProperty(0xED), LocoArrayLength(4)] uint8_t[] WallTypes, // There can be up to 4 different wall types for an industry
		[property: LocoStructProperty(0xF1)] uint8_t BuildingWall, // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
		[property: LocoStructProperty(0xF2)] uint8_t BuildingWallEntrance, // An alternative wall type that looks like a gate placed at random places in building perimeter
		[property: LocoStructProperty(0xF3)] uint8_t var_F3
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.industry;
		public static int StructLength => 0xF4;
	}
}
