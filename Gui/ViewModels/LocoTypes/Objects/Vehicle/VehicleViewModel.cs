using Common;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using DynamicData.Binding;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.LocoTypes.Objects.Vehicle;

public class VehicleComponentsViewModel : ReactiveObject
{

}

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
		CompatibleCargo1 = new(model.CompatibleCargoCategories[0], model.MaxCargo[0]);
		CompatibleCargo2 = new(model.CompatibleCargoCategories[1], model.MaxCargo[1]);
		CargoTypeSpriteOffsets = new([.. model.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value))]);
		StartSounds = new(model.StartSounds);
		var_135 = new(model.var_135);
		RoadOrTrackType = model.RoadOrTrackType;
		RackRail = model.RackRail;

		SimpleMotorSound = model.SimpleMotorSound ?? new SimpleMotorSound();
		FrictionSound = model.FrictionSound ?? new FrictionSound();
		GearboxMotorSound = model.GearboxMotorSound ?? new GearboxMotorSound();
		Sound = model.Sound;

		#region Road/Track Type Binding

		//_ = this.WhenAnyValue(x => x.Mode, x => x.Flags)
		//	.Subscribe((_) => this.RaisePropertyChanged(nameof(AnyRoadOrTrackType)));

		//_ = this.WhenAnyValue(x => x.AnyRoadOrTrackType)
		//	.Subscribe((_) => this.RaisePropertyChanged(nameof(RoadOrTrackType)));

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

		#region Cargo Category Synchronization

		CompatibleCargo1.CargoCategories.CollectionChanged += OnCompatibleCargo1Changed;
		CompatibleCargo2.CargoCategories.CollectionChanged += OnCompatibleCargo2Changed;

		// Subscribe to existing items
		foreach (var item in CompatibleCargo1.CargoCategories)
		{
			item.PropertyChanged += OnCargoCategoryPropertyChanged;
		}
		foreach (var item in CompatibleCargo2.CargoCategories)
		{
			item.PropertyChanged += OnCargoCategoryPropertyChanged;
		}

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

	[EnumProhibitValues<VehicleObjectFlags>(VehicleObjectFlags.None, VehicleObjectFlags.RackRail, VehicleObjectFlags.AnyRoadType)]
	public VehicleObjectFlags Flags
	{
		get => model.Flags;
		set
		{
			model.Flags = value;
			this.RaisePropertyChanged(nameof(Flags));
		}
	}

	[ConditionTarget]
	//bool IsTrackTypeSettable
	//	=> !model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType) && (model.Mode == TransportMode.Rail || model.Mode == TransportMode.Road);
	public bool AnyRoadOrTrackType
	{
		get => model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType);
		set
		{
			model.Flags = model.Flags.ToggleFlag(VehicleObjectFlags.AnyRoadType, value);

			if (RoadOrTrackType == null && model.Flags.HasFlag(VehicleObjectFlags.AnyRoadType))
			{
				RoadOrTrackType = new ObjectModelHeader() { Name = "<obj>", ObjectSource = ObjectSource.Custom, ObjectType = ObjectType.Road };
			}

			this.RaisePropertyChanged(nameof(RoadOrTrackType));
		}
	}
	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(AnyRoadOrTrackType), false)]
	public ObjectModelHeader? RoadOrTrackType
	{
		get => model.RoadOrTrackType;
		set => model.RoadOrTrackType = value;
	}

	[ConditionTarget]
	public bool HasRackRail
	{
		get => model.Flags.HasFlag(VehicleObjectFlags.RackRail);
		set
		{
			model.Flags = model.Flags.ToggleFlag(VehicleObjectFlags.RackRail, value);

			if (RackRail == null && model.Flags.HasFlag(VehicleObjectFlags.RackRail))
			{
				RackRail = new ObjectModelHeader() { Name = "<obj>", ObjectSource = ObjectSource.Custom, ObjectType = ObjectType.TrackExtra };
			}

			this.RaisePropertyChanged(nameof(RackRail));
		}
	}

	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(HasRackRail), true)]
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
	public BindingList<ObjectModelHeader> CompatibleVehicles { get; init; }

	[Length(0, 4)]
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
		set
		{
			Model.CostIndex = value;
			this.RaisePropertyChanged(nameof(CostFactor));
		}
	}

	[Category("Cost")]
	[Range(1, int16_t.MaxValue), InflatableCurrency(nameof(CostIndex), nameof(DesignedYear))]
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
		set
		{
			Model.RunCostIndex = value;
			this.RaisePropertyChanged(nameof(RunCostFactor));
		}
	}

	[Category("Cost")]
	[Range(0, int16_t.MaxValue), InflatableCurrency(nameof(RunCostIndex), nameof(DesignedYear))]
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
	public BindingList<VehicleObjectCar> CarComponents { get; init; }

	[Category("Sprites")]
	[EnumProhibitValues<BodySpriteFlags>(BodySpriteFlags.None)]
	public BindingList<BodySprite> BodySprites { get; init; }

	[Category("Sprites")]
	[EnumProhibitValues<BogieSpriteFlags>(BogieSpriteFlags.None)]
	public BindingList<BogieSprite> BogieSprites { get; init; }

	[Category("Sprites")]
	public BindingList<EmitterAnimation> Animation { get; init; }

	[Category("Cargo")]
	public CompatibleCargo CompatibleCargo1 { get; init; }

	[Category("Cargo")]
	public CompatibleCargo CompatibleCargo2 { get; init; }

	[Category("Cargo")]
	[Length(0, 32)]
	[Description("This is a dictionary. For every cargo defined in both CompatibleCargoCategories, an entry must exist in this dictionary.")]
	public ObservableCollection<CargoTypeSpriteOffset> CargoTypeSpriteOffsets { get; init; }

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
	[PropertyVisibilityCondition(nameof(IsDrivingSoundTypeSet), true)]
	public ObjectModelHeader? Sound { get; set; }

	[Category("Sound")]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.Friction)]
	public FrictionSound FrictionSound { get; set; }

	[Category("Sound")]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.SimpleMotor)]
	public SimpleMotorSound SimpleMotorSound { get; set; }

	[Category("Sound")]
	[ConditionTarget]
	[PropertyVisibilityCondition(nameof(DrivingSoundType), DrivingSoundType.GearboxMotor)]
	public GearboxMotorSound GearboxMotorSound { get; set; }

	[Category("Sound")]
	[Description("The sound the vehicle makes when starting or crossing a rail crossing. Essentially it's \"horn\"")]
	public BindingList<ObjectModelHeader> StartSounds { get; init; }

	[Category("<unknown>")]
	public BindingList<uint8_t> var_135 { get; init; }

	void OnCompatibleCargo1Changed(object? sender, NotifyCollectionChangedEventArgs e)
	{
		HandleCargoCollectionPropertySubscriptions(e);
		SynchronizeCargoTypeSpriteOffsets(e);
	}

	void OnCompatibleCargo2Changed(object? sender, NotifyCollectionChangedEventArgs e)
	{
		HandleCargoCollectionPropertySubscriptions(e);
		SynchronizeCargoTypeSpriteOffsets(e);
	}

	void HandleCargoCollectionPropertySubscriptions(NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
		{
			foreach (CargoCategoryViewModel item in e.NewItems)
			{
				item.PropertyChanged += OnCargoCategoryPropertyChanged;
			}
		}
		else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
		{
			foreach (CargoCategoryViewModel item in e.OldItems)
			{
				item.PropertyChanged -= OnCargoCategoryPropertyChanged;
			}
		}
		else if (e.Action == NotifyCollectionChangedAction.Replace)
		{
			if (e.OldItems != null)
			{
				foreach (CargoCategoryViewModel item in e.OldItems)
				{
					item.PropertyChanged -= OnCargoCategoryPropertyChanged;
				}
			}
			if (e.NewItems != null)
			{
				foreach (CargoCategoryViewModel item in e.NewItems)
				{
					item.PropertyChanged += OnCargoCategoryPropertyChanged;
				}
			}
		}
	}

	void OnCargoCategoryPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(CargoCategoryViewModel.Category) && sender is CargoCategoryViewModel changedItem)
		{
			SynchronizeCategoryChange(changedItem);
		}
	}

	void SynchronizeCategoryChange(CargoCategoryViewModel changedItem)
	{
		// Find if there's an existing offset entry that needs updating
		var existingOffset = CargoTypeSpriteOffsets.FirstOrDefault(x => x.CargoCategoryViewModel.Category == changedItem.Category);
		if (existingOffset == null)
		{
			CargoTypeSpriteOffsets.Add(new CargoTypeSpriteOffset(changedItem.Category, 0));
		}

		// remove anything from CargoTypeSpriteOffsets that has a Category no longer present in either CompatibleCargo1 or CompatibleCargo2
		var categoriesInUse = CompatibleCargo1.CargoCategories.Select(x => x.Category)
			.Concat(CompatibleCargo2.CargoCategories.Select(x => x.Category))
			.Distinct();

		var offsetsToRemove = CargoTypeSpriteOffsets.Where(x => !categoriesInUse.Contains(x.CargoCategoryViewModel.Category)).ToList();
		foreach (var offset in offsetsToRemove)
		{
			_ = CargoTypeSpriteOffsets.Remove(offset);
		}
	}

	void SynchronizeCargoTypeSpriteOffsets(NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
		{
			foreach (CargoCategoryViewModel item in e.NewItems)
			{
				var existingOffset = CargoTypeSpriteOffsets.FirstOrDefault(x => x.CargoCategoryViewModel.Category == item.Category);
				if (existingOffset == null)
				{
					CargoTypeSpriteOffsets.Add(new CargoTypeSpriteOffset(item.Category, 0));
				}
			}
		}
		else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
		{
			foreach (CargoCategoryViewModel item in e.OldItems)
			{
				var offsetToRemove = CargoTypeSpriteOffsets.FirstOrDefault(x => x.CargoCategoryViewModel.Category == item.Category);
				if (offsetToRemove != null)
				{
					var isStillUsed = CompatibleCargo1.CargoCategories.Any(x => x.Category == item.Category)
						|| CompatibleCargo2.CargoCategories.Any(x => x.Category == item.Category);
					
					if (!isStillUsed)
					{
						_ = CargoTypeSpriteOffsets.Remove(offsetToRemove);
					}
				}
			}
		}
	}

	public override void CopyBackToModel()
	{
		// this should be done with the reactive properties, but for now we'll leave it like this
		Model.MaxCargo = [CompatibleCargo1.MaxCargo, CompatibleCargo2.MaxCargo];
		Model.CompatibleCargoCategories =
		[
			[.. CompatibleCargo1.CargoCategories.Select(x => x.Category).ToArray()],
			[.. CompatibleCargo2.CargoCategories.Select(x => x.Category).ToArray()]
		];

		Model.NumSimultaneousCargoTypes += (byte)(CompatibleCargo1.CargoCategories.Count > 0 ? 1 : 0);
		Model.NumSimultaneousCargoTypes += (byte)(CompatibleCargo2.CargoCategories.Count > 0 ? 1 : 0);

		foreach (var ctso in CargoTypeSpriteOffsets)
		{
			Model.CargoTypeSpriteOffsets[ctso.CargoCategoryViewModel.Category] = ctso.Offset;
		}
	}
}

