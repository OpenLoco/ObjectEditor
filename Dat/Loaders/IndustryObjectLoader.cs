using Common;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using static Dat.Loaders.IndustryObjectLoader;

namespace Dat.Loaders;

public abstract class IndustryObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int AnimationSequencesCount = 4;
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;
		public const int InitialProductionRateCount = 2;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 3;
		public const int MaxWallTypeCount = 4;
	}

	public static class StructSizes
	{
		public const int Dat = 0xF4;
		public const int IndustryObjectProductionRateRange = 0x04;
		public const int IndustryObjectUnk38 = 0x02;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new IndustryObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipStringId(); // var_02, not part of object definition
			br.SkipStringId(); // NameClosingDown, not part of object definition
			br.SkipStringId(); // NameUpProduction, not part of object definition
			br.SkipStringId(); // NameDownProduction, not part of object definition
			br.SkipStringId(); // NameSingular, not part of object definition
			br.SkipStringId(); // NamePlural, not part of object definition
			br.SkipImageId(); // BaseShadowImageId, not part of object definition
			br.SkipImageId(); // BaseBuildingImageId, not part of object
			br.SkipImageId(); // BaseFarmImageIds, not part of object definition
			model.FarmImagesPerGrowthStage = br.ReadUInt32();
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			br.SkipPointer(); // BuildingHeights, not part of object definition
			br.SkipPointer(); // BuildingAnimations, not part of object definition
			br.SkipPointer(Constants.AnimationSequencesCount); // AnimationSequences, not part of object definition
			br.SkipPointer(); // var_38, not part of object definition
			br.SkipPointer(Constants.BuildingVariationCount); // BuildingVariations, not part of object definition
			model.MinNumBuildings = br.ReadByte();
			model.MaxNumBuildings = br.ReadByte();
			br.SkipPointer(); // Buildings, not part of object definition
			model.Colours = br.ReadUInt32();  // bitset
			model.BuildingSizeFlags = br.ReadUInt32(); // flags indicating the building types size
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			model.TotalOfTypeInScenario = br.ReadByte(); // Total industries of this type that
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.ScaffoldingSegmentType = br.ReadByte();
			model.ScaffoldingColour = (Colour)br.ReadByte();
			for (var i = 0; i < Constants.InitialProductionRateCount; ++i)
			{
				model.InitialProductionRate.Add(new IndustryObjectProductionRateRange(br.ReadUInt16(), br.ReadUInt16()));
			}
			br.SkipByte(Constants.MaxProducedCargoType); // ProducedCargo, not part of object definition
			br.SkipByte(Constants.MaxRequiredCargoType); // RequiredCargo, not part of object definition
			model.MapColour = (Colour)br.ReadByte();
			model.Flags = ((DatIndustryObjectFlags)br.ReadUInt32()).Convert();
			model.var_E8 = br.ReadByte(); // Unused, but must be 0 or 1
			model.FarmTileNumImageAngles = br.ReadByte(); // How many viewing angles the farm tiles have
			model.FarmGrowthStageWithNoProduction = br.ReadByte(); // At this stage of growth (except 0), a field tile produces nothing
			model.FarmIdealSize = br.ReadByte(); // Max production is reached at farmIdealSize * 25 tiles
			model.FarmNumStagesOfGrowth = br.ReadByte(); // How many growth stages there are sprites for
			br.SkipPointer(); // WallTypes, not part of object definition
			br.SkipObjectId(); // _BuildingWall, not part of object definition
			br.SkipObjectId(); // _BuildingWallEntrance, not part of object definition
			model.MonthlyClosureChance = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Industry), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Industry, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, IndustryObject model, byte numBuildingParts, byte numBuildingVariations)
	{
		model.BuildingHeights = br.ReadBuildingHeights(numBuildingParts);
		model.BuildingAnimations = br.ReadBuildingAnimations(numBuildingParts);

		// animation sequences
		for (var i = 0; i < Constants.AnimationSequencesCount; ++i)
		{
			var size = br.PeekByte();
			byte[] arr = [];
			if (size != 0)
			{
				br.SkipByte(); // skip size byte
				arr = br.ReadBytes(size - 1);
			}

			model.AnimationSequences.Add([.. arr]);
			br.SkipTerminator();
		}

		// unk
		while (br.PeekByte() != LocoConstants.Terminator)
		{
			model.var_38.Add(new IndustryObjectUnk38(br.ReadByte(), br.ReadByte()));
		}
		br.SkipTerminator();

		model.BuildingVariations = br.ReadBuildingVariations(numBuildingVariations);
		model.UnkBuildingData = [.. br.ReadBytes(model.MaxNumBuildings)];
		model.ProducedCargo = br.ReadS5HeaderList(Constants.MaxProducedCargoType);
		model.RequiredCargo = br.ReadS5HeaderList(Constants.MaxRequiredCargoType);
		model.WallTypes = br.ReadS5HeaderList(Constants.MaxWallTypeCount);
		model.BuildingWall = br.ReadS5Header();
		model.BuildingWallEntrance = br.ReadS5Header();
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (IndustryObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyStringId(); // var_02, not part of object definition
			bw.WriteEmptyStringId(); // NameClosingDown, not part of object definition
			bw.WriteEmptyStringId(); // NameUpProduction, not part of object definition
			bw.WriteEmptyStringId(); // NameDownProduction, not part of object definition
			bw.WriteEmptyStringId(); // NameSingular, not part of object definition
			bw.WriteEmptyStringId(); // NamePlural, not part of object definition
			bw.WriteEmptyImageId(); // BaseShadowImageId, not part of object definition
			bw.WriteEmptyImageId(); // BaseBuildingImageId, not part of object definition
			bw.WriteEmptyImageId(); // BaseFarmImageIds, not part of object definition
			bw.Write(model.FarmImagesPerGrowthStage);
			bw.Write((uint8_t)model.BuildingHeights.Count);
			bw.Write((uint8_t)model.BuildingVariations.Count);
			bw.WriteEmptyPointer(); // BuildingHeights, not part of object definition
			bw.WriteEmptyPointer(); // BuildingAnimations, not part of object definition
			bw.WriteEmptyPointer(Constants.AnimationSequencesCount); // AnimationSequences, not part of object definition
			bw.WriteEmptyPointer(); // var_38, not part of object definition
			bw.WriteEmptyPointer(Constants.BuildingVariationCount); // BuildingVariations, not part of object definition
			bw.Write(model.MinNumBuildings);
			bw.Write(model.MaxNumBuildings);
			bw.WriteEmptyPointer(); // Buildings, not part of object definition
			bw.Write(model.Colours);  // bitset
			bw.Write(model.BuildingSizeFlags); // flags indicating the building types size
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.Write(model.TotalOfTypeInScenario); // Total industries of this type that can be created in a scenario
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.ScaffoldingSegmentType);
			bw.Write((uint8_t)model.ScaffoldingColour);
			foreach (var rate in model.InitialProductionRate)
			{
				bw.Write(rate.Min);
				bw.Write(rate.Max);
			}
			bw.WriteEmptyBytes(Constants.MaxProducedCargoType);
			bw.WriteEmptyBytes(Constants.MaxRequiredCargoType);
			bw.Write((uint8_t)model.MapColour);
			bw.Write((uint32_t)model.Flags.Convert());
			bw.Write(model.var_E8); // Unused, but must be 0 or 1
			bw.Write(model.FarmTileNumImageAngles); // How many viewing angles the farm tiles have
			bw.Write(model.FarmGrowthStageWithNoProduction); // At this stage of growth (except 0), a field tile produces nothing
			bw.Write(model.FarmIdealSize); // Max production is reached at farmIdealSize * 25 tiles
			bw.Write(model.FarmNumStagesOfGrowth); // How many growth stages there are sprites for
			bw.WriteEmptyPointer(); // WallTypes, not part of object definition
			bw.WriteEmptyObjectId(); // _BuildingWall, not part of object definition
			bw.WriteEmptyObjectId(); // _BuildingWallEntrance, not part of object definition
			bw.Write(model.MonthlyClosureChance);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(IndustryObject model, LocoBinaryWriter bw)
	{
		bw.WriteBuildingHeights(model.BuildingHeights);
		bw.WriteBuildingAnimations(model.BuildingAnimations);

		// animation sequences
		foreach (var x in model.AnimationSequences)
		{
			bw.Write((uint8_t)x.Count);
			bw.Write(x.ToArray());
		}

		// unk animation related
		foreach (var x in model.var_38)
		{
			bw.Write(x.var_00);
			bw.Write(x.var_01);
		}
		bw.WriteTerminator();

		bw.WriteBuildingVariations(model.BuildingVariations);
		bw.Write([.. model.UnkBuildingData]);
		bw.WriteS5HeaderList(model.ProducedCargo, Constants.MaxProducedCargoType);
		bw.WriteS5HeaderList(model.RequiredCargo, Constants.MaxRequiredCargoType);
		bw.WriteS5HeaderList(model.WallTypes, Constants.MaxWallTypeCount);
		bw.WriteS5Header(model.BuildingWall);
		bw.WriteS5Header(model.BuildingWallEntrance);
	}

	[Flags]
	internal enum DatIndustryObjectFlags : uint32_t
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
		NotRotatable = 1 << 15,
		CanBeFoundedByPlayer = 1 << 16,
		RequiresAllCargo = 1 << 17,
		CanIncreaseProduction = 1 << 18,
		CanDecreaseProduction = 1 << 19,
		RequiresElectricityPylons = 1 << 20,
		HasShadows = 1 << 21,
		unk_22 = 1 << 22,
		unk_23 = 1 << 23,
		BuiltInDesert = 1 << 24,
		BuiltNearDesert = 1 << 25,
		unk_26 = 1 << 26,
		unk_27 = 1 << 27,
		unk_28 = 1 << 28,
	}
}

