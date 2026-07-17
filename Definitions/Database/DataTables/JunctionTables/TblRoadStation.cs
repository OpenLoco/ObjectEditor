namespace Definitions.Database;

public class TblRoadStation
{
	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }

	public UniqueObjectId RoadStationId { get; set; }
	public required TblObjectRoadStation RoadStation { get; set; }
}
