using Definitions.ObjectModels.Objects.Tunnel;

namespace Gui.ViewModels;

public class TunnelViewModel : LocoObjectViewModel<TunnelObject>
{
	public TunnelViewModel(TunnelObject obj)
	{
		// No properties to initialize as TunnelObject has no fields.
	}

	public override TunnelObject GetAsModel()
		=> new();
}
