using Definitions.Database;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectVehicle : IDtoSubObject
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
	public uint8_t ShipWakeSpacing { get; set; }
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
	public UniqueObjectId Id { get; set; }
}
