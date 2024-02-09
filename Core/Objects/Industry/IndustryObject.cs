using System.ComponentModel;
using OpenLocoObjectEditor;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace Core.Objects
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
	[LocoStructSize(0xF4)]
	[LocoStructType(ObjectType.Industry)]
	[LocoStringTable("Name", "var_02", "<unused>", "NameClosingDown", "NameUpProduction", "NameDownProduction", "NameSingular", "NamePlural")]
	public record IndustryObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id var_02,
		[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id _unused,
		[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id NameClosingDown,
		[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id NameUpProduction,
		[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id NameDownProduction,
		[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id NameSingular,
		[property: LocoStructOffset(0x0C), LocoString, Browsable(false)] string_id NamePlural,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id var_0E, // shadows image id base
		[property: LocoStructOffset(0x12), Browsable(false)] image_id var_12, // Base image id for building 0
		[property: LocoStructOffset(0x16), Browsable(false)] image_id var_16,
		[property: LocoStructOffset(0x1A), Browsable(false)] image_id var_1A,
		[property: LocoStructOffset(0x1E)] uint8_t NumBuildingAnimations,
		[property: LocoStructOffset(0x1F)] uint8_t NumBuildingVariations,
		[property: LocoStructOffset(0x20), LocoStructVariableLoad] List<uint8_t> BuildingVariationHeights,    // This is the height of a building image
		[property: LocoStructOffset(0x24), LocoStructVariableLoad] List<BuildingPartAnimation> BuildingVariationAnimations,
		[property: LocoStructOffset(0x28), LocoStructVariableLoad, LocoArrayLength(IndustryObject.AnimationSequencesCount)] List<uint8_t[]> AnimationSequences, // Access with getAnimationSequence helper method
		[property: LocoStructOffset(0x38), LocoStructVariableLoad] List<IndustryObjectUnk38> var_38,    // Access with getUnk38 helper method
		[property: LocoStructOffset(0x3C), LocoStructVariableLoad, LocoArrayLength(IndustryObject.VariationPartCount)] List<uint8_t[]> BuildingVariationParts,  // Access with getBuildingParts helper method
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
		[property: LocoStructOffset(0xDE), LocoStructVariableLoad, LocoArrayLength(IndustryObject.MaxProducedCargoType)] List<S5Header> ProducedCargo,   // (0xFF = null)
		[property: LocoStructOffset(0xE0), LocoStructVariableLoad, LocoArrayLength(IndustryObject.MaxRequiredCargoType)] List<S5Header> RequiredCargo, // (0xFF = null)
		[property: LocoStructOffset(0xE3), Browsable(false)] uint8_t pad_E3,
		[property: LocoStructOffset(0xE4)] IndustryObjectFlags Flags,
		[property: LocoStructOffset(0xE8)] uint8_t var_E8,
		[property: LocoStructOffset(0xE9)] uint8_t var_E9,
		[property: LocoStructOffset(0xEA)] uint8_t var_EA,
		[property: LocoStructOffset(0xEB)] uint8_t var_EB,
		[property: LocoStructOffset(0xEC)] uint8_t var_EC,
		[property: LocoStructOffset(0xED), LocoStructVariableLoad, LocoArrayLength(IndustryObject.WallTypeCount)] List<S5Header> WallTypes, // There can be up to 4 different wall types for an industry
		[property: LocoStructOffset(0xF1), LocoStructVariableLoad] object_id _BuildingWall, // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
		[property: LocoStructOffset(0xF2), LocoStructVariableLoad] object_id _BuildingWallEntrance, // An alternative wall type that looks like a gate placed at random places in building perimeter
		[property: LocoStructOffset(0xF3)] uint8_t var_F3
		) : ILocoStruct, ILocoStructVariableData
	{
		public const int AnimationSequencesCount = 4;
		public const int InitialProductionRateCount = 2;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 3;
		public const int VariationPartCount = 32;
		public const int WallTypeCount = 4;

		//public List<IndustryObjectUnk38> UnkIndustry38 { get; set; } = [];

		public S5Header BuildingWall { get; set; }

		public S5Header BuildingWallEntrance { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// variation heights
			BuildingVariationHeights.Clear();
			BuildingVariationHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumBuildingAnimations * 1)], NumBuildingAnimations));
			remainingData = remainingData[(NumBuildingAnimations * 1)..]; // uint8_t*

			// variation animations
			BuildingVariationAnimations.Clear();
			var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
			BuildingVariationAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumBuildingAnimations * buildingAnimationSize)], typeof(BuildingPartAnimation), NumBuildingAnimations, buildingAnimationSize)
				.Cast<BuildingPartAnimation>());
			remainingData = remainingData[(NumBuildingAnimations * 2)..]; // uint16_t*

			// animation sequences
			AnimationSequences.Clear();
			for (var i = 0; i < AnimationSequencesCount; ++i)
			{
				var size = remainingData[0];
				byte[] arr = [];
				if (size != 0)
				{
					arr = remainingData[1..size].ToArray();
				}

				AnimationSequences.Add(arr);
				remainingData = remainingData[(size + 1)..];
			}

			// unk animation related
			var_38.Clear();
			var structSize = ObjectAttributes.StructSize<IndustryObjectUnk38>();
			while (remainingData[0] != 0xFF)
			{
				var_38.Add(ByteReader.ReadLocoStruct<IndustryObjectUnk38>(remainingData[..structSize]));
				remainingData = remainingData[structSize..];
			}

			remainingData = remainingData[1..]; // skip final 0xFF byte

			// variation parts
			BuildingVariationParts.Clear();
			for (var i = 0; i < NumBuildingVariations; ++i)
			{
				var ptr_1F = 0;
				while (remainingData[++ptr_1F] != 0xFF)
				{
					;
				}

				BuildingVariationParts.Add(remainingData[..ptr_1F].ToArray());
				ptr_1F++;
				remainingData = remainingData[ptr_1F..];
			}

			// unk building data
			Buildings.Clear();
			Buildings.AddRange(remainingData[..MaxNumBuildings].ToArray());
			remainingData = remainingData[MaxNumBuildings..];

			// produced cargo
			ProducedCargo.Clear();
			ProducedCargo.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxProducedCargoType));
			remainingData = remainingData[(S5Header.StructLength * MaxProducedCargoType)..];

			// required cargo
			RequiredCargo.Clear();
			RequiredCargo.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxRequiredCargoType));
			remainingData = remainingData[(S5Header.StructLength * MaxRequiredCargoType)..];

			// wall types
			WallTypes.Clear();
			WallTypes.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, WallTypeCount));
			remainingData = remainingData[(S5Header.StructLength * WallTypeCount)..];

			// wall type
			BuildingWall = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];

			// wall type entrance
			BuildingWallEntrance = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			using (var ms = new MemoryStream())
			{
				// variation heights
				foreach (var x in BuildingVariationHeights)
				{
					ms.WriteByte(x);
				}

				// variation animations
				foreach (var x in BuildingVariationAnimations)
				{
					ms.WriteByte(x.NumFrames);
					ms.WriteByte(x.AnimationSpeed);
				}

				// animation sequences
				foreach (var x in AnimationSequences)
				{
					ms.WriteByte((uint8_t)x.Length);
					ms.Write(x);
				}

				// unk animation related
				foreach (var x in var_38)
				{
					ms.WriteByte(x.var_00);
					ms.WriteByte(x.var_01);
				}

				ms.WriteByte(0xFF);

				// variation parts
				foreach (var x in BuildingVariationParts)
				{
					ms.Write(x);
					ms.WriteByte(0xFF);
				}

				// unk building data
				ms.Write(Buildings.ToArray());

				// for the next 3 fields, loco industry objects print zeroes for all these headers if they're unused! insane!

				// produced cargo
				foreach (var obj in ProducedCargo.Fill(MaxProducedCargoType, S5Header.NullHeader))
				{
					ms.Write(obj.Write());
				}

				// required cargo
				foreach (var obj in RequiredCargo.Fill(MaxRequiredCargoType, S5Header.NullHeader))
				{
					ms.Write(obj.Write());
				}

				// wall types
				foreach (var obj in WallTypes.Fill(WallTypeCount, S5Header.NullHeader))
				{
					ms.Write(obj.Write());
				}

				// wall type
				ms.Write(BuildingWall.Write());

				// wall type entrance
				ms.Write(BuildingWallEntrance.Write());

				return ms.ToArray();
			}
		}
	}
}
