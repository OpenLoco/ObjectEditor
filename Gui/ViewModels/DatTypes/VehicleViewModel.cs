using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvaGui.ViewModels
{
	public interface IObjectViewModel;

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GenericObjectViewModel : ReactiveObject, IObjectViewModel
	{
		[Reactive] public required ILocoStruct Object { get; set; }
	}

	public class VehicleViewModel : ReactiveObject, IObjectViewModel
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
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Cost")] public int16_t CostFactor { get; set; }
		[Reactive, Category("Cost")] public uint8_t RunCostIndex { get; set; }
		[Reactive, Category("Cost")] public int16_t RunCostFactor { get; set; }
		[Reactive] public BindingList<S5Header> CompatibleVehicles { get; set; }
		[Reactive] public BindingList<S5Header> RequiredTrackExtras { get; set; }
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
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CargoTypeSpriteOffset(CargoCategory CargoCategory, uint8_t Offset)
	{
		public CargoTypeSpriteOffset() : this(CargoCategory.NULL, 0)
		{ }
	}
}
