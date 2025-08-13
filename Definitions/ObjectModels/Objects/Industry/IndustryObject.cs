using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Industry;

public class IndustryObject : ILocoStruct
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
	public uint8_t FarmIdealSize { get; set; } // Max production is reached at farmIdealSize * 25 tiles
	public uint8_t FarmNumStagesOfGrowth { get; set; } // How many growth stages there are sprites for
	public uint8_t MonthlyClosureChance { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
