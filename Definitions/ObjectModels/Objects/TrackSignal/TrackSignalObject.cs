using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.TrackSignal;

public class TrackSignalObject : ILocoStruct
{
	public TrackSignalObjectFlags Flags { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t NumFrames { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_0B { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];

	public bool Validate()
	{
		// animationSpeed must be 1 less than a power of 2 (its a mask)
		switch (AnimationSpeed)
		{
			case 0:
			case 1:
			case 3:
			case 7:
			case 15:
				break;
			default:
				return false;
		}

		switch (NumFrames)
		{
			case 4:
			case 7:
			case 10:
				break;
			default:
				return false;
		}

		if (CostIndex > 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		if (CompatibleTrackObjects.Count > 7)
		{
			return false;
		}

		return true;
	}
}
