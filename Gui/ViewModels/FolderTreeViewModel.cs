using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Threading;
using Dat.Data;
using Definitions.ObjectModels.Types;
using DynamicData;
using DynamicData.Binding;
using Gui.Models;
using Gui.ViewModels.Filters;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class DesignerFolderTreeViewModel : FolderTreeViewModel
{
	public DesignerFolderTreeViewModel()
	{
		SelectedTabIndex = 0;
		CurrentLocalDirectory = "test/directory";
		CurrentDirectoryItems.Add(new(
			"local-displayname1",
			"local-filename1",
			null,
			null,
			null,
			ObjectType.Airport,
			ObjectSource.Custom,
			null,
			null,
			null));

		var availableFilterCategories = new List<FilterTypeViewModel>
		{
			new() { Type = typeof(ObjectIndexEntry), DisplayName = "Index data", IconName = nameof(ObjectIndexEntry) },
			new() { Type = typeof (MetadataModel), DisplayName = "Metadata", IconName = nameof(MetadataModel) }
		};

		Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
		Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
	}
}

public class FolderTreeViewModel : ReactiveObject
{
	[Reactive]
	protected SourceList<ObjectIndexEntry> CurrentDirectoryItems { get; set; } = new();

	public HierarchicalTreeDataGridSource<FileSystemItem> TreeDataGridSource { get; set; }
	ReadOnlyObservableCollection<ObjectIndexEntry> treeDataGridSource;

	public ObservableCollection<FilterViewModel> Filters { get; } = [];
	public ReactiveCommand<Unit, Unit> AddFilterCommand { get; }

	private readonly BehaviorSubject<Func<ObjectIndexEntry, bool>> _filterSubject;

	ObjectEditorModel Model { get; init; }

	[Reactive]
	public string CurrentLocalDirectory { get; set; } = string.Empty;
	public string CurrentDirectory => SelectedTabIndex == 0
		? CurrentLocalDirectory
		: Model.Settings.UseHttps
			? Model.Settings.ServerAddressHttps
			: Model.Settings.ServerAddressHttp;

	[Reactive]
	public FileSystemItem? CurrentlySelectedObject { get; set; }

	[Reactive]
	public float IndexOrDownloadProgress { get; set; }

	Progress<float> Progress { get; } = new();

	public ReactiveCommand<Unit, Task>? RefreshDirectoryItems { get; }
	public ReactiveCommand<Unit, Unit>? OpenCurrentFolder { get; }
	public ReactiveCommand<FileSystemItem, Unit>? OpenFolderFor { get; }

	public ObservableCollection<ObjectDisplayMode> DisplayModeItems { get; } = [.. Enum.GetValues<ObjectDisplayMode>()];

	[Reactive]
	public int SelectedTabIndex { get; set; }

	public bool IsLocal => SelectedTabIndex == 0;
	public string RecreateText => IsLocal ? "Recreate index" : "Download object list";
	public string DirectoryFileCount => $"Objects: {CurrentDirectoryItems.Count}";

	public FolderTreeViewModel() { }

