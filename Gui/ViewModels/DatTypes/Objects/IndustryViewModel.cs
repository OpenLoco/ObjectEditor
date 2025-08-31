using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class IndustryViewModel : LocoObjectViewModel<IndustryObject>
{
	public uint8_t TotalOfTypeInScenario { get; set; } // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	[EnumProhibitValues<IndustryObjectFlags>(IndustryObjectFlags.None)] public IndustryObjectFlags Flags { get; set; }
	public Colour MapColour { get; set; }
	public uint32_t Colours { get; set; } // bitset
	[Category("Production")] public BindingList<IndustryObjectProductionRateRange> InitialProductionRate { get; set; }
	[Category("Production"), Length(0, IndustryObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeaderViewModel> ProducedCargo { get; set; }
	[Category("Production"), Length(0, IndustryObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeaderViewModel> RequiredCargo { get; set; }
	[Category("Production")] public uint8_t MonthlyClosureChance { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Building"), Length(IndustryObjectLoader.Constants.AnimationSequencesCount, IndustryObjectLoader.Constants.AnimationSequencesCount)] public List<List<uint8_t>> AnimationSequences { get; set; }
	[Category("Building"), Length(1, IndustryObjectLoader.Constants.BuildingVariationCount)] public List<List<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Category("Building"), Length(1, IndustryObjectLoader.Constants.BuildingHeightCount)] public List<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Category("Building"), Length(1, IndustryObjectLoader.Constants.BuildingAnimationCount)] public List<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Category("Building")] public uint8_t MinNumBuildings { get; set; }
	[Category("Building")] public uint8_t MaxNumBuildings { get; set; }
	[Category("Building")] public BindingList<uint8_t> UnkBuildingData { get; set; }
	[Category("Building")] public uint32_t BuildingSizeFlags { get; set; }
	[Category("Building")] public uint8_t ScaffoldingSegmentType { get; set; }
	[Category("Building")] public Colour ScaffoldingColour { get; set; }
	[Category("Building"), Length(0, IndustryObjectLoader.Constants.MaxWallTypeCount)] public List<ObjectModelHeaderViewModel> WallTypes { get; set; }
	[Category("Building")] public ObjectModelHeaderViewModel? BuildingWall { get; set; }
	[Category("Building")] public ObjectModelHeaderViewModel? BuildingWallEntrance { get; set; }
	[Category("<unknown>")] public List<IndustryObjectUnk38> var_38 { get; set; }
	[Category("<unknown>")] public uint8_t var_E8 { get; set; }
	[Category("Farm")] public uint8_t FarmTileNumImageAngles { get; set; }
	[Category("Farm")] public uint8_t FarmGrowthStageWithNoProduction { get; set; }
	[Category("Farm")] public uint8_t FarmIdealSize { get; set; }
	[Category("Farm")] public uint8_t FarmNumStagesOfGrowth { get; set; }

	public IndustryViewModel(IndustryObject io)
	{
		AnimationSequences = [.. io.AnimationSequences.Select(x => new List<uint8_t>(x))];
		BuildingAnimations = [.. io.BuildingComponents.BuildingAnimations];
		BuildingHeights = [.. io.BuildingComponents.BuildingHeights];
		BuildingVariations = [.. io.BuildingComponents.BuildingVariations.Select(x => new List<uint8_t>(x))];
		UnkBuildingData = new(io.UnkBuildingData);
		BuildingSizeFlags = io.BuildingSizeFlags;
		BuildingWall = io.BuildingWall == null ? null : new(io.BuildingWall);
		BuildingWallEntrance = io.BuildingWallEntrance == null ? null : new(io.BuildingWallEntrance);
		MinNumBuildings = io.MinNumBuildings;
		MaxNumBuildings = io.MaxNumBuildings;
		InitialProductionRate = new(io.InitialProductionRate);
		ProducedCargo = new(io.ProducedCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		RequiredCargo = new(io.RequiredCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		WallTypes = [.. io.WallTypes.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		Colours = io.Colours;
		DesignedYear = io.DesignedYear;
		ObsoleteYear = io.ObsoleteYear;
		TotalOfTypeInScenario = io.TotalOfTypeInScenario;
		CostIndex = io.CostIndex;
		BuildCostFactor = io.BuildCostFactor;
		SellCostFactor = io.SellCostFactor;
		ScaffoldingSegmentType = io.ScaffoldingSegmentType;
		ScaffoldingColour = io.ScaffoldingColour;
		MapColour = io.MapColour;
		Flags = io.Flags;
		FarmTileNumImageAngles = io.FarmTileNumImageAngles;
		FarmGrowthStageWithNoProduction = io.FarmGrowthStageWithNoProduction;
		FarmIdealSize = io.FarmIdealSize;
		FarmNumStagesOfGrowth = io.FarmNumStagesOfGrowth;
		MonthlyClosureChance = io.MonthlyClosureChance;
		var_E8 = io.var_E8;
		var_38 = [.. io.var_38];
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override IndustryObject GetAsModel()
		=> new()
		{
			AnimationSequences = AnimationSequences.ToList().ConvertAll(x => x.ToList()),
			BuildingComponents = new BuildingComponents()
			{
				BuildingHeights = [.. BuildingHeights],
				BuildingAnimations = [.. BuildingAnimations],
				BuildingVariations = BuildingVariations.ToList().ConvertAll(x => x.ToList()),
			},
			UnkBuildingData = [.. UnkBuildingData],
			BuildingSizeFlags = BuildingSizeFlags,
			BuildingWall = BuildingWall?.GetAsModel(),
			BuildingWallEntrance = BuildingWallEntrance?.GetAsModel(),
			MinNumBuildings = MinNumBuildings,
			MaxNumBuildings = MaxNumBuildings,
			InitialProductionRate = [.. InitialProductionRate],
			ProducedCargo = ProducedCargo.ToList().ConvertAll(x => x.GetAsModel()),
			RequiredCargo = RequiredCargo.ToList().ConvertAll(x => x.GetAsModel()),
			WallTypes = WallTypes.ToList().ConvertAll(x => x.GetAsModel()),
			Colours = Colours,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			TotalOfTypeInScenario = TotalOfTypeInScenario,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			ScaffoldingSegmentType = ScaffoldingSegmentType,
			ScaffoldingColour = ScaffoldingColour,
			MapColour = MapColour,
			Flags = Flags,
			FarmTileNumImageAngles = FarmTileNumImageAngles,
			FarmGrowthStageWithNoProduction = FarmGrowthStageWithNoProduction,
			FarmIdealSize = FarmIdealSize,
			FarmNumStagesOfGrowth = FarmNumStagesOfGrowth,
			MonthlyClosureChance = MonthlyClosureChance,
			var_E8 = var_E8,
			var_38 = [.. var_38],
		};
}
