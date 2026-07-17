namespace Definitions.Database;

public class TblTrackSignal
{
	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }

	public UniqueObjectId TrackSignalId { get; set; }
	public required TblObjectTrackSignal TrackSignal { get; set; }
}
