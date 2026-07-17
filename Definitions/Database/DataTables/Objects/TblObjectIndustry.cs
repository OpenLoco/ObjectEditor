using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Industry;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectIndustry : DbSubObject, IConvertibleToTable<TblObjectIndustry, IndustryObject>
{
	public uint32_t FarmImagesPerGrowthStage { get; set; }
	public uint8_t MinNumBuildings { get; set; }
	public uint8_t MaxNumBuildings { get; set; }
	public uint32_t Colours { get; set; }
	public uint32_t BuildingSizeFlags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint8_t TotalOfTypeInScenario { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t ScaffoldingSegmentType { get; set; }
	public Colour ScaffoldingColour { get; set; }
	public Colour MapColour { get; set; }
	public IndustryObjectFlags Flags { get; set; }
	public uint8_t FarmTileNumImageAngles { get; set; }
	public uint8_t FarmGrowthStageWithNoProduction { get; set; }
	public uint8_t FarmNumFields { get; set; }
	public uint8_t FarmNumStagesOfGrowth { get; set; }
	public uint8_t MonthlyClosureChance { get; set; }
	public uint8_t var_E8 { get; set; }
	public UniqueObjectId? BuildingWallId { get; set; }
	public TblObjectWall? BuildingWall { get; set; }
	public UniqueObjectId? BuildingWallEntranceId { get; set; }
	public TblObjectWall? BuildingWallEntrance { get; set; }
	public string BuildingComponents { get; set; } = "null";
	public string AnimationSequences { get; set; } = "[]";
	public string var_38 { get; set; } = "[]";
	public string InitialProductionRate { get; set; } = "[]";
	public string Buildings { get; set; } = "[]";

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
			var_E8 = obj.var_E8,
			BuildingComponents = JsonSerializer.Serialize(obj.BuildingComponents),
			AnimationSequences = JsonSerializer.Serialize(obj.AnimationSequences),
			var_38 = JsonSerializer.Serialize(obj.var_38),
			InitialProductionRate = JsonSerializer.Serialize(obj.InitialProductionRate),
			Buildings = JsonSerializer.Serialize(obj.Buildings),
		};
}