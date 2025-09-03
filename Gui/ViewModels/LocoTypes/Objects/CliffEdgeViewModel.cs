using Definitions.ObjectModels.Objects.CliffEdge;

namespace Gui.ViewModels;

public class CliffEdgeViewModel : LocoObjectViewModel<CliffEdgeObject>
{
	public CliffEdgeViewModel(CliffEdgeObject obj)
	{
		// No properties to initialize as CliffEdgeObject has no fields.
	}

	public override CliffEdgeObject GetAsModel()
		=> new();
}
