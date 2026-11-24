using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectIndustry : DbSubObject, IConvertibleToTable<TblObjectIndustry, IndustryObject>
{
	public uint32_t FarmImagesPerGrowthStage { get; set; }
	public uint8_t MinNumBuildings { get; set; }
	public uint8_t MaxNumBuildings { get; set; }
	public uint32_t Colours { get; set; }  // bitset
	public uint32_t BuildingSizeFlags { get; set; } // flags indicating the building types size 1:large4x4 { get; set; } 0:small1x1
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint8_t TotalOfTypeInScenario { get; set; } // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t ScaffoldingSegmentType { get; set; }
	public Colour ScaffoldingColour { get; set; }
	public Colour MapColour { get; set; }
	public IndustryObjectFlags Flags { get; set; }
	public uint8_t FarmTileNumImageAngles { get; set; } // How many viewing angles the farm tiles have
	public uint8_t FarmGrowthStageWithNoProduction { get; set; } // At this stage of growth (except 0) { get; set; } a field tile produces nothing
	public uint8_t FarmNumFields { get; set; } // Max production is reached at farmIdealSize * 25 tiles
	public uint8_t FarmNumStagesOfGrowth { get; set; } // How many growth stages there are sprites for
	public uint8_t MonthlyClosureChance { get; set; }

	public BuildingComponents BuildingComponents { get; set; } = new();
	public List<List<uint8_t>> AnimationSequences { get; set; } = [];
	public List<IndustryObjectRandomAnimation> RandomAnimations { get; set; } = [];
	public List<IndustryObjectProductionRateRange> InitialProductionRate { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargo { get; set; } = [];
	public List<ObjectModelHeader> RequiredCargo { get; set; } = [];
	public uint8_t NumFarmTileImages { get; set; }
	public List<ObjectModelHeader> WallTypes { get; set; } = [];
	public ObjectModelHeader? BuildingWall { get; set; }
	public ObjectModelHeader? BuildingWallEntrance { get; set; }
	public List<uint8_t> Buildings { get; set; } = [];

	public static TblObjectIndustry FromObject(TblObject tbl, IndustryObject obj)
		=> new()
		{
			Parent = tbl,
			FarmImagesPerGrowthStage = obj.FarmImagesPerGrowthStage,
			MinNumBuildings = obj.MinNumBuildings,
			MaxNumBuildings = obj.MaxNumBuildings,
			Colours = obj.Colours,
			BuildingSizeFlags = obj.BuildingSizeFlags,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			TotalOfTypeInScenario = obj.TotalOfTypeInScenario,
			CostIndex = obj.CostIndex,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			ScaffoldingSegmentType = obj.ScaffoldingSegmentType,
			ScaffoldingColour = obj.ScaffoldingColour,
			MapColour = obj.MapColour,
			Flags = obj.Flags,
			FarmTileNumImageAngles = obj.FarmTileNumImageAngles,
			FarmGrowthStageWithNoProduction = obj.FarmGrowthStageWithNoProduction,
			FarmNumFields = obj.FarmNumFields,
			FarmNumStagesOfGrowth = obj.FarmNumStagesOfGrowth,
			MonthlyClosureChance = obj.MonthlyClosureChance,
			BuildingComponents = obj.BuildingComponents,
			AnimationSequences = obj.AnimationSequences,
			RandomAnimations = obj.RandomAnimations,
			InitialProductionRate = obj.InitialProductionRate,
			ProducedCargo = obj.ProducedCargo,
			RequiredCargo = obj.RequiredCargo,
			NumFarmTileImages = obj.NumFarmTileImages,
			WallTypes = obj.WallTypes,
			BuildingWall = obj.BuildingWall,
			BuildingWallEntrance = obj.BuildingWallEntrance,
			Buildings = obj.Buildings,
		};
}
