namespace Definitions.Database;

public class TblBridgeCompatibleRoad
{
	public UniqueObjectId BridgeId { get; set; }
	public required TblObjectBridge Bridge { get; set; }

	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }
}