namespace Definitions.Database;

public class TblTrackStation
{
	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }

	public UniqueObjectId TrackStationId { get; set; }
	public required TblObjectTrackStation TrackStation { get; set; }
}
