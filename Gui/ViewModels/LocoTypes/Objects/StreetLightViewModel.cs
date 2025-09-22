using Definitions.ObjectModels.Objects.Streetlight;
using System.ComponentModel;

namespace Gui.ViewModels;

public class StreetLightViewModel(StreetLightObject model)
	: LocoObjectViewModel<StreetLightObject>(model)
{
	public BindingList<uint16_t> DesignedYears { get; init; } = new(model.DesignedYears);
}
