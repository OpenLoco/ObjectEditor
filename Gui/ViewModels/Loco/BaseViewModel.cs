using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public abstract class BaseViewModel<T> : ReactiveObject, IViewModel, IViewModelGroupHost where T : class
{
	[Browsable(false)]
	public virtual string DisplayName
		=> typeof(T).Name;

	protected BaseViewModel(T? model = default)
	{
		Model = model;

		_ = _allViewModels.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _allViewModelsCollection)
			.Subscribe();

		_ = _viewModelGroups.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _viewModelGroupsCollection)
			.Subscribe();

		AddGroupCommand = ReactiveCommand.Create(AddGroup);
		ResetViewModelGroups();
	}

	[Reactive, Browsable(false)]
	public T? Model { get; protected set; }

	private readonly SourceList<IViewModel> _allViewModels = new();
	private readonly ReadOnlyObservableCollection<IViewModel> _allViewModelsCollection;

	[Browsable(false)]
	public ReadOnlyObservableCollection<IViewModel> AllViewModels
		=> _allViewModelsCollection;

	[Browsable(false)]
	public string NewGroupName { get; set; } = "New Group";

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> AddGroupCommand { get; }

	private readonly SourceList<ViewModelGroup> _viewModelGroups = new();
	private readonly ReadOnlyObservableCollection<ViewModelGroup> _viewModelGroupsCollection;

	[Browsable(false)]
	public ReadOnlyObservableCollection<ViewModelGroup> ViewModelGroups
		=> _viewModelGroupsCollection;

	[Browsable(false)]
	public ViewModelGroup DefaultViewModelGroup { get; private set; } = null!;

	[Browsable(false)]
	public ReadOnlyObservableCollection<IViewModel> ViewModels
		=> DefaultViewModelGroup.ViewModels;

	protected void ResetViewModelGroups(string defaultGroupName = "General")
	{
		_allViewModels.Clear();

		_viewModelGroups.Edit(list =>
		{
			list.Clear();
			DefaultViewModelGroup = new ViewModelGroup(defaultGroupName, this);
			list.Add(DefaultViewModelGroup);
		});
	}

	protected ViewModelGroup AddViewModelGroup(string displayName)
	{
		var group = new ViewModelGroup(displayName, this);
		_viewModelGroups.Add(group);
		return group;
	}

	public bool RemoveViewModelGroup(ViewModelGroup group)
	{
		if (ReferenceEquals(group, DefaultViewModelGroup))
		{
			return false;
		}

		group.ClearViewModels();
		return _viewModelGroups.Remove(group);
	}

	protected void AddViewModel(IViewModel? vm)
		=> AddViewModelToGroup(vm, DefaultViewModelGroup);

	protected void AddViewModelToGroup(IViewModel? vm, ViewModelGroup group)
	{
		if (vm != null)
		{
			_ = MoveViewModelToGroup(vm, group);
		}
	}

	protected void RemoveViewModelFromGroup(IViewModel vm, ViewModelGroup group)
		=> group.RemoveViewModel(vm);

	public bool RemoveViewModelFromAllGroups(IViewModel vm)
	{
		var removed = false;
		foreach (var group in ViewModelGroups)
		{
			removed |= group.RemoveViewModel(vm);
		}

		EnsureViewModelTracked(vm);
		return removed;
	}

	public bool MoveViewModelToGroup(IViewModel vm, ViewModelGroup group)
	{
		if (!ViewModelGroups.Contains(group))
		{
			return false;
		}

		_ = RemoveViewModelFromAllGroups(vm);
		group.AddViewModel(vm);
		EnsureViewModelTracked(vm);
		return true;
	}

	public bool IsDefaultGroup(ViewModelGroup group)
		=> ReferenceEquals(group, DefaultViewModelGroup);

	protected void ClearViewModels()
	{
		foreach (var group in ViewModelGroups)
		{
			group.ClearViewModels();
		}

		_allViewModels.Clear();
	}

	void AddGroup()
	{
		var name = NewGroupName?.Trim();
		if (string.IsNullOrWhiteSpace(name))
		{
			return;
		}

		if (ViewModelGroups.Any(x => x.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase)))
		{
			return;
		}

		_ = AddViewModelGroup(name);
	}

	void EnsureViewModelTracked(IViewModel viewModel)
	{
		if (!_allViewModels.Items.Contains(viewModel))
		{
			_allViewModels.Add(viewModel);
		}
	}
}
