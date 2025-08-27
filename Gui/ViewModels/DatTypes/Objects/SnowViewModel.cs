using Definitions.ObjectModels.Objects.Snow;

namespace Gui.ViewModels;

public class SnowViewModel : LocoObjectViewModel<SnowObject>
{
	public SnowViewModel(SnowObject obj)
	{
		// No properties to initialize as SnowObject has no fields.
	}

	public override SnowObject GetAsModel()
		=> new();
}
