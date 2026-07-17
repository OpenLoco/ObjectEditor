namespace Definitions.Database;

public class TblRoadCompatibleTrackRoad
{
	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }

	public UniqueObjectId CompatibleTrackRoadId { get; set; }
	public required TblObjectTrack CompatibleTrackRoad { get; set; }
}