namespace Definitions.Database;

public class TblTrackBridge
{
	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }

	public UniqueObjectId BridgeId { get; set; }
	public required TblObjectBridge Bridge { get; set; }
}
