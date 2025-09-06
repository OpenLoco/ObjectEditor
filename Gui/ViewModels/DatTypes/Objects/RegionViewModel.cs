using Definitions.ObjectModels.Objects.Region;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RegionViewModel : LocoObjectViewModel<RegionObject>
{
	public DrivingSide VehiclesDriveOnThe { get; set; }
	public uint8_t pad_07 { get; set; }
	[Category("Cargo")] public List<ObjectModelHeaderViewModel> CargoInfluenceObjects { get; set; }
	[Category("Cargo")] public List<ObjectModelHeaderViewModel> DependentObjects { get; set; }
	[Category("Cargo")] public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public RegionViewModel(RegionObject ro)
	{
		CargoInfluenceObjects = [.. ro.CargoInfluenceObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		DependentObjects = [.. ro.DependentObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		CargoInfluenceTownFilter = [.. ro.CargoInfluenceTownFilter];
		VehiclesDriveOnThe = ro.VehiclesDriveOnThe;
		pad_07 = ro.pad_07;
	}

	public override RegionObject GetAsModel()
		=> new RegionObject()
		{
			CargoInfluenceObjects = CargoInfluenceObjects.ConvertAll(x => x.GetAsModel()),
			DependentObjects = DependentObjects.ConvertAll(x => x.GetAsModel()),
			CargoInfluenceTownFilter = CargoInfluenceTownFilter,
			VehiclesDriveOnThe = VehiclesDriveOnThe,
			pad_07 = pad_07
		};
}
