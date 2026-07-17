namespace Definitions.Database;

public class TblRegionDependentObject
{
	public UniqueObjectId RegionId { get; set; }
	public required TblObjectRegion Region { get; set; }

	public UniqueObjectId DependentObjectId { get; set; }
	public required TblObject DependentObject { get; set; }
}