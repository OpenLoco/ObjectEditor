using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public abstract class BaseViewModel : ReactiveObject, IViewModel, IViewModelGroupHost, IDisposable
{
	[Browsable(false)]
	public virtual string DisplayName => GetType().Name;

	readonly CompositeDisposable subscriptions = [];
	bool disposed;

	protected BaseViewModel()
	{
		_ = _allViewModels.Connect()
			.ObserveOn(RxSchedulers.MainThreadScheduler)
			.Bind(out _allViewModelsCollection)
			.Subscribe()
			.DisposeWith(subscriptions);

		_ = _viewModelGroups.Connect()
			.ObserveOn(RxSchedulers.MainThreadScheduler)
			.Bind(out _viewModelGroupsCollection)
			.Subscribe()
			.DisposeWith(subscriptions);

		AddGroupCommand = ReactiveCommand.Create(AddGroup);
		ResetViewModelGroups();
	}

	private readonly SourceList<IViewModel> _allViewModels = new();
	private readonly ReadOnlyObservableCollection<IViewModel> _allViewModelsCollection;

	[Browsable(false)]
	public ReadOnlyObservableCollection<IViewModel> AllViewModels
		=> _allViewModelsCollection;

	[Browsable(false)]
	public string NewGroupName { get; set; } = "<unnamed>";

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
		ClearViewModels();
		DisposeViewModelGroups();

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

		BaseViewModel.DisposeViewModels(group.ViewModels);
		group.ClearViewModels();
		var removed = _viewModelGroups.Remove(group);
		(group as IDisposable)?.Dispose();
		return removed;
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

	public virtual void SynchronizeToModel()
	{
	}

	protected void ClearViewModels()
	{
		BaseViewModel.DisposeViewModels(ViewModelGroups.SelectMany(group => group.ViewModels));

		foreach (var group in ViewModelGroups)
		{
			group.ClearViewModels();
		}

		_allViewModels.Clear();
	}

	static void DisposeViewModels(IEnumerable<IViewModel> viewModels)
	{
		foreach (var viewModel in viewModels.ToHashSet())
		{
			if (viewModel is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	void DisposeViewModelGroups()
	{
		foreach (var group in ViewModelGroups.OfType<IDisposable>())
		{
			group.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}

		if (disposing)
		{
			ClearViewModels();
			DisposeViewModelGroups();
			subscriptions.Dispose();
		}

		disposed = true;
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

public abstract class BaseViewModel<T> : BaseViewModel where T : class
{
	[Browsable(false)]
	public override string DisplayName => typeof(T).Name;

	protected BaseViewModel(T? model)
		: base()
	{
		Model = model!;
	}

	[Reactive, Browsable(false)]
	public T Model { get; protected set; }
}
