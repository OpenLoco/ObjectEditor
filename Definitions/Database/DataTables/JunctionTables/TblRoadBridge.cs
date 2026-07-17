namespace Definitions.Database;

public class TblRoadBridge
{
	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }

	public UniqueObjectId BridgeId { get; set; }
	public required TblObjectBridge Bridge { get; set; }
}
