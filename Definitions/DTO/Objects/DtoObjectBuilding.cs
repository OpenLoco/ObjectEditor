using Dat.Data;
using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectBuilding : IDtoSubObject
	{
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }
		public BuildingObjectFlags Flags { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint16_t SellCostFactor { get; set; }
		public int16_t DemolishRatingReduction { get; set; }
		public uint8_t ScaffoldingSegmentType { get; set; }
		public Colour ScaffoldingColour { get; set; }
		public uint32_t Colours { get; set; }
		public uint8_t GeneratorFunction { get; set; }
		public uint8_t AverageNumberOnMap { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
