using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectIndustry : IDtoSubObject
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
	public UniqueObjectId Id { get; set; }
}
