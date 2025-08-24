using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Vehicle;

public class VehicleObject : ILocoStruct
{
	public TransportMode Mode { get; set; }
	public VehicleType Type { get; set; }
	public uint8_t NumCarComponents { get; set; }
	public object_id TrackTypeId { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t CostFactor { get; set; }
	public uint8_t Reliability { get; set; }
	public uint8_t RunCostIndex { get; set; }
	public int16_t RunCostFactor { get; set; }
	public CompanyColourType SpecialColourSchemeIndex { get; set; }
	public uint16_t Power { get; set; }
	public Speed16 Speed { get; set; }
	public Speed16 RackSpeed { get; set; }
	public uint16_t Weight { get; set; }
	public VehicleObjectFlags Flags { get; set; }
	public uint8_t ShipWakeOffset { get; set; } // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<List<CargoCategory>> CompatibleCargoCategories { get; set; } = [];
	public SimpleAnimation[] Animation { get; set; }
	public uint8_t NumSimultaneousCargoTypes { get; set; }
	public ObjectModelHeader? TrackType { get; set; }
	public ObjectModelHeader? RackRail { get; set; }
	public ObjectModelHeader? Sound { get; set; }
	public List<ObjectModelHeader> AnimationHeaders { get; set; } = [];
	public List<ObjectModelHeader> StartSounds { get; set; } = [];
	public List<ObjectModelHeader> CompatibleVehicles { get; set; } = [];
	public List<ObjectModelHeader> RequiredTrackExtras { get; set; } = [];
	public VehicleObjectCar[] CarComponents { get; set; } = [];
	public BodySprite[] BodySprites { get; set; } = [];
	public BogieSprite[] BogieSprites { get; set; } = [];

	// the driving sound can only be one of these 3 sound types
	public DrivingSoundType DrivingSoundType { get; set; }
	public FrictionSound? FrictionSound { get; set; }
	public SimpleMotorSound? SimpleMotorSound { get; set; }
	public GearboxMotorSound? GearboxMotorSound { get; set; }

	public Dictionary<CargoCategory, uint8_t> CargoTypeSpriteOffsets { get; set; } = new();
	public List<uint8_t> MaxCargo { get; set; } = [];
	public uint8_t[] var_135 { get; set; } = [];

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		if (RunCostIndex > 32)
		{
			return false;
		}

		if (CostFactor <= 0)
		{
			return false;
		}

		if (RunCostFactor < 0)
		{
			return false;
		}

		if (Flags.HasFlag(VehicleObjectFlags.AnyRoadType))
		{
			if (RequiredTrackExtras.Count != 0)
			{
				return false;
			}

			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				return false;
			}
		}

		if (RequiredTrackExtras.Count > 4)
		{
			return false;
		}

		if (NumSimultaneousCargoTypes > 2)
		{
			return false;
		}

		if (CompatibleVehicles.Count > 8)
		{
			return false;
		}

		if (RackSpeed > Speed)
		{
			return false;
		}

		foreach (var bodySprite in BodySprites)
		{
			if (!bodySprite.Flags.HasFlag(BodySpriteFlags.HasSprites))
			{
				continue;
			}

			switch (bodySprite.NumFlatRotationFrames)
			{
				case 8:
				case 16:
				case 32:
				case 64:
				case 128:
					break;
				default:
					return false;
			}

			switch (bodySprite.NumSlopedRotationFrames)
			{
				case 4:
				case 8:
				case 16:
				case 32:
					break;
				default:
					return false;
			}

			switch (bodySprite.NumAnimationFrames)
			{
				case 1:
				case 2:
				case 4:
					break;
				default:
					return false;
			}

			if (bodySprite.NumCargoLoadFrames is < 1 or > 5)
			{
				return false;
			}

			switch (bodySprite.NumRollFrames)
			{
				case 1:
				case 3:
					break;
				default:
					return false;
			}
		}

		foreach (var bogieSprite in BogieSprites)
		{
			if (!bogieSprite.Flags.HasFlag(BogieSpriteFlags.HasSprites))
			{
				continue;
			}

			switch (bogieSprite.RollStates)
			{
				case 1:
				case 2:
				case 4:
					break;
				default:
					return false;
			}
		}

		return true;
	}
}
