using Definitions.ObjectModels.Objects.Track;
using System.Text.Json;

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
	public string Tunnel { get; set; } = "null";
	public string TrackMods { get; set; } = "[]";
	public string Signals { get; set; } = "[]";
	public string TracksAndRoads { get; set; } = "[]";
	public string Bridges { get; set; } = "[]";
	public string Stations { get; set; } = "[]";

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
			Tunnel = JsonSerializer.Serialize(obj.Tunnel),
			TrackMods = JsonSerializer.Serialize(obj.TrackMods),
			Signals = JsonSerializer.Serialize(obj.Signals),
			TracksAndRoads = JsonSerializer.Serialize(obj.TracksAndRoads),
			Bridges = JsonSerializer.Serialize(obj.Bridges),
			Stations = JsonSerializer.Serialize(obj.Stations),
		};
}