using Definitions.ObjectModels.Objects.Scaffolding;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class ScaffoldingViewModel : LocoObjectViewModel<ScaffoldingObject>
{
	[Length(0, 3)] public BindingList<uint16_t> SegmentHeights { get; set; }
	[Length(0, 3)] public BindingList<uint16_t> RoofHeights { get; set; }

	public ScaffoldingViewModel(ScaffoldingObject so)
	{
		SegmentHeights = so.SegmentHeights.ToBindingList();
		RoofHeights = so.RoofHeights.ToBindingList();
	}

	public override ScaffoldingObject GetAsModel()
		=> new()
		{
			SegmentHeights = [.. SegmentHeights],
			RoofHeights = [.. RoofHeights]
		};
}
