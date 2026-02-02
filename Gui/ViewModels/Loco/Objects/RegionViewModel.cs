using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RegionViewModel(RegionObject model)
	: BaseViewModel<RegionObject>(model)
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

	public BindingList<ObjectModelHeader> DependentObjects { get; init; } = new(model.DependentObjects);

	[Category("Cargo")]
	public BindingList<ObjectModelHeader> CargoInfluenceObjects { get; init; } = new(model.CargoInfluenceObjects);

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; init; } = new(model.CargoInfluenceTownFilter);
}
