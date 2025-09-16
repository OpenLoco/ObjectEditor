using Definitions.ObjectModels.Objects.Track;

namespace Definitions.ObjectModels.Objects.TrackExtra;

public class TrackExtraObject : ILocoStruct
{
	public TrackTraitFlags TrackPieces { get; set; }
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
		if (CostIndex > 32)
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
