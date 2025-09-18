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

	public VehicleViewModel(VehicleObject vo)
	{
		Mode = vo.Mode;
		Type = vo.Type;
		NumCarComponents = vo.NumCarComponents;
		TrackType = vo.TrackType == null ? null : new(vo.TrackType);
		CostIndex = vo.CostIndex;
		CostFactor = vo.CostFactor;
		Reliability = vo.Reliability;
		RunCostIndex = vo.RunCostIndex;
		RunCostFactor = vo.RunCostFactor;
		SpecialColourSchemeIndex = vo.SpecialColourSchemeIndex;
		CompatibleVehicles = Array.ConvertAll(vo.CompatibleVehicles, x => new ObjectModelHeaderViewModel(x));
		RequiredTrackExtras = Array.ConvertAll(vo.RequiredTrackExtras, x => new ObjectModelHeaderViewModel(x));
		CarComponents = [.. vo.CarComponents];
		BodySprites = [.. vo.BodySprites];
		BogieSprites = [.. vo.BogieSprites];
		Power = vo.Power;
		Speed = vo.Speed;
		RackSpeed = vo.RackSpeed;
		Weight = vo.Weight;
		Flags = vo.Flags;
		CompatibleCargo1 = new(vo.MaxCargo[0], [.. vo.CompatibleCargoCategories[0]]);
		CompatibleCargo2 = new(vo.MaxCargo[1], [.. vo.CompatibleCargoCategories[1]]);
		CargoTypeSpriteOffsets = new([.. vo.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value))]);
		Animation = [.. vo.Animation];
		AnimationHeaders = [.. vo.AnimationHeaders.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		ShipWakeOffset = vo.ShipWakeOffset;
		DesignedYear = vo.DesignedYear;
		ObsoleteYear = vo.ObsoleteYear;
		RackRail = vo.RackRail == null ? null : new(vo.RackRail);
		Sound = vo.Sound == null ? null : new(vo.Sound);
		StartSounds = Array.ConvertAll(vo.StartSounds, x => new ObjectModelHeaderViewModel(x));
		SoundType = vo.DrivingSoundType;
		FrictionSound = vo.FrictionSound;
		SimpleMotorSound = vo.SimpleMotorSound;
		GearboxMotorSound = vo.GearboxMotorSound;
		var_135 = [.. vo.var_135];
	}

	public override VehicleObject CopyBackToModel()
	{
		var vo = new VehicleObject()
		{
			Mode = Mode,
			Type = Type,
			NumCarComponents = NumCarComponents,
			CarComponents = [.. CarComponents],
			BodySprites = [.. BodySprites],
			BogieSprites = [.. BogieSprites],
			TrackType = TrackType?.CopyBackToModel(),
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
			RackRail = RackRail?.CopyBackToModel(),
			Sound = Sound?.CopyBackToModel(),
			StartSounds = Array.ConvertAll(StartSounds, x => x.CopyBackToModel()),
			CompatibleVehicles = Array.ConvertAll(CompatibleVehicles, x => x.CopyBackToModel()),
			RequiredTrackExtras = Array.ConvertAll(RequiredTrackExtras, x => x.CopyBackToModel()),
			AnimationHeaders = AnimationHeaders.ToList().ConvertAll(x => x.CopyBackToModel()),
			Animation = [.. Animation],
			DrivingSoundType = SoundType,
			FrictionSound = FrictionSound,
			SimpleMotorSound = SimpleMotorSound,
			GearboxMotorSound = GearboxMotorSound,
			//NumCompatibleVehicles = (uint8_t)CompatibleVehicles.Count,
			//NumRequiredTrackExtras = (uint8_t)RequiredTrackExtras.Count,
			//NumStartSounds = (uint8_t)StartSounds.Count,
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
