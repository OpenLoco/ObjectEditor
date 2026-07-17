namespace Definitions.Database;

public class TblVehicleRequiredTrackExtra
{
	public UniqueObjectId VehicleId { get; set; }
	public required TblObjectVehicle Vehicle { get; set; }

	public UniqueObjectId TrackExtraId { get; set; }
	public required TblObjectTrackExtra TrackExtra { get; set; }
}