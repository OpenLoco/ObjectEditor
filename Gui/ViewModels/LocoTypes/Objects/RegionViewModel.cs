using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RegionViewModel(RegionObject model)
	: LocoObjectViewModel<RegionObject>(model)
{
	public DrivingSide VehiclesDriveOnThe
	{
		get => Model.VehiclesDriveOnThe;
		set => Model.VehiclesDriveOnThe = value;
	}

	public uint8_t pad_07
	{
		get => Model.pad_07;
		set => Model.pad_07 = value;
	}

	public BindingList<ObjectModelHeader> DependentObjects { get; set; } = new(model.DependentObjects);

	[Category("Cargo")]
	public BindingList<ObjectModelHeader> CargoInfluenceObjects { get; set; } = new(model.CargoInfluenceObjects);

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; } = new(model.CargoInfluenceTownFilter);
}
