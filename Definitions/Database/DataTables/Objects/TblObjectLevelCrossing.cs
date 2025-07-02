using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectLevelCrossing : DbSubObject, IConvertibleToTable<TblObjectLevelCrossing, LevelCrossingObject>
	{
		public int16_t CostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint8_t AnimationSpeed { get; set; }
		public uint8_t ClosingFrames { get; set; }
		public uint8_t ClosedFrames { get; set; }
		public uint16_t DesignedYear { get; set; }

		//public uint8_t var_0A { get; set; } // something like IdleAnimationFrames or something

		public static TblObjectLevelCrossing FromObject(TblObject tbl, LevelCrossingObject obj)
			=> new()
			{
				Parent = tbl,
				CostFactor = obj.CostFactor,
				SellCostFactor = obj.SellCostFactor,
				CostIndex = obj.CostIndex,
				AnimationSpeed = obj.AnimationSpeed,
				ClosingFrames = obj.ClosingFrames,
				ClosedFrames = obj.ClosedFrames,
				DesignedYear = obj.DesignedYear,
			};
	}
}
