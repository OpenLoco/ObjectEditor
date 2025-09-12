using Definitions.ObjectModels.Objects.Building;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels.LocoTypes.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingStackViewModel : ReactiveObject
{
	public CardinalDirection Direction { get; init; }
	public ObservableCollection<BuildingLayerViewModel> Layers { get; set; } = [];
}

