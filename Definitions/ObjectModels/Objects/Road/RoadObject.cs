namespace Definitions.ObjectModels.Objects.Road;

public class RoadObject : ILocoStruct
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public TownSize TargetTownSize { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
