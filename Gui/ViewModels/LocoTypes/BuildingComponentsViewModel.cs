using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using System.Collections.Generic;

namespace Gui.ViewModels.LocoTypes;

public class BuildingComponentsViewModel : ReactiveObject
{
	public BuildingComponentsModel BuildingComponents { get; }

	List<GraphicsElement> GraphicsElements { get; }

	public BuildingComponentsViewModel(BuildingComponentsModel buildingComponents, List<GraphicsElement> graphicsElements)
	{
		BuildingComponents = buildingComponents;
		GraphicsElements = graphicsElements;
	}
}
