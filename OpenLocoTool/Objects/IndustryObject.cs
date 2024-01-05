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
		[property: LocoStructOffset(0x00)] uint8_t NumFrames,     // Must be a power of 2 (0 = no part animation, could still have animation sequence)
		[property: LocoStructOffset(0x01)] uint8_t AnimationSpeed // Also encodes in bit 7 if the animation is position modified
		) : ILocoStruct;

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]

	public record IndustryObjectUnk38(
		[property: LocoStructOffset(0x00)] uint8_t var_00,
		[property: LocoStructOffset(0x01)] uint8_t var_01
		) : ILocoStruct;

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]

	public record IndustryObjectProductionRateRange(
		[property: LocoStructOffset(0x00)] uint16_t Min,
		[property: LocoStructOffset(0x02)] uint16_t Max
		) : ILocoStruct;

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xF4)]
	[LocoStructType(ObjectType.Industry)]
	[LocoStringTable("Name", "var_02", "<unused>", "NameClosingDown", "NameUpProduction", "NameDownProduction", "NameSingular", "NamePlural")]
	public record IndustryObject(
		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id var_02,
		//[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id _unused,
		//[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id NameClosingDown,
		//[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id NameUpProduction,
		//[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id NameDownProduction,
		//[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id NameSingular,
		//[property: LocoStructOffset(0x0C), LocoString, Browsable(false)] string_id NamePlural,
		//[property: LocoStructOffset(0x0E)] image_id var_0E, // shadows image id base
		//[property: LocoStructOffset(0x12)] image_id var_12, // Base image id for building 0
		//[property: LocoStructOffset(0x16)] image_id var_16,
		//[property: LocoStructOffset(0x1A)] image_id var_1A,
		[property: LocoStructOffset(0x1E)] uint8_t BuildingPartCount1,
		[property: LocoStructOffset(0x1F)] uint8_t BuildingPartCount2,
		[property: LocoStructOffset(0x20), LocoStructVariableLoad] List<uint8_t> BuildingPartHeight,    // This is the height of a building image
		[property: LocoStructOffset(0x24), LocoStructVariableLoad] List<BuildingPartAnimation> BuildingPartAnimations,
		[property: LocoStructOffset(0x28), LocoStructVariableLoad, LocoArrayLength(IndustryObject.AnimationSequencesSize)] List<uint8_t> AnimationSequences, // Access with getAnimationSequence helper method
		[property: LocoStructOffset(0x38), LocoStructVariableLoad] List<IndustryObjectUnk38> var_38,    // Access with getUnk38 helper method
		[property: LocoStructOffset(0x3C), LocoStructVariableLoad, LocoArrayLength(IndustryObject.PartCount)] List<uint8_t[]> Parts,  // Access with getBuildingParts helper method
		[property: LocoStructOffset(0xBC)] uint8_t MinNumBuildings,
		[property: LocoStructOffset(0xBD)] uint8_t MaxNumBuildings,
		[property: LocoStructOffset(0xBE), LocoStructVariableLoad] List<uint8_t> Buildings,
		[property: LocoStructOffset(0xC2)] uint32_t AvailableColours,  // bitset
		[property: LocoStructOffset(0xC6)] uint32_t BuildingSizeFlags, // flags indicating the building types size 1:large4x4, 0:small1x1
		[property: LocoStructOffset(0xCA)] uint16_t DesignedYear,
		[property: LocoStructOffset(0xCC)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0xCE)] uint8_t TotalOfTypeInScenario, // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
		[property: LocoStructOffset(0xCF)] uint8_t CostIndex,
		[property: LocoStructOffset(0xD0)] int16_t CostFactor,
		[property: LocoStructOffset(0xD2)] int16_t ClearCostFactor,
		[property: LocoStructOffset(0xD4)] uint8_t ScaffoldingSegmentType,
		[property: LocoStructOffset(0xD5)] Colour ScaffoldingColour,
		[property: LocoStructOffset(0xD6), LocoArrayLength(IndustryObject.InitialProductionRateCount)] IndustryObjectProductionRateRange[] InitialProductionRate,
		//[property: LocoStructOffset(0xDE), LocoStructVariableLoad, LocoArrayLength(IndustryObject.MaxProducedCargoType)] object_id[] ProducedCargoType,   // (0xFF = null)
		//[property: LocoStructOffset(0xE0), LocoStructVariableLoad, LocoArrayLength(IndustryObject.MaxRequiredCargoType)] object_id[] RequiredCargoType, // (0xFF = null)
		//[property: LocoStructOffset(0xE3)] uint8_t pad_E3,
		[property: LocoStructOffset(0xE4)] IndustryObjectFlags Flags,
		[property: LocoStructOffset(0xE8)] uint8_t var_E8,
		[property: LocoStructOffset(0xE9)] uint8_t var_E9,
		[property: LocoStructOffset(0xEA)] uint8_t var_EA,
		[property: LocoStructOffset(0xEB)] uint8_t var_EB,
		[property: LocoStructOffset(0xEC)] uint8_t var_EC, // Used by Livestock cow shed count??
		[property: LocoStructOffset(0xED), LocoStructVariableLoad, LocoArrayLength(IndustryObject.WallTypeCount)] object_id[] WallTypes, // There can be up to 4 different wall types for an industry
		[property: LocoStructOffset(0xF1), LocoStructVariableLoad] object_id BuildingWall, // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
		[property: LocoStructOffset(0xF2), LocoStructVariableLoad] object_id BuildingWallEntrance, // An alternative wall type that looks like a gate placed at random places in building perimeter
		[property: LocoStructOffset(0xF3)] uint8_t var_F3
		) : ILocoStruct, ILocoStructVariableData
	{
		public const int AnimationSequencesSize = 4;
		public const int InitialProductionRateCount = 2;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 3;
		public const int PartCount = 32;
		public const int WallTypeCount = 4;

		public List<S5Header> ProducedCargo { get; set; } = [];
		public List<S5Header> RequiredCargo { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// part heights
			remainingData = remainingData[(BuildingPartCount1 * 1)..]; // sizeof(uint8_t)

			// part animations
			remainingData = remainingData[(BuildingPartCount1 * ObjectAttributes.StructSize<BuildingPartAnimation>())..]; // sizeof(uint8_t)

			// animation sequences
			for (var i = 0; i < AnimationSequencesSize; ++i)
			{
				var size = (remainingData[0] * 1) + 1;
				remainingData = remainingData[size..];
			}

			// unk animation related
			var ptr_38 = 0;
			while (remainingData[ptr_38] != 0xFF)
			{
				ptr_38 += ObjectAttributes.StructSize<IndustryObjectUnk38>();
			}

			ptr_38++;
			remainingData = remainingData[ptr_38..];

			// parts
			for (var i = 0; i < BuildingPartCount2; ++i)
			{
				var ptr_1F = 0;
				while (remainingData[ptr_1F] != 0xFF)
				{
					ptr_1F++;
				}

				ptr_1F++;
				remainingData = remainingData[ptr_1F..];
			}

			// unk
			remainingData = remainingData[(MaxNumBuildings * 1)..]; // sizeof(uint8_t)

			// produced cargo
			for (var i = 0; i < MaxProducedCargoType; ++i)
			{
				var header = S5Header.Read(remainingData[..S5Header.StructLength]);
				if (header.Checksum != 0 || header.Flags != 255)
				{
					ProducedCargo.Add(header);
				}

				remainingData = remainingData[S5Header.StructLength..];
			}

			// required cargo
			for (var i = 0; i < MaxRequiredCargoType; ++i)
			{
				var header = S5Header.Read(remainingData[..S5Header.StructLength]);
				if (header.Checksum != 0 || header.Flags != 255)
				{
					RequiredCargo.Add(header);
				}

				remainingData = remainingData[S5Header.StructLength..];
			}

			// wall types
			remainingData = remainingData[(S5Header.StructLength * WallTypes.Length)..];

			// unk wall type
			remainingData = remainingData[(S5Header.StructLength * 1)..];

			// unk wall type (building entrance?)
			remainingData = remainingData[(S5Header.StructLength * 1)..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
