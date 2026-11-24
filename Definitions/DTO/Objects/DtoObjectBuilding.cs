using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

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
	public BuildingComponents BuildingComponents { get; set; } = new();
	public List<uint8_t> ProducedQuantity { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargoType { get; set; } = [];
	public List<ObjectModelHeader> ConsumedCargoType { get; set; } = [];
	public List<uint8_t> ProducedCargoQuantity { get; set; } = [];
	public List<uint8_t> ConsumedCargoQuantity { get; set; } = [];
	public TownAmenityCategory TownAmenityCategory { get; set; }
	public List<uint8_t[]> ElevatorHeightSequences { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
