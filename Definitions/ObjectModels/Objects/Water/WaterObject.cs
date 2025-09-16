namespace Definitions.ObjectModels.Objects.Water;

public class WaterObject : ILocoStruct
{
	public uint8_t CostIndex { get; set; }
	public uint8_t var_03 { get; set; }
	public int16_t CostFactor { get; set; }

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		if (CostFactor <= 0)
		{
			return false;
		}

		return true;
	}
}