	public FolderTreeViewModel(ObjectEditorModel model)
	{
		Model = model;
		Progress.ProgressChanged += (_, progress) => IndexOrDownloadProgress = progress;

		var availableFilterCategories = new List<FilterTypeViewModel>
		{
			new() { Type = typeof(ObjectIndexEntry), DisplayName = "Index data", IconName = nameof(ObjectIndexEntry) },
			new() { Type = typeof (MetadataModel), DisplayName = "Metadata", IconName = nameof(MetadataModel) }
		};

		// todo: add in object-specific searches
		//var objBrush = new SolidColorBrush(Color.FromArgb(0x30, 0x80, 0x80, 0x80));
		//foreach (var obj in Enum.GetValues<ObjectType>().OrderBy(x => x.ToString()))
		//{
		//	var typeOfObj = ObjectTypeMapping.ObjectTypeToStructType(obj);
		//	availableFilterCategories.Add(new()
		//	{
		//		Type = typeOfObj,
		//		DisplayName = typeOfObj.Name.Replace("Object", string.Empty),
		//		IconName = typeOfObj.Name.Replace("Object", string.Empty),
		//		BackgroundColour = objBrush
		//	});

		//}

		AddFilterCommand = ReactiveCommand.Create(() => Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter)));

		_filterSubject = new BehaviorSubject<Func<ObjectIndexEntry, bool>>(t => true);

		var filtersChanged = Filters.ToObservableChangeSet()
			.Skip(1)
			.AutoRefresh(f => f.SelectedProperty)
			.AutoRefresh(f => f.SelectedOperator)
			.AutoRefresh(f => f.BoolValue)
			.AutoRefresh(f => f.DateValue)
			.AutoRefresh(f => f.EnumValue)
			.AutoRefresh(f => f.TextValue)
			.ToCollection()
			.Throttle(TimeSpan.FromMilliseconds(200))
			.Select(_ => CreateFilterPredicate())
			.Subscribe(_filterSubject);

		_ = CurrentDirectoryItems.Connect()
			.Filter(_filterSubject)
			.Bind(out treeDataGridSource)
			.Subscribe(_ => UpdateDirectoryItemsView());

		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(async _ => await ReloadDirectoryAsync(true));
		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));
		_ = this.WhenAnyValue(o => o.TreeDataGridSource).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));
		_ = this.WhenAnyValue(o => o.TreeDataGridSource).Skip(1).Subscribe(_ => CurrentlySelectedObject = null);
		_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => UpdateDirectoryItemsView());
		_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(RecreateText)));
		_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));
		_ = this.WhenAnyValue(o => o.CurrentDirectoryItems).Skip(1).Subscribe(_ => UpdateDirectoryItemsView());

		CurrentLocalDirectory = Model.Settings.ObjDataDirectory;
	}

	protected void RemoveFilter(FilterViewModel filter)
		=> Filters.Remove(filter);

	private Func<ObjectIndexEntry, bool> CreateFilterPredicate()
	{
		var parameter = Expression.Parameter(typeof(ObjectIndexEntry), "entry");
		Expression combinedExpression = Expression.Constant(true);

		foreach (var filter in Filters)
		{
			try
			{
				var filterExpression = filter.BuildExpression();
				if (filterExpression != null)
				{
					var invoked = Expression.Invoke(filterExpression, parameter);
					combinedExpression = Expression.AndAlso(combinedExpression, invoked);
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				Console.WriteLine($"Error building filter expression: {ex.Message}");
			}
		}

		return Expression
			.Lambda<Func<ObjectIndexEntry, bool>>(combinedExpression, parameter)
			.Compile();
	}

	public static int CountNodes(FileSystemItem fib)
	{
		if (fib.SubNodes == null || fib.SubNodes.Count == 0)
		{
			return 1;
		}

		var count = 0;

		foreach (var node in fib.SubNodes)
		{
			count += CountNodes(node);
		}

		return count;
	}

	async Task ReloadDirectoryAsync(bool useExistingIndex)
	{
		if (SelectedTabIndex == 0)
		{
			// local
			await LoadObjDirectoryAsync(CurrentLocalDirectory, useExistingIndex);
		}
		else // remote
		{
			await LoadOnlineDirectoryAsync(useExistingIndex);
		}

		await Model.CheckForDatFilesNotOnServer();
	}

	async Task LoadObjDirectoryAsync(string directory, bool useExistingIndex)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			CurrentDirectoryItems.Clear();
			return;
		}

		await Model.LoadObjDirectoryAsync(directory, Progress, useExistingIndex);

		if (Model.ObjectIndex != null)
		{
			var items = Model.ObjectIndex.Objects.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes);
			CurrentDirectoryItems.Clear();
			CurrentDirectoryItems.AddRange(items);
		}

		UpdateDirectoryItemsView();
	}

	async Task LoadOnlineDirectoryAsync(bool useExistingIndex)
	{
		if (Design.IsDesignMode)
		{
			// DO NOT WEB QUERY AT DESIGN TIME
			return;
		}

		if ((!useExistingIndex || Model.ObjectIndexOnline == null) && Model.ObjectServiceClient != null)
		{
			Model.ObjectIndexOnline = new ObjectIndex((await Model.ObjectServiceClient.GetObjectListAsync())
				.Select(x => new ObjectIndexEntry(
					x.DisplayName,
					null,
					x.Id,
					x.DatChecksum,
					null,
					x.ObjectType,
					x.ObjectSource,
					x.CreatedDate,
					x.ModifiedDate,
					x.VehicleType)));
		}

		if (Model.ObjectIndexOnline != null)
		{
			var items = Model.ObjectIndexOnline.Objects.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes);
			CurrentDirectoryItems.Clear();
			CurrentDirectoryItems.AddRange(items);

			UpdateDirectoryItemsView();
		}
	}

	protected void UpdateDirectoryItemsView()
	{
		var _treeGridDataSource = ConstructTreeView(
			treeDataGridSource,
			IsLocal ? Model.Settings.ObjDataDirectory : Model.Settings.DownloadFolder,
			IsLocal ? FileLocation.Local : FileLocation.Online);

		//treeDataGridSource = [.. items];
		TreeDataGridSource = new HierarchicalTreeDataGridSource<FileSystemItem>(_treeGridDataSource)
		{
			Columns =
			{
				new HierarchicalExpanderColumn<FileSystemItem>(
					new TemplateColumn<FileSystemItem>(
						string.Empty, // the column name
						"Object",
						"Edit",
						new GridLength(1, GridUnitType.Auto),
						new()
						{
							//CompareAscending = FileSystemItemBase.SortAscending(x => x.Name),
							//CompareDescending = FileSystemItemBase.SortDescending(x => x.Name),
							IsTextSearchEnabled = true,
							TextSearchValueSelector = x => x.ToString()
						}),
					x => x.SubNodes),

				new TextColumn<FileSystemItem, string?>("Source", x => GetNiceObjectSource(x.ObjectSource)),
				new TextColumn<FileSystemItem, FileLocation?>("Origin", x => x.FileLocation),
				new TextColumn<FileSystemItem, string?>("Location", x => x.FileName),
				new TextColumn<FileSystemItem, DateOnly?>("Created", x => x.CreatedDate),
				new TextColumn<FileSystemItem, DateOnly?>("Modified", x => x.ModifiedDate),
			},
		};

		Dispatcher.UIThread.Invoke(new Action(() => TreeDataGridSource.RowSelection!.SelectionChanged += SelectionChanged));

		this.RaisePropertyChanged(nameof(TreeDataGridSource));

	}

	static string GetNiceObjectSource(ObjectSource? os)
		=> os switch
		{
			ObjectSource.Custom => "Custom",
			ObjectSource.LocomotionSteam => "Steam",
			ObjectSource.LocomotionGoG => "GoG",
			ObjectSource.OpenLoco => "OpenLoco",
			null => string.Empty,
			_ => throw new NotImplementedException(),
		};

	void SelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<FileSystemItem> e)
	{
		CurrentlySelectedObject = null;
		if (e.SelectedItems.Count == 1)
		{
			CurrentlySelectedObject = e.SelectedItems[0];
		}
	}

	public static FileSystemItem IndexEntryToFileSystemItem(ObjectIndexEntry x, string baseDirectory, FileLocation fileLocation)
	{
		var computedFileName = fileLocation == FileLocation.Online ? $"{x.DisplayName}-{x.Id}.dat" : x.FileName;
		// fallback - DisplayName is never null
		computedFileName ??= x.DisplayName;

		return new FileSystemItem(x.DisplayName, Path.Combine(baseDirectory, computedFileName), x.Id, x.CreatedDate, x.ModifiedDate, fileLocation, x.ObjectSource);
	}

	static List<FileSystemItem> ConstructTreeView(IEnumerable<ObjectIndexEntry> index, string baseDirectory, FileLocation fileLocation)
	{
		var result = new List<FileSystemItem>();
		var groupedObjects = index
			.GroupBy(x => x.ObjectType)
			.OrderBy(fsg => fsg.Key.ToString());

		foreach (var objGroup in groupedObjects)
		{
			ObservableCollection<FileSystemItem> subNodes;
			if (objGroup.Key == ObjectType.Vehicle)
			{
				subNodes = [];
				foreach (var vg in objGroup
					.GroupBy(o => o.VehicleType)
					.OrderBy(vg => vg.Key.ToString()))
				{
					var vehicleSubNodes = new ObservableCollection<FileSystemItem>(vg
						.Select(x => IndexEntryToFileSystemItem(x, baseDirectory, fileLocation))
						.OrderBy(x => x.DisplayName));

					if (vg.Key == null)
					{
						// this should be impossible - object says its a vehicle but doesn't have a vehicle type
						// todo: move validation into the loading stage or cstr of IndexObjectHeader
						continue;
					}

					subNodes.Add(new FileSystemItem(
						vg.Key.Value.ToString(),
						null,
						null,
						VehicleType: vg.Key.Value,
						SubNodes: vehicleSubNodes));
				}
			}
			else
			{
				subNodes = new ObservableCollection<FileSystemItem>(objGroup
					.Select(x => IndexEntryToFileSystemItem(x, baseDirectory, fileLocation))
					.OrderBy(x => x.DisplayName));
			}

			result.Add(new FileSystemItem(
				objGroup.Key.ToString(),
				null,
				null,
				ObjectType: objGroup.Key,
				SubNodes: subNodes));
		}

		return result;
	}
}
