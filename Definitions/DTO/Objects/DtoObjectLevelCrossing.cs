using Definitions.Database;

namespace Definitions.DTO;

public class DtoObjectLevelCrossing : IDtoSubObject
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t ClosedAnimationFrameInterval { get; set; }
	public uint8_t ClosedAnimationFrameCount { get; set; }
	public uint8_t TransitionAnimationFrameCount { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint8_t TransitionAnimationDelayBitmask { get; set; }
	public UniqueObjectId Id { get; set; }
}
