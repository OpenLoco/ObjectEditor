using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Industry;

namespace Definitions.DTO.Objects;

public class DtoObjectIndustry : IDtoSubObject
{
	public UniqueObjectId Id { get; set; }
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
	public string BuildingComponents { get; set; } = "null";
	public string AnimationSequences { get; set; } = "[]";
	public string var_38 { get; set; } = "[]";
	public string InitialProductionRate { get; set; } = "[]";
	public string Buildings { get; set; } = "[]";
}