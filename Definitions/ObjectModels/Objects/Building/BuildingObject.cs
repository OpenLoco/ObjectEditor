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

	public bool Validate() => throw new NotImplementedException();
}
