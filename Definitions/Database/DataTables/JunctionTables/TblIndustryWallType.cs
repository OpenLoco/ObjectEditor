namespace Definitions.Database;

public class TblIndustryWallType
{
	public UniqueObjectId IndustryId { get; set; }
	public required TblObjectIndustry Industry { get; set; }

	public UniqueObjectId WallId { get; set; }
	public required TblObjectWall Wall { get; set; }
}