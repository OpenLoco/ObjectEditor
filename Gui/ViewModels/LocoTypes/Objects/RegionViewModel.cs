using Definitions.ObjectModels.Objects.Region;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RegionViewModel : LocoObjectViewModel<RegionObject>
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

	[Category("Cargo")] public List<ObjectModelHeaderViewModel> CargoInfluenceObjects { get; set; }
	[Category("Cargo")] public List<ObjectModelHeaderViewModel> DependentObjects { get; set; }
	[Category("Cargo")] public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public RegionViewModel(RegionObject model)
		: base(model)
	{
		CargoInfluenceObjects = [.. model.CargoInfluenceObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		DependentObjects = [.. model.DependentObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		CargoInfluenceTownFilter = [.. model.CargoInfluenceTownFilter];
	}

	public override void CopyBackToModel()
	{
		Model.CargoInfluenceObjects = [.. CargoInfluenceObjects.Select(x => x.Model)];
		Model.DependentObjects = [.. DependentObjects.Select(x => x.Model)];
		Model.CargoInfluenceTownFilter = [.. CargoInfluenceTownFilter];
	}
}
