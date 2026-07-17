namespace Definitions.Database;

public class TblVehicleCompatibleVehicle
{
	public UniqueObjectId VehicleId { get; set; }
	public required TblObjectVehicle Vehicle { get; set; }

	public UniqueObjectId CompatibleVehicleId { get; set; }
	public required TblObjectVehicle CompatibleVehicle { get; set; }
}