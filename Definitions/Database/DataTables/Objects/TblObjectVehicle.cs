using Definitions.ObjectModels.Objects.Vehicle;

namespace Definitions.Database;

public class TblObjectVehicle : DbSubObject, IConvertibleToTable<TblObjectVehicle, VehicleObject>
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
	//public TblObjectTrackExtra RackRailType { get; set; }

	//public List<S5Header> CompatibleVehicles { get; set; }
	//public List<S5Header> RequiredTrackExtras { get; set; }
	//public VehicleObjectCar[] CarComponents { get; set; }
	//public BodySprite[] BodySprites { get; set; }
	//public BogieSprite[] BogieSprites { get; set; }
	//public List<uint8_t> MaxCargo { get; set; }
	//public List<List<CargoCategory>> CompatibleCargoCategories { get; set; }
	//public Dictionary<CargoCategory, uint8_t> CargoTypeSpriteOffsets { get; set; }
	//public ICollection<SimpleAnimation> Animation { get; set; }

	public static TblObjectVehicle FromObject(TblObject tbl, VehicleObject obj)
		=> new()
		{
			Parent = tbl,
			Mode = obj.Mode,
			Type = obj.Type,
			NumCarComponents = obj.NumCarComponents,
			TrackTypeId = obj.TrackTypeId,
			NumRequiredTrackExtras = (uint8_t)obj.RequiredTrackExtras.Length,
			CostIndex = obj.CostIndex,
			CostFactor = obj.CostFactor,
			Reliability = obj.Reliability,
			RunCostIndex = obj.RunCostIndex,
			RunCostFactor = obj.RunCostFactor,
			NumCompatibleVehicles = (uint8_t)obj.CompatibleVehicles.Length,
			Power = obj.Power,
			Speed = obj.Speed,
			RackSpeed = obj.RackSpeed,
			Weight = obj.Weight,
			Flags = obj.Flags,
			ShipWakeOffset = obj.ShipWakeOffset, // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			DrivingSoundType = obj.DrivingSoundType,
			//RackRailType = obj.RackRailType,
		};
}
