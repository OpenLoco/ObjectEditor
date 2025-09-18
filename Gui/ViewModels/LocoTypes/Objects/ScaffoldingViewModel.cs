using Definitions.ObjectModels.Objects.Scaffolding;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class ScaffoldingViewModel(ScaffoldingObject model)
	: LocoObjectViewModel<ScaffoldingObject>(model)
{
	[Length(0, 3)] public ObservableCollection<uint16_t> SegmentHeights { get; set; } = new(model.SegmentHeights);
	[Length(0, 3)] public ObservableCollection<uint16_t> RoofHeights { get; set; } = new(model.RoofHeights);

	public override void CopyBackToModel()
	{
		Model.SegmentHeights = [.. SegmentHeights];
		Model.RoofHeights = [.. RoofHeights];
	}
}
