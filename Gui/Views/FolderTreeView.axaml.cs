using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Gui.Models;
using Gui.ViewModels;
using ReactiveUI;

namespace Gui.Views;

public partial class FolderTreeView : UserControl
{
	CompositeDisposable? _vmSubscriptions;
	HierarchicalTreeDataGridSource<FileSystemItem>? _source;

	public FolderTreeView()
	{
		InitializeComponent();
		DataContextChanged += OnDataContextChanged;
		DetachedFromVisualTree += (_, _) => DisposeVmSubscriptions();
	}

	void OnDataContextChanged(object? sender, EventArgs e)
	{
		DisposeVmSubscriptions();
		if (DataContext is not FolderTreeViewModel vm)
		{
			return;
		}

		var grid = this.FindControl<TreeDataGrid>("ObjectsTreeDataGrid");
		if (grid is null)
		{
			return;
		}

		var cellTemplate = grid.Resources["Object"] as IDataTemplate;
		var editTemplate = grid.Resources["Edit"] as IDataTemplate;

		_source = FileSystemItemDataGridSource.Create(
			vm.TreeRoot ?? [],
			cellTemplate,
			editTemplate);
		grid.Source = _source;

		var subs = new CompositeDisposable();
		_vmSubscriptions = subs;

		if (_source.RowSelection is { } rowSelection)
		{
			rowSelection.SelectionChanged += OnRowSelectionChanged;
			Disposable.Create(() => rowSelection.SelectionChanged -= OnRowSelectionChanged).DisposeWith(subs);
		}

		vm.ExpandAllRequests.Subscribe(_ => _source?.ExpandAll()).DisposeWith(subs);
		vm.CollapseAllRequests.Subscribe(_ => _source?.CollapseAll()).DisposeWith(subs);
		vm.WhenAnyValue(x => x.TreeRoot).Subscribe(items =>
		{
			if (_source is not null)
			{
				_source.Items = items ?? [];
			}
		}).DisposeWith(subs);
	}

	void OnRowSelectionChanged(object? sender, TreeDataGridSelectionChangedEventArgs<FileSystemItem> e)
	{
		if (DataContext is FolderTreeViewModel vm)
		{
			vm.OnSelectionChanged(e.SelectedItems!);
		}
	}

	void DisposeVmSubscriptions()
	{
		_vmSubscriptions?.Dispose();
		_vmSubscriptions = null;
	}
}
