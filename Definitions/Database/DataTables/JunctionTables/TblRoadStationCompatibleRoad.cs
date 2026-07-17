namespace Definitions.Database;

public class TblRoadStationCompatibleRoad
{
	public UniqueObjectId RoadStationId { get; set; }
	public required TblObjectRoadStation RoadStation { get; set; }

	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }
}