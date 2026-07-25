using Definitions.ObjectModels.Objects.LevelCrossing;

namespace Definitions.Database;

public class TblObjectLevelCrossing : DbSubObject, IConvertibleToTable<TblObjectLevelCrossing, LevelCrossingObject>
{
	public uint16_t DesignedYear { get; set; }
	public int16_t CostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t ClosedAnimationDelay { get; set; }
	public uint8_t ClosedAnimationFrameCount { get; set; }
	public uint8_t TransitionAnimationFrameCount { get; set; }
	public uint8_t TransitionAnimationDelayBitmask { get; set; }

	public static TblObjectLevelCrossing FromObject(TblObject tbl, LevelCrossingObject obj)
		=> new()
		{
			Parent = tbl,
			DesignedYear = obj.DesignedYear,
			CostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			ClosedAnimationDelay = obj.ClosedAnimationDelay,
			ClosedAnimationFrameCount = obj.ClosedAnimationFrameCount,
			TransitionAnimationFrameCount = obj.TransitionAnimationFrameCount,
			TransitionAnimationDelayBitmask = obj.TransitionAnimationDelayBitmask,
		};
}
