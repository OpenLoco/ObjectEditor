using Avalonia.Controls.Selection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gui.ViewModels.Graphics;

public class GroupedImageViewModel : ReactiveObject
{
	public string GroupName { get; set; }
	public ObservableCollection<ImageViewModel> Images { get; set; }

	[Reactive]
	public SelectionModel<ImageViewModel> SelectionModel { get; set; }

	public GroupedImageViewModel(string groupName, IEnumerable<ImageViewModel> images)
	{
		GroupName = groupName;
		Images = new ObservableCollection<ImageViewModel>(images);

		SelectionModel = new SelectionModel<ImageViewModel>
		{
			SingleSelect = false
		};
	}
}
