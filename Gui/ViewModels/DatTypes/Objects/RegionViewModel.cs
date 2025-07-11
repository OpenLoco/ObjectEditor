using Dat.Objects;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RegionViewModel : LocoObjectViewModel<RegionObject>
{
	[Reactive] public BindingList<S5HeaderViewModel> CargoInfluenceObjects { get; set; }
	[Reactive] public BindingList<S5HeaderViewModel> DependentObjects { get; set; }
	[Reactive] public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public RegionViewModel(RegionObject ro)
	{
		CargoInfluenceObjects = new(ro.CargoInfluenceObjects.ConvertAll(x => new S5HeaderViewModel(x)));
		DependentObjects = new(ro.DependentObjects.ConvertAll(x => new S5HeaderViewModel(x)));
		CargoInfluenceTownFilter = new(ro.CargoInfluenceTownFilter);
	}

	public override RegionObject GetAsStruct(RegionObject ro)
		=> ro with
		{
			CargoInfluenceObjects = CargoInfluenceObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			NumCargoInfluenceObjects = (uint8_t)CargoInfluenceObjects.Count,
			DependentObjects = DependentObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
		};
}
