using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class VehicleViewModel : ReactiveObject, IObjectViewModel<ILocoStruct>
	{
		[Reactive, Category("Stats")] public TransportMode Mode { get; set; }
		[Reactive, Category("Stats")] public VehicleType Type { get; set; }
		[Reactive, Category("Stats")] public uint16_t Weight { get; set; }
		[Reactive, Category("Stats")] public uint16_t Power { get; set; }
		[Reactive, Category("Stats")] public Speed16 Speed { get; set; }
		[Reactive, Category("Stats")] public Speed16 RackSpeed { get; set; }
		[Reactive, Category("Stats")] public uint8_t RackRailType { get; set; }
		[Reactive, Category("Stats")] public uint16_t DesignedYear { get; set; }
		[Reactive, Category("Stats")] public uint16_t ObsoleteYear { get; set; }
		[Reactive, Category("Stats")] public uint8_t Reliability { get; set; }
		[Reactive] public VehicleObjectFlags Flags { get; set; }
		[Reactive] public object_id TrackTypeId { get; set; }
		[Reactive, Category("Cost"), Range(0, 32)] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Cost"), Range(1, int16_t.MaxValue)] public int16_t CostFactor { get; set; }
		[Reactive, Category("Cost"), Range(0, 32)] public uint8_t RunCostIndex { get; set; }
		[Reactive, Category("Cost"), Range(0, int16_t.MaxValue)] public int16_t RunCostFactor { get; set; }
		[Reactive, Length(0, 8)] public BindingList<S5Header> CompatibleVehicles { get; set; }
		[Reactive, Length(0, 4)] public BindingList<S5Header> RequiredTrackExtras { get; set; }
		[Reactive] public uint8_t var_04 { get; set; }
		[Reactive] public uint8_t var_113 { get; set; }
		[Reactive, Category("Sprites")] public uint8_t ColourType { get; set; }
		[Reactive, Category("Sprites"), Editable(false)] public BindingList<VehicleObjectCar> CarComponents { get; set; }
		[Reactive, Category("Sprites"), Editable(false)] public BindingList<BodySprite> BodySprites { get; set; }
		[Reactive, Category("Sprites"), Editable(false)] public BindingList<BogieSprite> BogieSprites { get; set; }
		[Reactive, Category("Sprites"), Editable(false)] public BindingList<SimpleAnimation> Animation { get; set; }
		[Reactive, Category("Sprites")] public BindingList<S5Header> AnimationHeaders { get; set; }
		[Reactive, Category("Cargo")] public BindingList<uint8_t> MaxCargo { get; set; }
		[Reactive, Category("Cargo")] public BindingList<CargoCategory> CompatibleCargoCategories1 { get; set; }
		[Reactive, Category("Cargo")] public BindingList<CargoCategory> CompatibleCargoCategories2 { get; set; }
		[Reactive, Category("Cargo"), Length(0, 32)] public BindingList<CargoTypeSpriteOffset> CargoTypeSpriteOffsets { get; set; } // this is a dictionary type
		[Reactive, Category("Sound")] public DrivingSoundType SoundType { get; set; }

		// SoundPropertiesData
		// these next 3 properties are a union in the dat file
		[Reactive, Category("Sound")] public FrictionSound? FrictionSound { get; set; }
		[Reactive, Category("Sound")] public Engine1Sound? Engine1Sound { get; set; }
		[Reactive, Category("Sound")] public Engine2Sound? Engine2Sound { get; set; }
		[Reactive, Category("Sound")] public BindingList<S5Header> StartSounds { get; set; }

		public VehicleViewModel(VehicleObject veh)
		{
			Mode = veh.Mode;
			Type = veh.Type;
			var_04 = veh.var_04;
			TrackTypeId = veh.TrackTypeId;
			CostIndex = veh.CostIndex;
			CostFactor = veh.CostFactor;
			Reliability = veh.Reliability;
			RunCostIndex = veh.RunCostIndex;
			RunCostFactor = veh.RunCostFactor;
			ColourType = veh.ColourType;
			CompatibleVehicles = new(veh.CompatibleVehicles);
			RequiredTrackExtras = new(veh.RequiredTrackExtras);
			CarComponents = new(veh.CarComponents);
			BodySprites = new(veh.BodySprites);
			BogieSprites = new(veh.BogieSprites);
			Power = veh.Power;
			Speed = veh.Speed;
			RackSpeed = veh.RackSpeed;
			Weight = veh.Weight;
			Flags = veh.Flags;
			MaxCargo = new(veh.MaxCargo);
			CompatibleCargoCategories1 = new(veh.CompatibleCargoCategories[0]);
			CompatibleCargoCategories2 = new(veh.CompatibleCargoCategories[1]);
			CargoTypeSpriteOffsets = new(veh.CargoTypeSpriteOffsets.Select(x => new CargoTypeSpriteOffset(x.Key, x.Value)).ToList());
			Animation = new(veh.Animation);
			AnimationHeaders = new(veh.AnimationHeaders);
			var_113 = veh.var_113;
			DesignedYear = veh.DesignedYear;
			ObsoleteYear = veh.ObsoleteYear;
			RackRailType = veh.RackRailType;
			SoundType = veh.SoundType;
			StartSounds = new(veh.StartSounds);
			FrictionSound = veh.SoundPropertyFriction;
			Engine1Sound = veh.SoundPropertyEngine1;
			Engine2Sound = veh.SoundPropertyEngine2;
		}

		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct) => GetAsStruct((locoStruct as VehicleObject)!);

		public VehicleObject GetAsStruct(VehicleObject veh)
		{
			foreach (var ctso in CargoTypeSpriteOffsets)
			{
				veh.CargoTypeSpriteOffsets[ctso.CargoCategory] = ctso.Offset;
			}

			return veh with
			{
				Mode = Mode,
				Type = Type,
				var_04 = var_04,
				TrackTypeId = TrackTypeId,
				CostIndex = CostIndex,
				CostFactor = CostFactor,
				Reliability = Reliability,
				RunCostIndex = RunCostIndex,
				RunCostFactor = RunCostFactor,
				ColourType = ColourType,
				Power = Power,
				Speed = Speed,
				RackSpeed = RackSpeed,
				Weight = Weight,
				Flags = Flags,
				var_113 = var_113,
				DesignedYear = DesignedYear,
				ObsoleteYear = ObsoleteYear,
				RackRailType = RackRailType,
				SoundType = SoundType,
				SoundPropertyFriction = FrictionSound,
				SoundPropertyEngine1 = Engine1Sound,
				SoundPropertyEngine2 = Engine2Sound,
				NumCompatibleVehicles = (byte)CompatibleVehicles.Count,
				NumRequiredTrackExtras = (byte)RequiredTrackExtras.Count,
				NumStartSounds = (byte)StartSounds.Count,
			};
		}
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CargoTypeSpriteOffset(CargoCategory CargoCategory, uint8_t Offset)
	{
		public CargoTypeSpriteOffset() : this(CargoCategory.NULL, 0)
		{ }
	}
}
