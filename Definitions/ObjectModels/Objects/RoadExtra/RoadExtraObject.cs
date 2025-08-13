using Definitions.ObjectModels.Objects.Road;

namespace Definitions.ObjectModels.Objects.RoadExtra;

public class RoadExtraObject : ILocoStruct
{
	public RoadTraitFlags RoadPieces { get; set; } = RoadTraitFlags.None;
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public bool Validate()
	{
		if (PaintStyle >= 2)
		{
			return false;
		}

		// This check missing from vanilla
		if (CostIndex >= 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		return BuildCostFactor > 0;
	}
}
