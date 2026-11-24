using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectTrack : DbSubObject, IConvertibleToTable<TblObjectTrack, TrackObject>
{
	public TrackTraitFlags TrackPieces { get; set; }
	public TrackTraitFlags StationTrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 MaxCurveSpeed { get; set; }
	public TrackObjectFlags Flags { get; set; }
	public uint8_t VehicleDisplayListVerticalOffset { get; set; }

	public uint8_t var_06 { get; set; }
	public ObjectModelHeader Tunnel { get; set; } = null!;
	public List<ObjectModelHeader> TrackMods { get; set; } = [];
	public List<ObjectModelHeader> Signals { get; set; } = [];
	public List<ObjectModelHeader> TracksAndRoads { get; set; } = [];
	public List<ObjectModelHeader> Bridges { get; set; } = [];
	public List<ObjectModelHeader> Stations { get; set; } = [];

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
			MaxCurveSpeed = obj.MaxCurveSpeed,
			Flags = obj.Flags,
			VehicleDisplayListVerticalOffset = obj.VehicleDisplayListVerticalOffset,
			var_06 = obj.var_06,
			Tunnel = obj.Tunnel,
			TrackMods = obj.TrackMods,
			Signals = obj.Signals,
			TracksAndRoads = obj.TracksAndRoads,
			Bridges = obj.Bridges,
			Stations = obj.Stations,
		};
}
