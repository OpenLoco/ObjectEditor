using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class VehicleViewModel(VehicleObject model)
	: LocoObjectViewModel<VehicleObject>(model)
{
	[Category("Stats")]
	public TransportMode Mode
	{
		get => model.Mode;
		set => model.Mode = value;
	}

	[Category("Stats")]
	public VehicleType Type
	{
		get => model.Type;
		set => model.Type = value;
	}

	[Category("Stats")]
	public uint16_t Weight
	{
		get => model.Weight;
		set => model.Weight = value;
	}

	[Category("Stats")]
	public uint16_t Power
	{
		get => model.Power;
		set => model.Power = value;
	}

	[Category("Stats")]
	public Speed16 Speed
	{
		get => model.Speed;
		set => model.Speed = value;
	}

	[Category("Stats"), Description("Also used for Aircraft as their broken-down speed, landing speed, and approaching speed")]
	public Speed16 RackSpeed
	{
		get => model.RackSpeed;
		set => model.RackSpeed = value;
	}

	[Category("Stats")]
	public uint16_t DesignedYear
	{
		get => model.DesignedYear;
		set => model.DesignedYear = value;
	}

	[Category("Stats")]
	public uint16_t ObsoleteYear
	{
		get => model.ObsoleteYear;
		set => model.ObsoleteYear = value;
	}

	[Category("Stats")]
	public uint8_t Reliability
	{
		get => model.Reliability;
		set => model.Reliability = value;
	}

	[EnumProhibitValues<VehicleObjectFlags>(VehicleObjectFlags.None)]
	public VehicleObjectFlags Flags
	{
		get => model.Flags;
		set => model.Flags = value;
	}

	public ObjectModelHeader? TrackType
	{
		get => model.TrackType;
		set => model.TrackType = value;
	}

	public ObjectModelHeader? RackRail
	{
		get => model.RackRail;
		set => model.RackRail = value;
	}

	[Range(0, 4)]
	public uint8_t NumCarComponents
	{
		get => model.NumCarComponents;
		set => model.NumCarComponents = value;
	}

	[Length(0, 8)]
	public BindingList<ObjectModelHeader> CompatibleVehicles { get; init; } = new(model.CompatibleVehicles);

	[Length(0, 4)]
	public BindingList<ObjectModelHeader> RequiredTrackExtras { get; init; } = new(model.RequiredTrackExtras);

	[Description("If 0, boat has a single wake animation. if > 0, boat has 2 wakes, offset horizontally by this value")]
	public uint8_t ShipWakeOffset
	{
		get => model.ShipWakeOffset;
		set => model.ShipWakeOffset = value;
	}

	[Category("Cost"), Range(0, 32)]
	public uint8_t CostIndex
	{
		get => model.CostIndex;
		set => model.CostIndex = value;
	}

	[Category("Cost"), Range(1, int16_t.MaxValue)]
	public int16_t CostFactor
	{
		get => model.CostFactor;
		set => model.CostFactor = value;
	}

	[Category("Cost"), Range(0, 32)]
	public uint8_t RunCostIndex
	{
		get => model.RunCostIndex;
		set => model.RunCostIndex = value;
	}

	[Category("Cost"), Range(0, int16_t.MaxValue)]
	public int16_t RunCostFactor
	{
		get => model.RunCostFactor;
		set => model.RunCostFactor = value;
	}

	[Category("Sprites")]
	public CompanyColourType SpecialColourSchemeIndex
	{
		get => model.CompanyColourSchemeIndex;
		set => model.CompanyColourSchemeIndex = value;
	} // called "ColourType" in the loco codebase

	[Category("Sprites"), Editable(false)] public BindingList<VehicleObjectCar> CarComponents { get; init; } = new(model.CarComponents);
	[Category("Sprites"), Editable(false)] public BindingList<BodySprite> BodySprites { get; init; } = new(model.BodySprites);
	[Category("Sprites"), Editable(false)] public BindingList<BogieSprite> BogieSprites { get; init; } = new(model.BogieSprites);
	[Category("Sprites"), Editable(false)] public BindingList<EmitterAnimation> Animation { get; init; } = new(model.ParticleEmitters);

	[Category("Cargo")]
	public CompatibleCargo CompatibleCargo1 { get; init; } = new(model.MaxCargo[0], new(model.CompatibleCargoCategories[0]));

	[Category("Cargo")]
	public CompatibleCargo CompatibleCargo2 { get; init; } = new(model.MaxCargo[1], new(model.CompatibleCargoCategories[1]));

	[Category("Cargo"), Length(0, 32), Description("This is a dictionary. For every cargo defined in both CompatibleCargoCategories, an entry must exist in this dictionary.")]
	public BindingList<CargoTypeSpriteOffset> CargoTypeSpriteOffsets { get; init; } = new([.. model.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value))]);

	[Category("Sound")]
	public ObjectModelHeader? Sound
	{
		get => model.Sound;
		set => model.Sound = value;
	}

	[Category("Sound")]
	public DrivingSoundType DrivingSoundType
	{
		get => model.DrivingSoundType;
		set => model.DrivingSoundType = value;
	}

	[Category("Sound")]
	public FrictionSound? FrictionSound
	{
		get => model.FrictionSound;
		set => model.FrictionSound = value;
	}

	[Category("Sound")]
	public SimpleMotorSound? SimpleMotorSound
	{
		get => model.SimpleMotorSound;
		set => model.SimpleMotorSound = value;
	}

	[Category("Sound")]
	public GearboxMotorSound? GearboxMotorSound
	{
		get => model.GearboxMotorSound;
		set => model.GearboxMotorSound = value;
	}

	[Category("Sound")]
	public BindingList<ObjectModelHeader> StartSounds { get; init; } = new(model.StartSounds);

	[Category("<unknown>")]
	public BindingList<uint8_t> var_135 { get; init; } = new(model.var_135);

	public override void CopyBackToModel()
	{
		Model.MaxCargo = [CompatibleCargo1.MaxCargo, CompatibleCargo2.MaxCargo];
		Model.CompatibleCargoCategories =
		[
			[.. CompatibleCargo1.CompatibleCargoCategories.ToArray()],
			[.. CompatibleCargo2.CompatibleCargoCategories.ToArray()]
		];

		Model.NumSimultaneousCargoTypes += (byte)(CompatibleCargo1.CompatibleCargoCategories.Count > 0 ? 1 : 0);
		Model.NumSimultaneousCargoTypes += (byte)(CompatibleCargo2.CompatibleCargoCategories.Count > 0 ? 1 : 0);

		foreach (var ctso in CargoTypeSpriteOffsets)
		{
			Model.CargoTypeSpriteOffsets[ctso.CargoCategory] = ctso.Offset;
		}
	}
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CargoTypeSpriteOffset(CargoCategory CargoCategory, uint8_t Offset)
{
	public CargoTypeSpriteOffset() : this(CargoCategory.NULL, 0)
	{ }

	public CargoCategory CargoCategory { get; set; } = CargoCategory;
	public byte Offset { get; set; } = Offset;
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CompatibleCargo(uint8_t MaxCargo, BindingList<CargoCategory> CompatibleCargoCategories)
{
	public byte MaxCargo { get; set; } = MaxCargo;
	public BindingList<CargoCategory> CompatibleCargoCategories { get; init; } = CompatibleCargoCategories;
}
