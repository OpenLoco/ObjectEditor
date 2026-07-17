namespace Definitions.Database;

public class TblVehicleStartSound
{
	public UniqueObjectId VehicleId { get; set; }
	public required TblObjectVehicle Vehicle { get; set; }

	public UniqueObjectId SoundId { get; set; }
	public required TblObjectSound Sound { get; set; }
}