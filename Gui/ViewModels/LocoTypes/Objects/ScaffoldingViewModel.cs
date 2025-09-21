using Definitions.ObjectModels.Objects.Scaffolding;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class ScaffoldingViewModel : LocoObjectViewModel<ScaffoldingObject>
{
	[Length(0, 3)] public ObservableCollection<uint16_t> SegmentHeights { get; set; }
	[Length(0, 3)] public ObservableCollection<uint16_t> RoofHeights { get; set; }

	public ScaffoldingViewModel(ScaffoldingObject model) : base(model)
	{
		SegmentHeights = new(model.SegmentHeights);
		RoofHeights = new(model.RoofHeights);
	}

	public override void CopyBackToModel()
	{
		//Model.SegmentHeights = [.. SegmentHeights];
		//Model.RoofHeights = [.. RoofHeights];
	}
}
