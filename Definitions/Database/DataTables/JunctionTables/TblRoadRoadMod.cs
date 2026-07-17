namespace Definitions.Database;

public class TblRoadRoadMod
{
	public UniqueObjectId RoadId { get; set; }
	public required TblObjectRoad Road { get; set; }

	public UniqueObjectId RoadExtraId { get; set; }
	public required TblObjectRoadExtra RoadExtra { get; set; }
}
