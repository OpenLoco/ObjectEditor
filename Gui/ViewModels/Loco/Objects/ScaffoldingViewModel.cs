using Definitions.ObjectModels.Objects.Scaffolding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class ScaffoldingViewModel(ScaffoldingObject model) : BaseViewModel<ScaffoldingObject>(model)
{
	[Length(0, 3)]
	public BindingList<uint16_t> SegmentHeights { get; init; } = [with(model.SegmentHeights)];

	[Length(0, 3)]
	public BindingList<uint16_t> RoofHeights { get; init; } = [with(model.RoofHeights)];
}
