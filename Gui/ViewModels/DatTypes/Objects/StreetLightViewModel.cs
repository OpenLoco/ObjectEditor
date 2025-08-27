using Definitions.ObjectModels.Objects.Streetlight;
using System.Collections.Generic;

namespace Gui.ViewModels;

public class StreetLightViewModel : LocoObjectViewModel<StreetLightObject>
{
	public List<uint16_t> DesignedYears { get; set; }

	public StreetLightViewModel(StreetLightObject ro)
	{
		DesignedYears = ro.DesignedYears;
	}

	public override StreetLightObject GetAsModel()
		=> new()
		{
			DesignedYears = [.. DesignedYears]
		};
}
