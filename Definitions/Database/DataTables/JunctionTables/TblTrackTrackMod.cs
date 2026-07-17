namespace Definitions.Database;

public class TblTrackTrackMod
{
	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }

	public UniqueObjectId TrackExtraId { get; set; }
	public required TblObjectTrackExtra TrackExtra { get; set; }
}
