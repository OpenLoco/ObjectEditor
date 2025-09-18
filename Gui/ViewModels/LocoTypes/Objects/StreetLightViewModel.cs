using Definitions.ObjectModels.Objects.Streetlight;
using System.Collections.ObjectModel;

namespace Gui.ViewModels;

public class StreetLightViewModel(StreetLightObject model)
	: LocoObjectViewModel<StreetLightObject>(model)
{
	public ObservableCollection<uint16_t> DesignedYears { get; set; } = new ObservableCollection<uint16_t>(model.DesignedYears);

	public override void CopyBackToModel()
		=> Model.DesignedYears = [.. DesignedYears];
}