internal static class IndustryObjectFlagsConverter
{
	public static IndustryObjectFlags Convert(this DatIndustryObjectFlags datIndustryObjectFlags)
		=> (IndustryObjectFlags)datIndustryObjectFlags;

	public static DatIndustryObjectFlags Convert(this IndustryObjectFlags industryObjectFlags)
		=> (DatIndustryObjectFlags)industryObjectFlags;
}

[LocoStructSize(0xF4)]
[LocoStructType(DatObjectType.Industry)]
internal record DatIndustryObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoString, Browsable(false), LocoPropertyMaybeUnused] string_id var_02,
	[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id NameClosingDown,
	[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id NameUpProduction,
	[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id NameDownProduction,
	[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id NameSingular,
	[property: LocoStructOffset(0x0C), LocoString, Browsable(false)] string_id NamePlural,
	[property: LocoStructOffset(0x0E), Browsable(false)] image_id BaseShadowImageId, // shadows image id base
	[property: LocoStructOffset(0x12), Browsable(false)] image_id BaseBuildingImageId, // Base image id for building 0
	[property: LocoStructOffset(0x16), Browsable(false)] image_id BaseFarmImageIds,
	[property: LocoStructOffset(0x1A), Browsable(false)] uint32_t FarmImagesPerGrowthStage,
	[property: LocoStructOffset(0x1E)] uint8_t NumBuildingParts,
	[property: LocoStructOffset(0x1F)] uint8_t NumBuildingVariations,
	[property: LocoStructOffset(0x20), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.BuildingHeightCount)] List<uint8_t> BuildingHeights,    // This is the height of a building image
	[property: LocoStructOffset(0x24), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.BuildingAnimationCount)] List<BuildingPartAnimation> BuildingAnimations,
	[property: LocoStructOffset(0x28), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.AnimationSequencesCount)] List<List<uint8_t>> AnimationSequences, // Access with getAnimationSequence helper method
	[property: LocoStructOffset(0x38), LocoStructVariableLoad] List<IndustryObjectUnk38> var_38,    // Access with getUnk38 helper method
	[property: LocoStructOffset(0x3C), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.BuildingVariationCount)] List<List<uint8_t>> BuildingVariations,  // Access with getBuildingParts helper method
	[property: LocoStructOffset(0xBC)] uint8_t MinNumBuildings,
	[property: LocoStructOffset(0xBD)] uint8_t MaxNumBuildings,
	[property: LocoStructOffset(0xBE), LocoStructVariableLoad] List<uint8_t> Buildings,
	[property: LocoStructOffset(0xC2)] uint32_t Colours,  // bitset
	[property: LocoStructOffset(0xC6)] uint32_t BuildingSizeFlags, // flags indicating the building types size 1:large4x4, 0:small1x1
	[property: LocoStructOffset(0xCA)] uint16_t DesignedYear,
	[property: LocoStructOffset(0xCC)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0xCE)] uint8_t TotalOfTypeInScenario, // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
	[property: LocoStructOffset(0xCF)] uint8_t CostIndex,
	[property: LocoStructOffset(0xD0)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0xD2)] int16_t SellCostFactor,
	[property: LocoStructOffset(0xD4)] uint8_t ScaffoldingSegmentType,
	[property: LocoStructOffset(0xD5)] DatColour ScaffoldingColour,
	[property: LocoStructOffset(0xD6), LocoArrayLength(IndustryObjectLoader.Constants.InitialProductionRateCount)] IndustryObjectProductionRateRange[] InitialProductionRate,
	[property: LocoStructOffset(0xDE), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.MaxProducedCargoType)] List<S5Header> ProducedCargo, // (0xFF = null)
	[property: LocoStructOffset(0xE0), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.MaxRequiredCargoType)] List<S5Header> RequiredCargo, // (0xFF = null)
	[property: LocoStructOffset(0xE3)] DatColour MapColour,
	[property: LocoStructOffset(0xE4)] DatIndustryObjectFlags Flags,
	[property: LocoStructOffset(0xE8)] uint8_t var_E8,
	[property: LocoStructOffset(0xE9)] uint8_t FarmTileNumImageAngles, // How many viewing angles the farm tiles have
	[property: LocoStructOffset(0xEA)] uint8_t FarmGrowthStageWithNoProduction, // At this stage of growth (except 0), a field tile produces nothing
	[property: LocoStructOffset(0xEB)] uint8_t FarmIdealSize, // Max production is reached at farmIdealSize * 25 tiles
	[property: LocoStructOffset(0xEC)] uint8_t FarmNumStagesOfGrowth, // How many growth stages there are sprites for
	[property: LocoStructOffset(0xED), LocoStructVariableLoad, LocoArrayLength(IndustryObjectLoader.Constants.MaxWallTypeCount)] List<S5Header> WallTypes, // There can be up to 4 different wall types for an industry
	[property: LocoStructOffset(0xF1), LocoStructVariableLoad, Browsable(false)] object_id _BuildingWall, // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
	[property: LocoStructOffset(0xF2), LocoStructVariableLoad, Browsable(false)] object_id _BuildingWallEntrance, // An alternative wall type that looks like a gate placed at random places in building perimeter
	[property: LocoStructOffset(0xF3)] uint8_t MonthlyClosureChance
	)
{
	//public List<IndustryObjectUnk38> UnkIndustry38 { get; set; } = [];

	public S5Header? BuildingWall { get; set; }

	public S5Header? BuildingWallEntrance { get; set; }

	public image_id var_0E { get; private set; } // shadows image id base
	public image_id var_12 { get; private set; } // Base image id for building 0
	public image_id var_16 { get; private set; }
	public image_id var_1A { get; private set; }

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// part heights
		BuildingHeights.Clear();
		BuildingHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumBuildingParts * 1)], NumBuildingParts));
		remainingData = remainingData[(NumBuildingParts * 1)..]; // uint8_t*

		// part animations
		BuildingAnimations.Clear();
		var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
		BuildingAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumBuildingParts * buildingAnimationSize)], typeof(BuildingPartAnimation), NumBuildingParts, buildingAnimationSize)
			.Cast<BuildingPartAnimation>());
		remainingData = remainingData[(NumBuildingParts * 2)..]; // uint16_t*

		// animation sequences
		AnimationSequences.Clear();
		for (var i = 0; i < IndustryObjectLoader.Constants.AnimationSequencesCount; ++i)
		{
			var size = remainingData[0];
			byte[] arr = [];
			if (size != 0)
			{
				arr = remainingData[1..size].ToArray();
			}

			AnimationSequences.Add([.. arr]);
			remainingData = remainingData[(size + 1)..];
		}

		// unk animation related
		var_38.Clear();
		var structSize = IndustryObjectLoader.StructSizes.IndustryObjectUnk38;
		while (remainingData[0] != LocoConstants.Terminator)
		{
			var_38.Add(ByteReader.ReadLocoStruct<IndustryObjectUnk38>(remainingData[..structSize]));
			remainingData = remainingData[structSize..];
		}

		remainingData = remainingData[1..]; // skip final 0xFF byte

		// variation parts
		BuildingVariations.Clear();
		for (var i = 0; i < NumBuildingVariations; ++i)
		{
			var ptr_1F = 0;
			while (remainingData[++ptr_1F] != LocoConstants.Terminator)
			{ }

			BuildingVariations.Add(remainingData[..ptr_1F].ToArray().ToList());
			ptr_1F++;
			remainingData = remainingData[ptr_1F..];
		}

		// unk building data
		Buildings.Clear();
		Buildings.AddRange(remainingData[..MaxNumBuildings].ToArray());
		remainingData = remainingData[MaxNumBuildings..];

		// produced cargo
		ProducedCargo.Clear();
		ProducedCargo.AddRange(SawyerStreamReader.ReadS5HeaderList(remainingData, IndustryObjectLoader.Constants.MaxProducedCargoType));
		remainingData = remainingData[(S5Header.StructLength * IndustryObjectLoader.Constants.MaxProducedCargoType)..];

		// required cargo
		RequiredCargo.Clear();
		RequiredCargo.AddRange(SawyerStreamReader.ReadS5HeaderList(remainingData, IndustryObjectLoader.Constants.MaxRequiredCargoType));
		remainingData = remainingData[(S5Header.StructLength * IndustryObjectLoader.Constants.MaxRequiredCargoType)..];

		// wall types
		WallTypes.Clear();
		WallTypes.AddRange(SawyerStreamReader.ReadS5HeaderList(remainingData, IndustryObjectLoader.Constants.MaxWallTypeCount));
		remainingData = remainingData[(S5Header.StructLength * IndustryObjectLoader.Constants.MaxWallTypeCount)..];

		// wall type
		if (remainingData[0] != LocoConstants.Terminator)
		{
			BuildingWall = S5Header.Read(remainingData[..S5Header.StructLength]);
		}

		remainingData = remainingData[S5Header.StructLength..]; // there's always a struct, its just whether its zeroed out or not

		// wall type entrance
		if (remainingData[0] != LocoConstants.Terminator)
		{
			BuildingWallEntrance = S5Header.Read(remainingData[..S5Header.StructLength]);
		}

		remainingData = remainingData[S5Header.StructLength..]; // there's always a struct, its just whether its zeroed out or not

		// image stuff, in openloco it happens after image table load, but only to get image offsets, which we can just set to 0 here
		var_0E = 0;
		var_12 = var_0E;
		if (Flags.HasFlag(DatIndustryObjectFlags.HasShadows))
		{
			var_12 += NumBuildingVariations * 4u;
		}

		var_16 = (NumBuildingParts * 4u) + var_12;
		var_1A = FarmTileNumImageAngles * 21u;

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		using (var ms = new MemoryStream())
		{
			// part heights
			foreach (var x in BuildingHeights)
			{
				ms.WriteByte(x);
			}

			// part animations
			foreach (var x in BuildingAnimations)
			{
				ms.WriteByte(x.NumFrames);
				ms.WriteByte(x.AnimationSpeed);
			}

			// animation sequences
			foreach (var x in AnimationSequences)
			{
				ms.WriteByte((uint8_t)x.Count);
				ms.Write(x.ToArray());
			}

			// unk animation related
			foreach (var x in var_38)
			{
				ms.WriteByte(x.var_00);
				ms.WriteByte(x.var_01);
			}

			ms.WriteByte(LocoConstants.Terminator);

			// variation parts
			foreach (var x in BuildingVariations)
			{
				ms.Write(x.ToArray());
				ms.WriteByte(LocoConstants.Terminator);
			}

			// unk building data
			ms.Write(Buildings.ToArray());

			// for the next 3 fields, loco industry objects print zeroes for all these headers if they're unused! insane!

			// produced cargo
			foreach (var obj in ProducedCargo.Fill(IndustryObjectLoader.Constants.MaxProducedCargoType, S5Header.NullHeader))
			{
				ms.Write(obj!.Write());
			}

			// required cargo
			foreach (var obj in RequiredCargo.Fill(IndustryObjectLoader.Constants.MaxRequiredCargoType, S5Header.NullHeader))
			{
				ms.Write(obj!.Write());
			}

			// wall types
			foreach (var obj in WallTypes.Fill(IndustryObjectLoader.Constants.MaxWallTypeCount, S5Header.NullHeader))
			{
				ms.Write(obj!.Write());
			}

			// wall type
			if (BuildingWall != null)
			{
				ms.Write(BuildingWall.Write());
			}
			else
			{
				ms.Write(S5Header.NullHeader.Write());
			}

			// wall type entrance
			if (BuildingWallEntrance != null)
			{
				ms.Write(BuildingWallEntrance.Write());
			}
			else
			{
				ms.Write(S5Header.NullHeader.Write());
			}

			return ms.ToArray();
		}
	}
}
