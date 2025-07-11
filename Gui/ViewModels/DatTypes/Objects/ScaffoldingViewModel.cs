using Dat.Objects;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels
{
	public class ScaffoldingViewModel : LocoObjectViewModel<ScaffoldingObject>
	{
		[Reactive, Length(0, 3)] public BindingList<uint16_t> SegmentHeights { get; set; }
		[Reactive, Length(0, 3)] public BindingList<uint16_t> RoofHeights { get; set; }

		public ScaffoldingViewModel(ScaffoldingObject so)
		{
			SegmentHeights = so.SegmentHeights.ToBindingList();
			RoofHeights = so.RoofHeights.ToBindingList();
		}

		public override ScaffoldingObject GetAsStruct(ScaffoldingObject sco)
			=> sco with
			{
				SegmentHeights = [.. SegmentHeights],
				RoofHeights = [.. RoofHeights]
			};
	}
}
