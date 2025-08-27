using Definitions.ObjectModels.Objects.Region;
using System.Collections.Generic;

namespace Gui.ViewModels;

public class RegionViewModel : LocoObjectViewModel<RegionObject>
{
	public List<ObjectModelHeaderViewModel> CargoInfluenceObjects { get; set; }
	public List<ObjectModelHeaderViewModel> DependentObjects { get; set; }
	public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public RegionViewModel(RegionObject ro)
	{
		CargoInfluenceObjects = [.. ro.CargoInfluenceObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		DependentObjects = [.. ro.DependentObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		CargoInfluenceTownFilter = [.. ro.CargoInfluenceTownFilter];
	}

	public override RegionObject GetAsModel()
	{ 
		var regionObject = new RegionObject()
		{
			CargoInfluenceObjects = CargoInfluenceObjects.ConvertAll(x => x.GetAsModel()),
			DependentObjects = DependentObjects.ConvertAll(x => x.GetAsModel()),
			CargoInfluenceTownFilter = CargoInfluenceTownFilter,
		};
		return regionObject;
	}
}
