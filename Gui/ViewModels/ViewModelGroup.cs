using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public sealed class ViewModelGroup : ReactiveObject, IViewModel
{
	readonly IViewModelGroupHost _host;

	public ViewModelGroup(string displayName, IViewModelGroupHost host)
	{
		_host = host;
		DisplayName = displayName;

		_ = _viewModels.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _viewModelsCollection)
			.Subscribe();

		AddSelectedViewModelCommand = ReactiveCommand.Create(AddSelectedViewModel);
		RemoveSelectedViewModelCommand = ReactiveCommand.Create(RemoveSelectedViewModel);
		RemoveGroupCommand = ReactiveCommand.Create(RemoveGroup);
	}

	[Reactive]
	public string DisplayName { get; set; }

	[Reactive]
	public IViewModel? SelectedViewModelToAdd { get; set; }

	[Reactive]
	public IViewModel? SelectedViewModelToRemove { get; set; }

	public ReadOnlyObservableCollection<IViewModel> AllViewModels
		=> _host.AllViewModels;

	public bool CanRemoveGroup
		=> !_host.IsDefaultGroup(this);

	public ReactiveCommand<Unit, Unit> AddSelectedViewModelCommand { get; }
	public ReactiveCommand<Unit, Unit> RemoveSelectedViewModelCommand { get; }
	public ReactiveCommand<Unit, Unit> RemoveGroupCommand { get; }

	private readonly SourceList<IViewModel> _viewModels = new();
	private readonly ReadOnlyObservableCollection<IViewModel> _viewModelsCollection;

	[Browsable(false)]
	public ReadOnlyObservableCollection<IViewModel> ViewModels
		=> _viewModelsCollection;

	public void AddViewModel(IViewModel? vm)
	{
		if (vm != null && !_viewModels.Items.Contains(vm))
		{
			_viewModels.Add(vm);
		}
	}

	public bool RemoveViewModel(IViewModel vm)
		=> _viewModels.Remove(vm);

	public void ClearViewModels()
		=> _viewModels.Clear();

	void AddSelectedViewModel()
	{
		if (SelectedViewModelToAdd != null)
		{
			_ = _host.MoveViewModelToGroup(SelectedViewModelToAdd, this);
		}
	}

	void RemoveSelectedViewModel()
	{
		if (SelectedViewModelToRemove != null)
		{
			_ = _host.RemoveViewModelFromAllGroups(SelectedViewModelToRemove);
		}
	}

	void RemoveGroup()
	{
		if (CanRemoveGroup)
		{
			_ = _host.RemoveViewModelGroup(this);
		}
	}
}
