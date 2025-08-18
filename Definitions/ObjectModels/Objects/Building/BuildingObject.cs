using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Building;

[Flags]
public enum BuildingObjectFlags : uint8_t
{
	None = 0,
	LargeTile = 1 << 0, // 2x2 tile
	MiscBuilding = 1 << 1,
	Indestructible = 1 << 2,
	IsHeadquarters = 1 << 3,
}

public class BuildingObject : ILocoStruct
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

	public List<uint8_t> BuildingHeights { get; set; } = [];
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];
	public List<uint8_t> ProducedQuantity { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargo { get; set; } = [];
	public List<ObjectModelHeader> RequiredCargo { get; set; } = [];

	public List<uint8_t[]> ElevatorSequence1 { get; set; } = [];
	public List<uint8_t[]> ElevatorSequence2 { get; set; } = [];
	public List<uint8_t[]> ElevatorSequence3 { get; set; } = [];
	public List<uint8_t[]> ElevatorSequence4 { get; set; } = [];

	public bool Validate()
		=> ProducedQuantity.Count == 2
		&& BuildingHeights.Count is not 0 and not > 63
		&& BuildingAnimations.Count is not 0 and not > 63
		&& BuildingHeights.Count == BuildingAnimations.Count
		&& BuildingVariations.Count is not 0 and <= 31;
}
