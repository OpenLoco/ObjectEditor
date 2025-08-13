namespace Definitions.ObjectModels.Objects.Water;
public class WaterObject : ILocoStruct
{
	public uint8_t CostIndex { get; set; }
	public int16_t CostFactor { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
