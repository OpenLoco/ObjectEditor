using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;
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

	public static ObjectType ObjectType => ObjectType.Industry;
	public static DatObjectType DatObjectType => DatObjectType.Industry;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new IndustryObject();

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
				model.InitialProductionRate.Add(new() { Min = br.ReadUInt16(), Max = br.ReadUInt16() });
			}

			br.SkipByte(Constants.MaxProducedCargoType); // ProducedCargo, not part of object definition
			br.SkipByte(Constants.MaxRequiredCargoType); // RequiredCargo, not part of object definition
			model.MapColour = (Colour)br.ReadByte();
			model.Flags = ((DatIndustryObjectFlags)br.ReadUInt32()).Convert();
			model.var_E8 = br.ReadByte(); // Unused, but must be 0 or 1
			model.FarmTileNumImageAngles = br.ReadByte(); // How many viewing angles the farm tiles have
			model.FarmGrowthStageWithNoProduction = br.ReadByte(); // At this stage of growth (except 0), a field tile produces nothing
			model.FarmNumFields = br.ReadByte(); // Max production is reached at farmIdealSize * 25 tiles
			model.FarmNumStagesOfGrowth = br.ReadByte(); // How many growth stages there are sprites for
			br.SkipPointer(); // WallTypes, not part of object definition
			br.SkipObjectId(); // _BuildingWall, not part of object definition
			br.SkipObjectId(); // _BuildingWallEntrance, not part of object definition
			model.MonthlyClosureChance = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, IndustryObject model, byte numBuildingParts, byte numBuildingVariations)
	{
		model.BuildingComponents.BuildingHeights = br.ReadBuildingHeights(numBuildingParts);
		model.BuildingComponents.BuildingAnimations = br.ReadBuildingAnimations(numBuildingParts);

		// animation sequences
		for (var i = 0; i < Constants.AnimationSequencesCount; ++i)
		{
			var size = br.ReadByte();
			var seq = br.ReadBytes(size);
			model.AnimationSequences.Add([.. seq]);
		}

		// unk
		while (br.PeekByte() != LocoConstants.Terminator)
		{
			model.var_38.Add(new() { var_00 = br.ReadByte(), var_01 = br.ReadByte() });
		}

		br.SkipTerminator();

		model.BuildingComponents.BuildingVariations = br.ReadBuildingVariations(numBuildingVariations);
		model.Buildings = [.. br.ReadBytes(model.MaxNumBuildings)];
		model.ProducedCargo = br.ReadS5HeaderList(Constants.MaxProducedCargoType);
		model.RequiredCargo = br.ReadS5HeaderList(Constants.MaxRequiredCargoType);
		model.WallTypes = br.ReadS5HeaderList(Constants.MaxWallTypeCount);
		model.BuildingWall = br.ReadS5Header();
		model.BuildingWallEntrance = br.ReadS5Header();
	}

	public static void Save(Stream stream, LocoObject obj)
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
			bw.Write((uint8_t)model.BuildingComponents.BuildingHeights.Count);
			bw.Write((uint8_t)model.BuildingComponents.BuildingVariations.Count);
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
			bw.Write(model.FarmNumFields); // Max production is reached at farmIdealSize * 25 tiles
			bw.Write(model.FarmNumStagesOfGrowth); // How many growth stages there are sprites for
			bw.WriteEmptyPointer(); // WallTypes, not part of object definition
			bw.WriteEmptyObjectId(); // _BuildingWall, not part of object definition
			bw.WriteEmptyObjectId(); // _BuildingWallEntrance, not part of object definition
			bw.Write(model.MonthlyClosureChance);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(IndustryObject model, LocoBinaryWriter bw)
	{
		bw.Write(model.BuildingComponents.BuildingHeights);
		bw.Write(model.BuildingComponents.BuildingAnimations);

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

		bw.Write(model.BuildingComponents.BuildingVariations);
		bw.Write((ReadOnlySpan<byte>)[.. model.Buildings]);
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
