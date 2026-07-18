using Definitions.ObjectModels.Objects.Vehicle;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectVehicle : DbSubObject, IConvertibleToTable<TblObjectVehicle, VehicleObject>
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
	public string CompanyColourSchemeIndex { get; set; } = string.Empty;
	public uint16_t Power { get; set; }
	public Speed16 Speed { get; set; }
	public Speed16 RackSpeed { get; set; }
	public uint16_t Weight { get; set; }
	public VehicleObjectFlags Flags { get; set; }
	public uint8_t ShipWakeSpacing { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public DrivingSoundType DrivingSoundType { get; set; }
	public uint8_t NumSimultaneousCargoTypes { get; set; }
	public string CarComponents { get; set; } = "[]";
	public string BodySprites { get; set; } = "[]";
	public string BogieSprites { get; set; } = "[]";
	public string MaxCargo { get; set; } = "[]";
	public string CompatibleCargoCategories { get; set; } = "[]";
	public string CargoTypeSpriteOffsets { get; set; } = "{}";
	public string CompatibleVehicles { get; set; } = "[]";
	public string RequiredTrackExtras { get; set; } = "[]";
	public string StartSounds { get; set; } = "[]";
	public string ParticleEmitters { get; set; } = "[]";
	public string RackRail { get; set; } = "null";
	public string Sound { get; set; } = "null";
	public string FrictionSound { get; set; } = "null";
	public string SimpleMotorSound { get; set; } = "null";
	public string GearboxMotorSound { get; set; } = "null";
	public string var_135 { get; set; } = "[]";

	public static TblObjectVehicle FromObject(TblObject tbl, VehicleObject obj)
		=> new()
		{
			Parent = tbl,
			Mode = obj.Mode,
			Type = obj.Type,
			NumCarComponents = obj.NumCarComponents,
			TrackTypeId = obj.TrackTypeId,
			CostIndex = obj.CostIndex,
			CostFactor = obj.CostFactor,
			Reliability = obj.Reliability,
			RunCostIndex = obj.RunCostIndex,
			RunCostFactor = obj.RunCostFactor,
			CompanyColourSchemeIndex = obj.CompanyColourSchemeIndex.ToString(),
			Power = obj.Power,
			Speed = obj.Speed,
			RackSpeed = obj.RackSpeed,
			Weight = obj.Weight,
			Flags = obj.Flags,
			ShipWakeSpacing = obj.ShipWakeSpacing,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			DrivingSoundType = obj.DrivingSoundType,
			NumSimultaneousCargoTypes = obj.NumSimultaneousCargoTypes,
			CarComponents = JsonSerializer.Serialize(obj.CarComponents),
			BodySprites = JsonSerializer.Serialize(obj.BodySprites),
			BogieSprites = JsonSerializer.Serialize(obj.BogieSprites),
			MaxCargo = JsonSerializer.Serialize(obj.MaxCargo),
			CompatibleCargoCategories = JsonSerializer.Serialize(obj.CompatibleCargoCategories),
			CargoTypeSpriteOffsets = JsonSerializer.Serialize(obj.CargoTypeSpriteOffsets),
			CompatibleVehicles = JsonSerializer.Serialize(obj.CompatibleVehicles),
			RequiredTrackExtras = JsonSerializer.Serialize(obj.RequiredTrackExtras),
			StartSounds = JsonSerializer.Serialize(obj.StartSounds),
			ParticleEmitters = JsonSerializer.Serialize(obj.ParticleEmitters),
			RackRail = JsonSerializer.Serialize(obj.RackRail),
			Sound = JsonSerializer.Serialize(obj.Sound),
			FrictionSound = JsonSerializer.Serialize(obj.FrictionSound),
			SimpleMotorSound = JsonSerializer.Serialize(obj.SimpleMotorSound),
			GearboxMotorSound = JsonSerializer.Serialize(obj.GearboxMotorSound),
			var_135 = JsonSerializer.Serialize(obj.var_135),
		};
}