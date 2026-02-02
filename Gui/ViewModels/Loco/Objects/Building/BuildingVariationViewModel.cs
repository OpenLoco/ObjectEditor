using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingVariationViewModel : ReactiveObject
{
	public string VariationName { get; init; }
	public ObservableCollection<BuildingStackViewModel> Directions { get; set; } = [];
}

