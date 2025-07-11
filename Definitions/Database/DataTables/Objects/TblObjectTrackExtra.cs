using Dat.Objects;

namespace Definitions.Database;

public class TblObjectTrackExtra : DbSubObject, IConvertibleToTable<TblObjectTrackExtra, TrackExtraObject>
{
	public TrackTraitFlags TrackPieces { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public static TblObjectTrackExtra FromObject(TblObject tbl, TrackExtraObject obj)
		=> new()
		{
			Parent = tbl,
			TrackPieces = obj.TrackPieces,
			PaintStyle = obj.PaintStyle,
			CostIndex = obj.CostIndex,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
		};
}
