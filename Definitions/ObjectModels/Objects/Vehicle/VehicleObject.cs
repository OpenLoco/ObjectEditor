using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Vehicle;

public class VehicleObject : ILocoStruct
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
	public ObjectModelHeader[] CompatibleVehicles { get; set; } = [];
	public ObjectModelHeader[] RequiredTrackExtras { get; set; } = [];
	public VehicleObjectCar[] CarComponents { get; set; } = [];
	public BodySprite[] BodySprites { get; set; } = [];
	public BogieSprite[] BogieSprites { get; set; } = [];
	public uint16_t Power { get; set; }
	public Speed16 Speed { get; set; }
	public Speed16 RackSpeed { get; set; }
	public uint16_t Weight { get; set; }
	public VehicleObjectFlags Flags { get; set; }
	public uint8_t[] MaxCargo { get; set; } = new uint8_t[2]; // VehicleObjectLoader.Constants.MaxCompatibleCargoCategories
	public List<CargoCategory>[] CompatibleCargoCategories { get; set; } = new List<CargoCategory>[2]; // VehicleObjectLoader.Constants.MaxCompatibleCargoCategories
	public Dictionary<CargoCategory, uint8_t> CargoTypeSpriteOffsets { get; set; } = [];
	public uint8_t NumSimultaneousCargoTypes { get; set; }
	public EmitterAnimation[] ParticleEmitters { get; set; } = [];
	public uint8_t ShipWakeSpacing { get; set; } // the distance between each wake of the boat. 0 will be a single wake. anything > 0 gives dual wakes
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public ObjectModelHeader? RackRail { get; set; }
	// the driving sound can only be one of these 3 sound types
	public DrivingSoundType DrivingSoundType { get; set; }
	public FrictionSound? FrictionSound { get; set; }
	public SimpleMotorSound? SimpleMotorSound { get; set; }
	public GearboxMotorSound? GearboxMotorSound { get; set; }
	public ObjectModelHeader? Sound { get; set; }
	public uint8_t[] var_135 { get; set; } = [];
	public ObjectModelHeader[] StartSounds { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (RunCostIndex > 32)
		{
			yield return new ValidationResult($"{nameof(RunCostIndex)} must be between 0 and 32 inclusive.", [nameof(RunCostIndex)]);
		}

		if (CostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(CostFactor)} must be positive.", [nameof(CostFactor)]);
		}

		if (RunCostFactor < 0)
		{
			yield return new ValidationResult($"{nameof(RunCostFactor)} must be non-negative.", [nameof(RunCostFactor)]);
		}

		if (Flags.HasFlag(VehicleObjectFlags.AnyRoadType))
		{
			if (RequiredTrackExtras.Length != 0)
			{
				yield return new ValidationResult($"{nameof(RequiredTrackExtras)} must be empty if {nameof(VehicleObjectFlags.AnyRoadType)} is set.", [nameof(RequiredTrackExtras), nameof(Flags)]);
			}

			if (Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				yield return new ValidationResult($"{nameof(Flags)} cannot have both {nameof(VehicleObjectFlags.AnyRoadType)} and {nameof(VehicleObjectFlags.RackRail)} set.", [nameof(Flags)]);
			}
		}

		if (RequiredTrackExtras.Length > 4)
		{
			yield return new ValidationResult($"{nameof(RequiredTrackExtras)} must have at most 4 entries.", [nameof(RequiredTrackExtras)]);
		}

		if (NumSimultaneousCargoTypes > 2)
		{
			yield return new ValidationResult($"{nameof(NumSimultaneousCargoTypes)} must be between 0 and 2 inclusive.", [nameof(NumSimultaneousCargoTypes)]);
		}

		if (CompatibleVehicles.Length > 8)
		{
			yield return new ValidationResult($"{nameof(CompatibleVehicles)} must have at most 8 entries.", [nameof(CompatibleVehicles)]);
		}

		if (RackSpeed > Speed)
		{
			yield return new ValidationResult($"{nameof(RackSpeed)} must be less than or equal to {nameof(Speed)}.", [nameof(RackSpeed), nameof(Speed)]);
		}

		foreach (var bodySprite in BodySprites)
		{
			foreach (var result in bodySprite.Validate(validationContext))
			{
				yield return result;
			}
		}

		foreach (var bogieSprite in BogieSprites)
		{
			foreach (var result in bogieSprite.Validate(validationContext))
			{
				yield return result;
			}
		}
	}
}
