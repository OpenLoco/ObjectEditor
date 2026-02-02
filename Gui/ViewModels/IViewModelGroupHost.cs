using System.Collections.ObjectModel;

namespace Gui.ViewModels;

public interface IViewModelGroupHost
{
	ReadOnlyObservableCollection<IViewModel> AllViewModels { get; }
	ViewModelGroup DefaultViewModelGroup { get; }

	bool MoveViewModelToGroup(IViewModel viewModel, ViewModelGroup group);
	bool RemoveViewModelFromAllGroups(IViewModel viewModel);
	bool RemoveViewModelGroup(ViewModelGroup group);
	bool IsDefaultGroup(ViewModelGroup group);
}
