namespace Definitions.ObjectModels.Objects.Dock;

public class DockObject : ILocoStruct
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public DockObjectFlags Flags { get; set; }
	public uint8_t NumBuildingPartAnimations { get; set; }
	public uint8_t NumBuildingVariationParts { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	// these map to [Pos2 BoatPosition] in the Dock object
	public coord_t BoatPositionX { get; set; }
	public coord_t BoatPositionY { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
