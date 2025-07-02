using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectIndustry : IHasId
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
		public uint8_t FarmIdealSize { get; set; }
		public uint8_t FarmNumStagesOfGrowth { get; set; }
		public uint8_t MonthlyClosureChance { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
