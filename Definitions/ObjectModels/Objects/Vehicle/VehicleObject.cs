namespace Definitions.ObjectModels.Objects.Vehicle;
public class VehicleObject : ILocoStruct
{
	public TransportMode Mode { get; set; }
	public VehicleType Type { get; set; }
	public uint8_t NumCarComponents { get; set; }
	public object_id TrackTypeId { get; set; }
	public uint8_t NumRequiredTrackExtras { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t CostFactor { get; set; }
	public uint8_t Reliability { get; set; }
	public uint8_t RunCostIndex { get; set; }
	public int16_t RunCostFactor { get; set; }
	public uint8_t NumCompatibleVehicles { get; set; }
	public uint16_t Power { get; set; }
	public Speed16 Speed { get; set; }
	public Speed16 RackSpeed { get; set; }
	public uint16_t Weight { get; set; }
	public VehicleObjectFlags Flags { get; set; }
	public uint8_t ShipWakeOffset { get; set; } // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public DrivingSoundType DrivingSoundType { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
