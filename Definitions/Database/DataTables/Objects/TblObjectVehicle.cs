using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectVehicle : DbSubObject, IConvertibleToTable<TblObjectVehicle, VehicleObject>
{
	public TransportMode Mode { get; set; }
	public VehicleType Type { get; set; }
	public uint8_t NumCarComponents { get; set; }
	public ObjectModelHeader? RoadOrTrackType { get; set; }
	public object_id TrackTypeId { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t CostFactor { get; set; }
	public uint8_t Reliability { get; set; }
	public uint8_t RunCostIndex { get; set; }
	public int16_t RunCostFactor { get; set; }
	public CompanyColourType CompanyColourSchemeIndex { get; set; }
	public List<ObjectModelHeader> CompatibleVehicles { get; set; } = [];
	public List<ObjectModelHeader> RequiredTrackExtras { get; set; } = [];
	public List<VehicleObjectCar> CarComponents { get; set; } = [];
	public List<BodySprite> BodySprites { get; set; } = [];
	public List<BogieSprite> BogieSprites { get; set; } = [];
	public uint16_t Power { get; set; }
	public Speed16 Speed { get; set; }
	public Speed16 RackSpeed { get; set; }
	public uint16_t Weight { get; set; }
	public VehicleObjectFlags Flags { get; set; }
	public List<uint8_t> MaxCargo { get; set; } = [0, 0];
	public List<CargoCategory>[] CompatibleCargoCategories { get; set; } = new List<CargoCategory>[2];
	public Dictionary<CargoCategory, uint8_t> CargoTypeSpriteOffsets { get; set; } = [];
	public uint8_t NumSimultaneousCargoTypes { get; set; }
	public List<EmitterAnimation> ParticleEmitters { get; set; } = [];
	public uint8_t ShipWakeSpacing { get; set; } // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public ObjectModelHeader? RackRail { get; set; }
	public DrivingSoundType DrivingSoundType { get; set; }
	public FrictionSound? FrictionSound { get; set; }
	public SimpleMotorSound? SimpleMotorSound { get; set; }
	public GearboxMotorSound? GearboxMotorSound { get; set; }
	public ObjectModelHeader? DrivingSound { get; set; }
	public List<ObjectModelHeader> StartSounds { get; set; } = [];
	public List<ObjectModelHeader> CrossingSounds { get; set; } = [];

	public static TblObjectVehicle FromObject(TblObject tbl, VehicleObject obj)
		=> new()
		{
			Parent = tbl,
			Mode = obj.Mode,
			Type = obj.Type,
			NumCarComponents = obj.NumCarComponents,
			RoadOrTrackType = obj.RoadOrTrackType,
			TrackTypeId = obj.TrackTypeId,
			CostIndex = obj.CostIndex,
			CostFactor = obj.CostFactor,
			Reliability = obj.Reliability,
			RunCostIndex = obj.RunCostIndex,
			RunCostFactor = obj.RunCostFactor,
			CompanyColourSchemeIndex = obj.CompanyColourSchemeIndex,
			CompatibleVehicles = obj.CompatibleVehicles,
			RequiredTrackExtras = obj.RequiredTrackExtras,
			CarComponents = obj.CarComponents,
			BodySprites = obj.BodySprites,
			BogieSprites = obj.BogieSprites,
			Power = obj.Power,
			Speed = obj.Speed,
			RackSpeed = obj.RackSpeed,
			Weight = obj.Weight,
			Flags = obj.Flags,
			MaxCargo = obj.MaxCargo,
			CompatibleCargoCategories = obj.CompatibleCargoCategories,
			CargoTypeSpriteOffsets = obj.CargoTypeSpriteOffsets,
			NumSimultaneousCargoTypes = obj.NumSimultaneousCargoTypes,
			ParticleEmitters = obj.ParticleEmitters,
			ShipWakeSpacing = obj.ShipWakeSpacing, // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			RackRail = obj.RackRail,
			DrivingSoundType = obj.DrivingSoundType,
			FrictionSound = obj.FrictionSound,
			SimpleMotorSound = obj.SimpleMotorSound,
			GearboxMotorSound = obj.GearboxMotorSound,
			DrivingSound = obj.DrivingSound,
			StartSounds = obj.StartSounds,
			CrossingSounds = obj.CrossingSounds,
		};
}
