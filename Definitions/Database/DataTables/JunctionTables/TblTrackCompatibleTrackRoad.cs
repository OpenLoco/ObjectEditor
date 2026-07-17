namespace Definitions.Database;

public class TblTrackCompatibleTrackRoad
{
	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }

	public UniqueObjectId CompatibleTrackRoadId { get; set; }
	public required TblObjectTrack CompatibleTrackRoad { get; set; }
}
