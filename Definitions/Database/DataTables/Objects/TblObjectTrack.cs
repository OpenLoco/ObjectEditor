using Definitions.ObjectModels.Objects.Track;

namespace Definitions.Database;

public class TblObjectTrack : DbSubObject, IConvertibleToTable<TblObjectTrack, TrackObject>
{
	public TrackTraitFlags TrackPieces { get; set; }
	public TrackTraitFlags StationTrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 CurveSpeed { get; set; }
	public TrackObjectFlags Flags { get; set; }
	public uint8_t VehicleDisplayListVerticalOffset { get; set; }
	public uint8_t var_06 { get; set; }
	public UniqueObjectId TunnelId { get; set; }
	public TblObjectTunnel? Tunnel { get; set; }

	public static TblObjectTrack FromObject(TblObject tbl, TrackObject obj)
		=> new()
		{
			Parent = tbl,
			TrackPieces = obj.TrackPieces,
			StationTrackPieces = obj.StationTrackPieces,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			TunnelCostFactor = obj.TunnelCostFactor,
			CostIndex = obj.CostIndex,
			CurveSpeed = obj.MaxCurveSpeed,
			Flags = obj.Flags,
			VehicleDisplayListVerticalOffset = obj.VehicleDisplayListVerticalOffset,
			var_06 = obj.var_06,
		};
}