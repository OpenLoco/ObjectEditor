using Definitions.ObjectModels.Objects.ScenarioText;

namespace Gui.ViewModels;

public class ScenarioTextViewModel : LocoObjectViewModel<ScenarioTextObject>
{
	public ScenarioTextViewModel(ScenarioTextObject obj)
	{
		// No properties to initialize as ScenarioTextObject has no fields.
	}

	public override ScenarioTextObject GetAsModel()
		=> new();
}
