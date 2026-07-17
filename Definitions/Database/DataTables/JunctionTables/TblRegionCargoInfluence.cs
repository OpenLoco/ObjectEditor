namespace Definitions.Database;

public class TblRegionCargoInfluence
{
	public UniqueObjectId RegionId { get; set; }
	public required TblObjectRegion Region { get; set; }

	public UniqueObjectId CargoId { get; set; }
	public required TblObjectCargo Cargo { get; set; }
}