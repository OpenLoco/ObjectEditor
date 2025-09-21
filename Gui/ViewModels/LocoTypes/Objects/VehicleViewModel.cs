using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Vehicle;
using PropertyModels.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class VehicleViewModel : LocoObjectViewModel<VehicleObject>
{
	[Category("Stats")] public TransportMode Mode { get; set; }
	[Category("Stats")] public VehicleType Type { get; set; }
	[Category("Stats")] public uint16_t Weight { get; set; }
	[Category("Stats")] public uint16_t Power { get; set; }
	[Category("Stats")] public Speed16 Speed { get; set; }
	[Category("Stats"), Description("Also used for Aircraft as their broken-down speed, landing speed, and approaching speed")] public Speed16 RackSpeed { get; set; }
	[Category("Stats")] public uint8_t RackRailType { get; set; }
	[Category("Stats")] public uint16_t DesignedYear { get; set; }
	[Category("Stats")] public uint16_t ObsoleteYear { get; set; }
	[Category("Stats")] public uint8_t Reliability { get; set; }
	[EnumProhibitValues<VehicleObjectFlags>(VehicleObjectFlags.None)] public VehicleObjectFlags Flags { get; set; }
	public ObjectModelHeaderViewModel? TrackType { get; set; }
	public ObjectModelHeaderViewModel? RackRail { get; set; }
	[Range(0, 4)] public uint8_t NumCarComponents { get; set; }
	[Length(0, 8)] public ObjectModelHeaderViewModel[] CompatibleVehicles { get; set; }
	[Length(0, 4)] public ObjectModelHeaderViewModel[] RequiredTrackExtras { get; set; }
	[Description("If 0, boat has a single wake animation. if > 0, boat has 2 wakes, offset horizontally by this value")] public uint8_t ShipWakeOffset { get; set; }
	[Category("Cost"), Range(0, 32)] public uint8_t CostIndex { get; set; }
	[Category("Cost"), Range(1, int16_t.MaxValue)] public int16_t CostFactor { get; set; }
	[Category("Cost"), Range(0, 32)] public uint8_t RunCostIndex { get; set; }
	[Category("Cost"), Range(0, int16_t.MaxValue)] public int16_t RunCostFactor { get; set; }
	[Category("Sprites")] public CompanyColourType SpecialColourSchemeIndex { get; set; } // called "ColourType" in the loco codebase
	[Category("Sprites"), Editable(false)] public VehicleObjectCar[] CarComponents { get; set; }
	[Category("Sprites"), Editable(false)] public BodySprite[] BodySprites { get; set; }
	[Category("Sprites"), Editable(false)] public BogieSprite[] BogieSprites { get; set; }
	[Category("Sprites"), Editable(false)] public SimpleAnimation[] Animation { get; set; }
	[Category("Sprites")] public ObjectModelHeaderViewModel[] AnimationHeaders { get; set; }
	//[Category("Cargo")] public ObservableCollection<uint8_t> MaxCargo { get; set; }
	//[Category("Cargo")] public ObservableCollection<CargoCategory> CompatibleCargoCategories1 { get; set; }
	//[Category("Cargo")] public ObservableCollection<CargoCategory> CompatibleCargoCategories2 { get; set; }
	[Category("Cargo")] public CompatibleCargo CompatibleCargo1 { get; set; }
	[Category("Cargo")] public CompatibleCargo CompatibleCargo2 { get; set; }
	[Category("Cargo"), Length(0, 32)] public List<CargoTypeSpriteOffset> CargoTypeSpriteOffsets { get; set; } // this is a dictionary type
	[Category("Sound")] public ObjectModelHeaderViewModel? Sound { get; set; }
	[Category("Sound")] public DrivingSoundType SoundType { get; set; }
	// SoundPropertiesData
	// these next 3 properties are a union in the dat file
	[Category("Sound")] public FrictionSound? FrictionSound { get; set; }
	[Category("Sound")] public SimpleMotorSound? SimpleMotorSound { get; set; }
	[Category("Sound")] public GearboxMotorSound? GearboxMotorSound { get; set; }
	[Category("Sound")] public ObjectModelHeaderViewModel[] StartSounds { get; set; }
	[Category("<unknown>")] public uint8_t[] var_135 { get; set; } = [];

	public VehicleViewModel(VehicleObject model)
		: base(model)
	{
		Mode = model.Mode;
		Type = model.Type;
		NumCarComponents = model.NumCarComponents;
		TrackType = model.TrackType == null ? null : new(model.TrackType);
		CostIndex = model.CostIndex;
		CostFactor = model.CostFactor;
		Reliability = model.Reliability;
		RunCostIndex = model.RunCostIndex;
		RunCostFactor = model.RunCostFactor;
		SpecialColourSchemeIndex = model.SpecialColourSchemeIndex;
		CompatibleVehicles = Array.ConvertAll(model.CompatibleVehicles, x => new ObjectModelHeaderViewModel(x));
		RequiredTrackExtras = Array.ConvertAll(model.RequiredTrackExtras, x => new ObjectModelHeaderViewModel(x));
		CarComponents = [.. model.CarComponents];
		BodySprites = [.. model.BodySprites];
		BogieSprites = [.. model.BogieSprites];
		Power = model.Power;
		Speed = model.Speed;
		RackSpeed = model.RackSpeed;
		Weight = model.Weight;
		Flags = model.Flags;
		CompatibleCargo1 = new(model.MaxCargo[0], [.. model.CompatibleCargoCategories[0]]);
		CompatibleCargo2 = new(model.MaxCargo[1], [.. model.CompatibleCargoCategories[1]]);
		CargoTypeSpriteOffsets = new([.. model.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value))]);
		Animation = [.. model.Animation];
		AnimationHeaders = [.. model.AnimationHeaders.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		ShipWakeOffset = model.ShipWakeOffset;
		DesignedYear = model.DesignedYear;
		ObsoleteYear = model.ObsoleteYear;
		RackRail = model.RackRail == null ? null : new(model.RackRail);
		Sound = model.Sound == null ? null : new(model.Sound);
		StartSounds = Array.ConvertAll(model.StartSounds, x => new ObjectModelHeaderViewModel(x));
		SoundType = model.DrivingSoundType;
		FrictionSound = model.FrictionSound;
		SimpleMotorSound = model.SimpleMotorSound;
		GearboxMotorSound = model.GearboxMotorSound;
		var_135 = [.. model.var_135];
	}

	public VehicleObject CopyBackToModel()
	{
		var vo = new VehicleObject()
		{
			Mode = Mode,
			Type = Type,
			NumCarComponents = NumCarComponents,
			CarComponents = [.. CarComponents],
			BodySprites = [.. BodySprites],
			BogieSprites = [.. BogieSprites],
			//TrackType = TrackType?.CopyBackToModel(),
			CostIndex = CostIndex,
			CostFactor = CostFactor,
			Reliability = Reliability,
			RunCostIndex = RunCostIndex,
			RunCostFactor = RunCostFactor,
			SpecialColourSchemeIndex = SpecialColourSchemeIndex,
			Power = Power,
			Speed = Speed,
			RackSpeed = RackSpeed,
			Weight = Weight,
			Flags = Flags,
			ShipWakeOffset = ShipWakeOffset,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			//RackRail = RackRail?.CopyBackToModel(),
			//Sound = Sound?.CopyBackToModel(),
			//StartSounds = Array.ConvertAll(StartSounds, x => x.CopyBackToModel()),
			//CompatibleVehicles = Array.ConvertAll(CompatibleVehicles, x => x.CopyBackToModel()),
			//RequiredTrackExtras = Array.ConvertAll(RequiredTrackExtras, x => x.CopyBackToModel()),
			//AnimationHeaders = AnimationHeaders.ToList().ConvertAll(x => x.CopyBackToModel()),
			Animation = [.. Animation],
			DrivingSoundType = SoundType,
			FrictionSound = FrictionSound,
			SimpleMotorSound = SimpleMotorSound,
			GearboxMotorSound = GearboxMotorSound,
			MaxCargo = [CompatibleCargo1.MaxCargo, CompatibleCargo2.MaxCargo],
			CompatibleCargoCategories = [
				[.. CompatibleCargo1.CompatibleCargoCategories.ToArray()],
				[.. CompatibleCargo2.CompatibleCargoCategories.ToArray()]
			],
			var_135 = [.. var_135], // purpose unknown
		};

		vo.NumSimultaneousCargoTypes += (byte)(CompatibleCargo1.CompatibleCargoCategories.Count > 0 ? 1 : 0);
		vo.NumSimultaneousCargoTypes += (byte)(CompatibleCargo2.CompatibleCargoCategories.Count > 0 ? 1 : 0);

		foreach (var ctso in CargoTypeSpriteOffsets)
		{
			vo.CargoTypeSpriteOffsets[ctso.CargoCategory] = ctso.Offset;
		}

		return vo;
	}
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public record CargoTypeSpriteOffset(CargoCategory CargoCategory, uint8_t Offset)
{
	public CargoTypeSpriteOffset() : this(CargoCategory.NULL, 0)
	{ }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public record CompatibleCargo(uint8_t MaxCargo, List<CargoCategory> CompatibleCargoCategories);
