namespace Definitions.Database;

public class TblIndustryRequiredCargo
{
	public UniqueObjectId IndustryId { get; set; }
	public required TblObjectIndustry Industry { get; set; }

	public UniqueObjectId CargoId { get; set; }
	public required TblObjectCargo Cargo { get; set; }
}