using OpenLoco.Dat.Objects;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class RegionViewModel : LocoObjectViewModel<RegionObject>
	{
		[Reactive] public BindingList<S5HeaderViewModel> RequiredObjects { get; set; }
		[Reactive] public BindingList<S5HeaderViewModel> DependentObjects { get; set; }

		[Reactive, Category("<unknown>")] public BindingList<uint8_t> var_06 { get; set; }
		[Reactive, Category("<unknown>")] public BindingList<uint8_t> var_0D { get; set; }

		public RegionViewModel(RegionObject ro)
		{
			RequiredObjects = new(ro.RequiredObjects.ConvertAll(x => new S5HeaderViewModel(x)));
			DependentObjects = new(ro.DependentObjects.ConvertAll(x => new S5HeaderViewModel(x)));
			var_06 = new(ro.var_06);
			var_0D = new(ro.var_0D);
		}

		public override RegionObject GetAsStruct(RegionObject ro)
			=> ro with
			{
				RequiredObjects = RequiredObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				RequiredObjectCount = (uint8_t)RequiredObjects.Count,
				DependentObjects = DependentObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				// var_06 is bound
				// var_0D is bound
			};
	}
}
