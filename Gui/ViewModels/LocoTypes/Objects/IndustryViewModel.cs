using Dat.Loaders;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class IndustryViewModel(IndustryObject model)
	: LocoObjectViewModel<IndustryObject>(model)
{
	[Description("Total industries of this type that can be created in a scenario. Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.")]
	public uint8_t TotalOfTypeInScenario
	{
		get => Model.TotalOfTypeInScenario;
		set => Model.TotalOfTypeInScenario = value;
	}

	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	public uint16_t ObsoleteYear
	{
		get => Model.ObsoleteYear;
		set => Model.ObsoleteYear = value;
	}

	[EnumProhibitValues<IndustryObjectFlags>(IndustryObjectFlags.None)]
	public IndustryObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	[Description("Bitset")]
	public Colour MapColour
	{
		get => Model.MapColour;
		set => Model.MapColour = value;
	}

	[Description("Bitset")]
	public uint32_t Colours
	{
		get => Model.Colours;
		set => Model.Colours = value;
	}

	[Category("Production")]
	public BindingList<IndustryObjectProductionRateRange> InitialProductionRate { get; init; } = new(model.InitialProductionRate);

	[Category("Production")]
	[Length(0, IndustryObjectLoader.Constants.MaxProducedCargoType)]
	public BindingList<ObjectModelHeader> ProducedCargo { get; init; } = new(model.ProducedCargo);

	[Category("Production")]
	[Length(0, IndustryObjectLoader.Constants.MaxProducedCargoType)]
	public BindingList<ObjectModelHeader> RequiredCargo { get; init; } = new(model.RequiredCargo);

	[Category("Production")]
	public uint8_t MonthlyClosureChance
	{
		get => Model.MonthlyClosureChance;
		set => Model.MonthlyClosureChance = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Farm")]
	public uint8_t FarmTileNumImageAngles
	{
		get => Model.FarmTileNumImageAngles;
		set => Model.FarmTileNumImageAngles = value;
	}

	[Category("Farm")]
	public uint8_t FarmGrowthStageWithNoProduction
	{
		get => Model.FarmGrowthStageWithNoProduction;
		set => Model.FarmGrowthStageWithNoProduction = value;
	}

	[Category("Farm")]
	public uint8_t FarmIdealSize
	{
		get => Model.FarmNumFields;
		set => Model.FarmNumFields = value;
	}

	[Category("Farm")]
	public uint8_t FarmNumStagesOfGrowth
	{
		get => Model.FarmNumStagesOfGrowth;
		set => Model.FarmNumStagesOfGrowth = value;
	}

	[Category("Building")]
	[Length(IndustryObjectLoader.Constants.AnimationSequencesCount, IndustryObjectLoader.Constants.AnimationSequencesCount)]
	public BindingList<BindingList<uint8_t>> AnimationSequences { get; init; } = model.AnimationSequences.Select(x => x.ToBindingList()).ToBindingList();

	[Category("Building")]
	[Length(1, IndustryObjectLoader.Constants.BuildingVariationCount)]
	public BindingList<BindingList<uint8_t>> BuildingVariations { get; init; } = new(model.BuildingComponents.BuildingVariations.Select(x => x.ToBindingList()).ToBindingList());

	[Category("Building")]
	[Length(1, IndustryObjectLoader.Constants.BuildingHeightCount)]
	public BindingList<uint8_t> BuildingHeights { get; init; } = model.BuildingComponents.BuildingHeights.ToBindingList();

	[Category("Building")]
	[Length(1, IndustryObjectLoader.Constants.BuildingAnimationCount)]
	public BindingList<BuildingPartAnimation> BuildingAnimations { get; init; } = model.BuildingComponents.BuildingAnimations.ToBindingList();

	[Category("Building")]
	public uint8_t MinNumBuildings
	{
		get => Model.MinNumBuildings;
		set => Model.MinNumBuildings = value;
	}

	[Category("Building")]
	public uint8_t MaxNumBuildings
	{
		get => Model.MaxNumBuildings;
		set => Model.MaxNumBuildings = value;
	}

	[Category("Building")]
	public BindingList<uint8_t> UnkBuildingData { get; init; } = model.Buildings.ToBindingList();

	[Category("Building")]
	public uint32_t BuildingSizeFlags
	{
		get => Model.BuildingSizeFlags;
		set => Model.BuildingSizeFlags = value;
	}

	[Category("Building")]
	public uint8_t ScaffoldingSegmentType
	{
		get => Model.ScaffoldingSegmentType;
		set => Model.ScaffoldingSegmentType = value;
	}

	[Category("Building")]
	public Colour ScaffoldingColour
	{
		get => Model.ScaffoldingColour;
		set => Model.ScaffoldingColour = value;
	}

	[Category("Building")]
	[Length(0, IndustryObjectLoader.Constants.MaxWallTypeCount)]
	public BindingList<ObjectModelHeader> WallTypes { get; init; } = model.WallTypes.ToBindingList();

	[Category("Building")]
	public ObjectModelHeader? BuildingWall
	{
		get => Model.BuildingWall;
		set => Model.BuildingWall = value;
	}

	[Category("Building")]
	public ObjectModelHeader? BuildingWallEntrance
	{
		get => Model.BuildingWallEntrance;
		set => Model.BuildingWallEntrance = value;
	}

	[Category("<unknown>")]
	public BindingList<IndustryObjectUnk38> var_38 { get; init; } = model.var_38.ToBindingList();

	[Category("<unknown>")]
	public uint8_t var_E8
	{
		get => Model.var_E8;
		set => Model.var_E8 = value;
	}
}
