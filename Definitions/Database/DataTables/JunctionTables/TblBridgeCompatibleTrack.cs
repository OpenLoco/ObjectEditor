namespace Definitions.Database;

public class TblBridgeCompatibleTrack
{
	public UniqueObjectId BridgeId { get; set; }
	public required TblObjectBridge Bridge { get; set; }

	public UniqueObjectId TrackId { get; set; }
	public required TblObjectTrack Track { get; set; }
}