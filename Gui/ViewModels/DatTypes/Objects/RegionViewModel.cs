using Definitions.ObjectModels.Objects.Region;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RegionViewModel : LocoObjectViewModel<RegionObject>
{
	[Reactive] public BindingList<ObjectModelHeaderViewModel> CargoInfluenceObjects { get; set; }
	[Reactive] public BindingList<ObjectModelHeaderViewModel> DependentObjects { get; set; }
	[Reactive] public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public RegionViewModel(RegionObject ro)
	{
		CargoInfluenceObjects = new(ro.CargoInfluenceObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		DependentObjects = new(ro.DependentObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CargoInfluenceTownFilter = new(ro.CargoInfluenceTownFilter);
	}

	public override RegionObject GetAsStruct()
	{ 
		var regionObject = new RegionObject()
		{
			CargoInfluenceObjects = CargoInfluenceObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			DependentObjects = DependentObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
		};
		return regionObject;
	}
}
