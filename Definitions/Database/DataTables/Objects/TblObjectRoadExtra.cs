using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectRoadExtra : DbSubObject, IConvertibleToTable<TblObjectRoadExtra, RoadExtraObject>
	{
		public RoadTraitFlags RoadPieces { get; set; }
		public uint8_t PaintStyle { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }

		public static TblObjectRoadExtra FromObject(TblObject tbl, RoadExtraObject obj)
			=> new()
			{
				Parent = tbl,
				RoadPieces = obj.RoadPieces,
				PaintStyle = obj.PaintStyle,
				CostIndex = obj.CostIndex,
				BuildCostFactor = obj.BuildCostFactor,
				SellCostFactor = obj.SellCostFactor,
			};
	}
}
