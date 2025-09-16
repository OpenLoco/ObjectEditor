namespace Definitions.ObjectModels.Objects.LevelCrossing;

public class LevelCrossingObject : ILocoStruct
{
	public int16_t CostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t ClosingFrames { get; set; }
	public uint8_t ClosedFrames { get; set; }
	public uint8_t var_0A { get; set; } // something like IdleAnimationFrames or something
	public uint16_t DesignedYear { get; set; }

	public bool Validate()
	{
		if (-SellCostFactor > CostFactor)
		{
			return false;
		}

		if (CostFactor <= 0)
		{
			return false;
		}

		return ClosingFrames switch
		{
			1 or 2 or 4 or 8 or 16 or 32 => true,
			_ => false,
		};
	}
}
