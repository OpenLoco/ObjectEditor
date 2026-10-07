using Definitions.Database;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectTrack : IDtoSubObject
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
	public UniqueObjectId Id { get; set; }
}