// todo: use this in CargoObject
[TypeConverter(typeof(ExpandableObjectConverter))]
public class CargoCategoryViewModel : ReactiveUI.ReactiveObject
{
	public CargoCategory Category
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				this.RaisePropertyChanged(nameof(Category));
				this.RaisePropertyChanged(nameof(Override));
			}
		}
	}

	public uint16_t Override
	{
		get => (uint16_t)Category;
		set
		{
			if ((uint16_t)Category != value)
			{
				Category = (CargoCategory)value;
				this.RaisePropertyChanged(nameof(Category));
				this.RaisePropertyChanged(nameof(Override));
			}
		}
	}

	public CargoCategoryViewModel(CargoCategory cargoCategory)
		=> Category = cargoCategory;

	public CargoCategoryViewModel()
		: this(CargoCategory.NULL)
	{ }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CargoTypeSpriteOffset : ReactiveUI.ReactiveObject
{
	public CargoTypeSpriteOffset(CargoCategory cargoCategory, uint8_t offset)
	{
		CargoCategoryViewModel = new CargoCategoryViewModel(cargoCategory);
		Offset = offset;
	}

	public CargoTypeSpriteOffset()
		: this(CargoCategory.NULL, 0)
	{ }

	public CargoCategoryViewModel CargoCategoryViewModel { get; set; }

	public byte Offset { get; set; }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CompatibleCargo : ReactiveUI.ReactiveObject
{
	public byte MaxCargo { get; set; }
	public ObservableCollection<CargoCategoryViewModel> CargoCategories { get; init; }

	public CompatibleCargo(IEnumerable<CargoCategory> compatibleCargoCategories, uint8_t maxCargo)
	{
		MaxCargo = maxCargo;
		CargoCategories = new(compatibleCargoCategories.Select(x => new CargoCategoryViewModel(x)));
	}

	public CompatibleCargo()
		: this([], 0)
	{ }
}
