using Dat.Data;
using Dat.Objects;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class IndustryViewModel : LocoObjectViewModel<IndustryObject>
{
	[Reactive] public uint8_t TotalOfTypeInScenario { get; set; } // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
	[Reactive] public uint16_t DesignedYear { get; set; }
	[Reactive] public uint16_t ObsoleteYear { get; set; }
	[Reactive, EnumProhibitValues<IndustryObjectFlags>(IndustryObjectFlags.None)] public IndustryObjectFlags Flags { get; set; }
	[Reactive] public Colour MapColour { get; set; }
	[Reactive] public uint32_t Colours { get; set; } // bitset
	[Reactive, Category("Production")] public BindingList<IndustryObjectProductionRateRange> InitialProductionRate { get; set; }
	[Reactive, Category("Production"), Length(0, IndustryObject.MaxProducedCargoType)] public BindingList<S5HeaderViewModel> ProducedCargo { get; set; }
	[Reactive, Category("Production"), Length(0, IndustryObject.MaxProducedCargoType)] public BindingList<S5HeaderViewModel> RequiredCargo { get; set; }
	[Reactive, Category("Production")] public uint8_t MonthlyClosureChance { get; set; }
	[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
	[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Reactive, Category("Building"), Length(IndustryObject.AnimationSequencesCount, IndustryObject.AnimationSequencesCount)] public BindingList<BindingList<uint8_t>> AnimationSequences { get; set; }
	[Reactive, Category("Building"), Length(1, IndustryObject.BuildingVariationCount)] public BindingList<BindingList<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Reactive, Category("Building"), Length(1, IndustryObject.BuildingHeightCount)] public BindingList<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Reactive, Category("Building"), Length(1, IndustryObject.BuildingAnimationCount)] public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Reactive, Category("Building")] public uint8_t MinNumBuildings { get; set; }
	[Reactive, Category("Building")] public uint8_t MaxNumBuildings { get; set; }
	[Reactive, Category("Building")] public BindingList<uint8_t> Buildings { get; set; }
	[Reactive, Category("Building")] public uint32_t BuildingSizeFlags { get; set; }
	[Reactive, Category("Building")] public uint8_t ScaffoldingSegmentType { get; set; }
	[Reactive, Category("Building")] public Colour ScaffoldingColour { get; set; }
	[Reactive, Category("Building"), Length(0, IndustryObject.MaxWallTypeCount)] public BindingList<S5HeaderViewModel> WallTypes { get; set; }
	[Reactive, Category("Building")] public S5HeaderViewModel? BuildingWall { get; set; }
	[Reactive, Category("Building")] public S5HeaderViewModel? BuildingWallEntrance { get; set; }
	[Reactive, Category("<unknown>")] public BindingList<IndustryObjectUnk38> var_38 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_E8 { get; set; }
	[Reactive, Category("Farm")] public uint8_t FarmTileNumImageAngles { get; set; }
	[Reactive, Category("Farm")] public uint8_t FarmGrowthStageWithNoProduction { get; set; }
	[Reactive, Category("Farm")] public uint8_t FarmIdealSize { get; set; }
	[Reactive, Category("Farm")] public uint8_t FarmNumStagesOfGrowth { get; set; }

	public IndustryViewModel(IndustryObject io)
	{
		AnimationSequences = new(io.AnimationSequences.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
		var_38 = new(io.var_38);
		BuildingHeights = new(io.BuildingHeights);
		BuildingAnimations = new(io.BuildingAnimations);
		BuildingVariations = new(io.BuildingVariations.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
		Buildings = new(io.Buildings);
		BuildingSizeFlags = io.BuildingSizeFlags;
		BuildingWall = io.BuildingWall == null ? null : new(io.BuildingWall);
		BuildingWallEntrance = io.BuildingWallEntrance == null ? null : new(io.BuildingWallEntrance);
		MinNumBuildings = io.MinNumBuildings;
		MaxNumBuildings = io.MaxNumBuildings;
		InitialProductionRate = new(io.InitialProductionRate);
		ProducedCargo = new(io.ProducedCargo.ConvertAll(x => new S5HeaderViewModel(x)));
		RequiredCargo = new(io.RequiredCargo.ConvertAll(x => new S5HeaderViewModel(x)));
		WallTypes = new(io.WallTypes.ConvertAll(x => new S5HeaderViewModel(x)));
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
		var_E8 = io.var_E8;
		FarmTileNumImageAngles = io.FarmTileNumImageAngles;
		FarmGrowthStageWithNoProduction = io.FarmGrowthStageWithNoProduction;
		FarmIdealSize = io.FarmIdealSize;
		FarmNumStagesOfGrowth = io.FarmNumStagesOfGrowth;
		MonthlyClosureChance = io.MonthlyClosureChance;
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override IndustryObject GetAsStruct(IndustryObject io)
		=> io with
		{
			BuildingSizeFlags = BuildingSizeFlags,
			BuildingWall = BuildingWall?.GetAsUnderlyingType(),
			BuildingWallEntrance = BuildingWallEntrance?.GetAsUnderlyingType(),
			MinNumBuildings = MinNumBuildings,
			MaxNumBuildings = MaxNumBuildings,
			NumBuildingParts = (uint8_t)BuildingAnimations.Count,
			NumBuildingVariations = (uint8_t)BuildingVariations.Count,
			ProducedCargo = ProducedCargo.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			RequiredCargo = RequiredCargo.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			WallTypes = WallTypes.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
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
			var_E8 = var_E8,
			FarmTileNumImageAngles = FarmTileNumImageAngles,
			FarmGrowthStageWithNoProduction = FarmGrowthStageWithNoProduction,
			FarmIdealSize = FarmIdealSize,
			FarmNumStagesOfGrowth = FarmNumStagesOfGrowth,
			MonthlyClosureChance = MonthlyClosureChance,
		};
}
