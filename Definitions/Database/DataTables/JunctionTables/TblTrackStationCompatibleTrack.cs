namespace Definitions.Database;

public class TblTrackStationCompatibleTrack
{
	public UniqueObjectId TrackStationId { get; set; }
	public required TblObjectTrackStation TrackStation { get; set; }

	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }
}