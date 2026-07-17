namespace Definitions.Database;

public class TblTrackSignalCompatibleTrack
{
	public UniqueObjectId TrackSignalId { get; set; }
	public required TblObjectTrackSignal TrackSignal { get; set; }

	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }
}