using Definitions.ObjectModels.Objects.Vehicle;

namespace Definitions.Database;

public class TblVehicleEmitterAnimation
{
	public UniqueObjectId Id { get; set; }
	public UniqueObjectId VehicleId { get; set; }
	public required TblObjectVehicle Vehicle { get; set; }

	public UniqueObjectId AnimationObjectId { get; set; }
	public required TblObjectSteam AnimationObject { get; set; }

	public uint8_t EmitterVerticalPos { get; set; }
	public SimpleAnimationType Type { get; set; }
}