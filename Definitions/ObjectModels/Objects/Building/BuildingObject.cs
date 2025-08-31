using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Building;

public class BuildingObject : ILocoStruct, IHasBuildingComponents
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

	public uint8_t var_A6 { get; set; }
	public uint8_t var_A7 { get; set; }
	public uint8_t var_A8 { get; set; }
	public uint8_t var_A9 { get; set; }

	public uint8_t var_AC { get; set; }

	public BuildingComponents BuildingComponents { get; set; } = new();

	public List<uint8_t> ProducedQuantity { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargo { get; set; } = [];
	public List<ObjectModelHeader> RequiredCargo { get; set; } = [];

	public List<uint8_t[]> ElevatorHeightSequences { get; set; } = [];

	public bool Validate()
		=> ProducedQuantity.Count == 2
		&& BuildingComponents.Validate();
}
