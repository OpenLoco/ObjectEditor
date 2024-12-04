using OpenLoco.Dat.Objects;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	public class StreetLightViewModel : LocoObjectViewModel<StreetLightObject>
	{
		[Reactive] public BindingList<uint16_t> DesignedYears { get; set; }

		public StreetLightViewModel(StreetLightObject ro)
		{
			DesignedYears = ro.DesignedYears.ToBindingList();
		}

		public override StreetLightObject GetAsStruct(StreetLightObject input)
			=> input with
			{
				DesignedYears = [.. DesignedYears]
			};
	}
}
