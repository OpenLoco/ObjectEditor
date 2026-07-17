namespace Definitions.Database;

public class TblBuildingConsumedCargo
{
	public UniqueObjectId BuildingId { get; set; }
	public required TblObjectBuilding Building { get; set; }

	public UniqueObjectId CargoId { get; set; }
	public required TblObjectCargo Cargo { get; set; }
}