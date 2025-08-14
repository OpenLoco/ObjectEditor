using Definitions.ObjectModels.Objects.Streetlight;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace Gui.ViewModels;

public class StreetLightViewModel : LocoObjectViewModel<StreetLightObject>
{
	[Reactive] public BindingList<uint16_t> DesignedYears { get; set; }

	public StreetLightViewModel(StreetLightObject ro)
	{
		//DesignedYears = ro.DesignedYears.ToBindingList();
	}

	public override StreetLightObject GetAsStruct()
		=> new()
		{
			//DesignedYears = [.. DesignedYears]
		};
}
