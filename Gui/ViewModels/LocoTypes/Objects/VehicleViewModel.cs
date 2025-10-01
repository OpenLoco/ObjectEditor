using Common;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using DynamicData.Binding;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class VehicleViewModel : LocoObjectViewModel<VehicleObject>
{
	private readonly VehicleObject model;

	public VehicleViewModel(VehicleObject model) : base(model)
	{
		this.model = model;
		CompatibleVehicles = new(model.CompatibleVehicles);
		RequiredTrackExtras = new(model.RequiredTrackExtras);
		CarComponents = new(model.CarComponents);
		BodySprites = new(model.BodySprites);
		BogieSprites = new(model.BogieSprites);
		Animation = new(model.ParticleEmitters);
		CompatibleCargo1 = new(model.MaxCargo[0], new(model.CompatibleCargoCategories[0]));
		CompatibleCargo2 = new(model.MaxCargo[1], new(model.CompatibleCargoCategories[1]));
		CargoTypeSpriteOffsets = new([.. model.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value))]);
		StartSounds = new(model.StartSounds);
		var_135 = new(model.var_135);
		RoadOrTrackType = model.RoadOrTrackType;
		RackRail = model.RackRail;

		HasRackRail = model.Flags.HasFlag(VehicleObjectFlags.RackRail);

		SimpleMotorSound = model.SimpleMotorSound ?? new SimpleMotorSound();
		FrictionSound = model.FrictionSound ?? new FrictionSound();
		GearboxMotorSound = model.GearboxMotorSound ?? new GearboxMotorSound();
		Sound = model.Sound;

		_ = this.WhenAnyValue(x => x.RackRail)
			.Subscribe((_) => model.RackRail = RackRail);

		#region Road/Track Type Binding

		_ = this.WhenAnyValue(x => x.Mode, x => x.Flags)
			.Subscribe((_) => this.RaisePropertyChanged(nameof(IsTrackTypeSettable)));

		_ = this.WhenAnyValue(x => x.IsTrackTypeSettable)
			.Subscribe((_) => this.RaisePropertyChanged(nameof(RoadOrTrackType)));

		_ = this.WhenAnyValue(x => x.RoadOrTrackType)
			.Subscribe((_) => model.RoadOrTrackType = RoadOrTrackType);

		#endregion

		#region Sound Properties Binding
		_ = this.WhenAnyValue(x => x.DrivingSoundType)
			.Subscribe((_) =>
			{
				this.RaisePropertyChanged(nameof(SimpleMotorSound));
				this.RaisePropertyChanged(nameof(FrictionSound));
				this.RaisePropertyChanged(nameof(GearboxMotorSound));
			});

		_ = this.WhenAnyValue(x => x.Sound)
			.Subscribe((_) => model.Sound = Sound);

		_ = this.WhenAnyValue(x => x.SimpleMotorSound)
			.Subscribe((_) => model.SimpleMotorSound = SimpleMotorSound);
		_ = this.WhenAnyValue(x => x.FrictionSound)
			.Subscribe((_) => model.FrictionSound = FrictionSound);
		_ = this.WhenAnyValue(x => x.GearboxMotorSound)
			.Subscribe((_) => model.GearboxMotorSound = GearboxMotorSound);

		_ = this.WhenPropertyChanged(x => x.SimpleMotorSound)
			.Subscribe((_) => model.SimpleMotorSound = SimpleMotorSound);
		_ = this.WhenPropertyChanged(x => x.FrictionSound)
			.Subscribe((_) => model.FrictionSound = FrictionSound);
		_ = this.WhenPropertyChanged(x => x.GearboxMotorSound)
			.Subscribe((_) => model.GearboxMotorSound = GearboxMotorSound);

		#endregion
	}

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

	[Category("Stats"), Description("Also used for Aircraft as their broken-down speed, landing speed, and approaching speed")]
	public Speed16 RackSpeed
	{
		get => model.RackSpeed;
		set => model.RackSpeed = value;
	}

	[EnumProhibitValues<VehicleObjectFlags>(VehicleObjectFlags.None, VehicleObjectFlags.RackRail)]
	public VehicleObjectFlags Flags
	{
		get => model.Flags;
		set
		{
			model.Flags = value;
			this.RaisePropertyChanged(nameof(Flags));
		}
	}

	[Browsable(false)]
	bool IsTrackTypeSettable
		=> !model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType) && (model.Mode == TransportMode.Rail || model.Mode == TransportMode.Road);

	[Reactive]
	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(IsTrackTypeSettable), true)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public ObjectModelHeader? RoadOrTrackType { get; set; }

	[ConditionTarget]
	public bool HasRackRail
	{
		get => model.Flags.HasFlag(VehicleObjectFlags.RackRail);
		set
		{
			model.Flags = model.Flags.ToggleFlag(VehicleObjectFlags.RackRail, value);
			RackRail = value && RackRail == null
				? new ObjectModelHeader("<obj>", ObjectType.TrackExtra, ObjectSource.OpenLoco, 0)
				: null;
		}
	}

	[Reactive]
	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(HasRackRail), true)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public ObjectModelHeader? RackRail { get; set; }

	[Range(0, 4)]
	public uint8_t NumCarComponents
	{
		get => model.NumCarComponents;
		set => model.NumCarComponents = value;
	}

	[Length(0, 8)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<ObjectModelHeader> CompatibleVehicles { get; init; }

	[Length(0, 4)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<ObjectModelHeader> RequiredTrackExtras { get; init; }

	[Description("If 0, boat has a single wake animation. if > 0, boat has 2 wakes, offset horizontally by this value")]
	public uint8_t ShipWakeSpacing
	{
		get => model.ShipWakeSpacing;
		set => model.ShipWakeSpacing = value;
	}

	[Category("Cost")]
	[Range(0, 32)]
	public uint8_t CostIndex
	{
		get => model.CostIndex;
		set => model.CostIndex = value;
	}

	[Category("Cost")]
	[Range(1, int16_t.MaxValue)]
	public int16_t CostFactor
	{
		get => model.CostFactor;
		set => model.CostFactor = value;
	}

	[Category("Cost")]
	[Range(0, 32)]
	public uint8_t RunCostIndex
	{
		get => model.RunCostIndex;
		set => model.RunCostIndex = value;
	}

	[Category("Cost")]
	[Range(0, int16_t.MaxValue)]
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

	[Category("Sprites")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<VehicleObjectCar> CarComponents { get; init; }

	[Category("Sprites")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<BodySprite> BodySprites { get; init; }

	[Category("Sprites")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<BogieSprite> BogieSprites { get; init; }

	[Category("Sprites")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<EmitterAnimation> Animation { get; init; }

	[Category("Cargo")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public CompatibleCargo CompatibleCargo1 { get; init; }

	[Category("Cargo")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public CompatibleCargo CompatibleCargo2 { get; init; }

	[Category("Cargo")]
	[Length(0, 32)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	[Description("This is a dictionary. For every cargo defined in both CompatibleCargoCategories, an entry must exist in this dictionary.")]
	public BindingList<CargoTypeSpriteOffset> CargoTypeSpriteOffsets { get; init; }

	[Category("Sound")]
	[ConditionTarget]
	public DrivingSoundType DrivingSoundType
	{
		get => model.DrivingSoundType;
		set
		{
			model.DrivingSoundType = value;
			this.RaisePropertyChanged(nameof(DrivingSoundType));
		}
	}

	[Browsable(false)]
	[DependsOnProperty(nameof(DrivingSoundType))]
	[ConditionTarget]
	bool IsDrivingSoundTypeSet
		=> model.DrivingSoundType != DrivingSoundType.None;

	[Category("Sound")]
	[Reactive]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	[PropertyVisibilityCondition(nameof(IsDrivingSoundTypeSet), true)]
	public ObjectModelHeader? Sound { get; set; }

	[Category("Sound")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.Friction)]
	public FrictionSound FrictionSound { get; set; }

	[Category("Sound")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.SimpleMotor)]
	public SimpleMotorSound SimpleMotorSound { get; set; }

	[Category("Sound")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.GearboxMotor)]
	public GearboxMotorSound GearboxMotorSound { get; set; }

	[Category("Sound")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<ObjectModelHeader> StartSounds { get; init; }

	[Category("<unknown>")]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<uint8_t> var_135 { get; init; }

	public override void CopyBackToModel()
	{
		// this should be done with the reactive properties, but for now we'll leave it like this
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
