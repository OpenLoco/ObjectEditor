using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Media;
using Avalonia.Threading;
using Common;
using Dat.Data;
using Definitions.DTO;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using Definitions.Web;
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
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class DesignerFolderTreeViewModel : FolderTreeViewModel
{
	public DesignerFolderTreeViewModel()
	{
		IsLocal = true;
		CurrentLocalDirectory = "test/directory";
		CurrentDirectoryItems.Add(new(
			"local-displayname1",
			"local-filename1",
			null,
			null,
			null,
			FileLocation.Local,
			ObjectSource.Custom,
			ObjectType.Airport,
			null,
			null));

		var availableFilterCategories = new List<FilterTypeViewModel>
		{
			new() { Type = typeof(FileSystemItem), DisplayName = "Index data", IconName = nameof(FileSystemItem) },
			new() { Type = typeof (ObjectMetadata), DisplayName = "Metadata", IconName = nameof(ObjectMetadata) }
		};

		//Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
		//Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
	}
}

public class FolderTreeViewModel : ReactiveObject
{
	[Reactive]
	protected SourceList<FileSystemItem> CurrentDirectoryItems { get; set; } = new();

	public HierarchicalTreeDataGridSource<FileSystemItem> TreeDataGridSource { get; set; }
	ReadOnlyObservableCollection<FileSystemItem> treeDataGridSource;
	public int TreeDataGridSourceCount => treeDataGridSource?.Count ?? 0;

	public ObservableCollection<FilterViewModel> Filters { get; } = [];
	public ReactiveCommand<Unit, Unit> AddFilterCommand { get; }
	public ReactiveCommand<Unit, Unit> ExpandAllCommand { get; }
	public ReactiveCommand<Unit, Unit> CollapseAllCommand { get; }

	private readonly BehaviorSubject<Func<FileSystemItem, bool>> _filterSubject;

	ObjectEditorContext EditorContext { get; init; }

	[Reactive]
	public string CurrentLocalDirectory { get; set; } = string.Empty;
	public string CurrentDirectory => IsLocal
		? CurrentLocalDirectory
		: EditorContext.Settings.UseHttps
			? EditorContext.Settings.ServerAddressHttps
			: EditorContext.Settings.ServerAddressHttp;

	[Reactive]
	public FileSystemItem? CurrentlySelectedObject { get; set; }

	[Reactive]
	public int IndexOrDownloadProgress { get; set; }

	Progress<float> Progress { get; } = new();

	public ReactiveCommand<Unit, Unit>? RefreshDirectoryItems { get; }
	public ReactiveCommand<Unit, Unit>? OpenCurrentFolder { get; }
	public ReactiveCommand<FileSystemItem, Unit>? OpenFolderFor { get; }

	public ObservableCollection<ObjectDisplayMode> DisplayModeItems { get; } = [.. Enum.GetValues<ObjectDisplayMode>()];
	static OnlineBrowseTargetOption DefaultOnlineBrowseTarget { get; } = new(OnlineApiEndpointGroup.Objects, "Objects", "Objects", Client.ObjectsEndpointGroup);
	public ObservableCollection<OnlineBrowseTargetOption> OnlineBrowseTargets { get; } =
	[
		DefaultOnlineBrowseTarget,
		new(OnlineApiEndpointGroup.ObjectPacks, "Object packs", "Object packs", Client.ObjectPacksEndpointGroup),
		new(OnlineApiEndpointGroup.Scenarios, "Scenarios", "Scenarios", Client.ScenariosEndpointGroup),
		new(OnlineApiEndpointGroup.SC5FilePacks, "SC5 file packs", "SC5 file packs", Client.SC5FilePacksEndpointGroup),
		new(OnlineApiEndpointGroup.Tags, "Tags", "Tags", Client.TagsEndpointGroup),
		new(OnlineApiEndpointGroup.Authors, "Authors", "Authors", Client.AuthorsEndpointGroup),
		new(OnlineApiEndpointGroup.Licences, "Licences", "Licences", Client.LicencesEndpointGroup),
		new(OnlineApiEndpointGroup.MissingObjects, "Missing objects", "Missing objects", Client.MissingObjectsEndpointGroup),
	];

	[Reactive]
	public int SelectedTabIndex { get; set; }

	[Reactive]
	public OnlineBrowseTargetOption SelectedOnlineBrowseTarget { get; set; } = DefaultOnlineBrowseTarget;

	readonly Dictionary<OnlineApiEndpointGroup, IReadOnlyList<FileSystemItem>> onlineDirectoryItemsCache = [];

	public bool IsLocal
	{
		get => SelectedTabIndex == 0;
		set => SelectedTabIndex = value ? 0 : 1;
	}

	public string RecreateText
		=> IsLocal
			? "Recreate index"
			: $"Download {CurrentItemLabelPlural.ToLowerInvariant()} list";

	public string DirectoryFileCount
		=> $"{CurrentItemLabelPlural}: {CurrentDirectoryItems.Count}";

	public string CurrentItemLabelPlural
		=> IsLocal
			? "Objects"
			: SelectedOnlineBrowseTarget?.ItemLabelPlural ?? "Items";

	public bool IsOnline
		=> !IsLocal;

	public FolderTreeViewModel() { }

	public FolderTreeViewModel(ObjectEditorContext editorContext)
	{
		EditorContext = editorContext;
		SelectedOnlineBrowseTarget = OnlineBrowseTargets.First();
		Progress.ProgressChanged += (_, progress) => IndexOrDownloadProgress = (int)(progress * 100);

		var indexFilterModel = new FilterTypeViewModel() { Type = typeof(FileSystemItem), DisplayName = "Index data", IconName = nameof(FileSystemItem) };
		var availableFilterCategories = new List<FilterTypeViewModel>
		{
			indexFilterModel,
			//new() { Type = typeof (MetadataModel), DisplayName = "Metadata", IconName = nameof(MetadataModel) }
		};

		// todo: add in object-specific searches
		var objBrush = new SolidColorBrush(Color.FromArgb(0x30, 0x80, 0x80, 0x80));
		foreach (var obj in Enum.GetValues<ObjectType>().OrderBy(x => x.ToString()))
		{
			var typeOfObj = ObjectTypeMapping.ObjectTypeToStructType(obj);
			availableFilterCategories.Add(new()
			{
				Type = typeOfObj,
				DisplayName = typeOfObj.Name.Replace("Object", string.Empty),
				IconName = typeOfObj.Name.Replace("Object", string.Empty),
				BackgroundColour = objBrush
			});
		}

		RefreshDirectoryItems = ReactiveCommand.CreateFromTask(async () => await LoadDirectoryAsync(false));
		OpenCurrentFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(IsLocal ? CurrentLocalDirectory : this.EditorContext.Settings.DownloadFolder, this.EditorContext.Logger));
		AddFilterCommand = ReactiveCommand.Create(() => Filters.Add(new FilterViewModel(this.EditorContext, availableFilterCategories, RemoveFilter)));
		OpenFolderFor = ReactiveCommand.Create((FileSystemItem clickedOn) =>
		{
			if (IsLocal
			&& clickedOn is FileSystemItem clickedOnObject
			&& clickedOnObject.FileLocation == FileLocation.Local
			&& (clickedOnObject.SubNodes == null || clickedOnObject.SubNodes.Count == 0)
			&& File.Exists(clickedOnObject.FileName))
			{
				var dir = Directory.GetParent(clickedOnObject.FileName)?.FullName;
				PlatformSpecific.FolderOpenInDesktop(dir, this.EditorContext.Logger, Path.GetFileName(clickedOnObject.FileName));

			}
		});

		ExpandAllCommand = ReactiveCommand.Create(() =>
		{
			if (TreeDataGridSource is HierarchicalTreeDataGridSource<FileSystemItem> htgds)
			{
				htgds?.ExpandAll();
			}
		});

		CollapseAllCommand = ReactiveCommand.Create(() =>
		{
			if (TreeDataGridSource is HierarchicalTreeDataGridSource<FileSystemItem> htgds)
			{
				htgds?.CollapseAll();
			}
		});

		_filterSubject = new BehaviorSubject<Func<FileSystemItem, bool>>(t => true);

		var filtersChanged = Filters.ToObservableChangeSet()
			.Skip(1)
			.AutoRefresh(f => f.SelectedProperty)
			.AutoRefresh(f => f.SelectedOperator)
			.AutoRefresh(f => f.BoolValue)
			.AutoRefresh(f => f.DateValue)
			.AutoRefresh(f => f.EnumValue)
			.AutoRefresh(f => f.TextValue)
			.ToCollection()
			.Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
			.Select(_ => Observable.Start(CreateFilterPredicate, RxApp.TaskpoolScheduler))
			.Switch()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Subscribe(_filterSubject);

		_ = CurrentDirectoryItems.Connect()
			.Filter(_filterSubject)
			.Bind(out treeDataGridSource)
			.Subscribe(_ => UpdateDirectoryItemsView());

		_ = this.WhenAnyValue(x => x.TreeDataGridSource)
			.Where(x => x?.RowSelection != null)
			.Select(x => Observable.FromEventPattern<TreeSelectionModelSelectionChangedEventArgs<FileSystemItem>>(x.RowSelection, nameof(x.RowSelection.SelectionChanged)))
			.Switch()
			.Subscribe(e =>
			{
				CurrentlySelectedObject = e.EventArgs.SelectedItems.Count == 1
					? e.EventArgs.SelectedItems[0]
					: null;
			});

		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(async _ => await LoadDirectoryAsync(true));
		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));

		//_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => UpdateDirectoryItemsView());
		_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));
		_ = this.WhenAnyValue(o => o.CurrentDirectory).Skip(1).Subscribe(async _ => await LoadDirectoryAsync(true));
		_ = this.WhenAnyValue(o => o.CurrentDirectoryItems).Skip(1).Subscribe(_ => UpdateDirectoryItemsView());
		_ = this.WhenAnyValue(o => o.SelectedOnlineBrowseTarget).Skip(1).Subscribe(async _ => await LoadDirectoryAsync(false));

		_ = this.WhenAnyValue(o => o.TreeDataGridSource).Skip(1).Subscribe(_ =>
		{
			CurrentlySelectedObject = null;
			this.RaisePropertyChanged(nameof(DirectoryFileCount));
			this.RaisePropertyChanged(nameof(TreeDataGridSourceCount));
			this.RaisePropertyChanged(nameof(RecreateText));
			this.RaisePropertyChanged(nameof(CurrentDirectory));
			this.RaisePropertyChanged(nameof(CurrentItemLabelPlural));
			this.RaisePropertyChanged(nameof(IsOnline));
		});

		CurrentLocalDirectory = this.EditorContext.Settings.ObjDataDirectory;

		// add default name filter
		var defaultFilter = new FilterViewModel(this.EditorContext, availableFilterCategories, RemoveFilter)
		{
			SelectedObjectType = indexFilterModel,
			SelectedOperator = FilterOperator.Contains,
		};
		defaultFilter.SelectedProperty = defaultFilter.AvailableProperties.FirstOrDefault(p => p.Name == nameof(FileSystemItem.DisplayName));
		Filters.Add(defaultFilter);
	}

	protected void RemoveFilter(FilterViewModel filter)
		=> Filters.Remove(filter);

	// this needs to be async as it blocks the UI when building expressions is slow
	private Func<FileSystemItem, bool> CreateFilterPredicate()
	{
		var filterDelegates = new List<Func<FileSystemItem, bool>>();

		foreach (var filter in Filters.Where(x => x.IsValid))
		{
			try
			{
				var filterDelegate = filter.BuildExpression(IsLocal);
				if (filterDelegate != null)
				{
					filterDelegates.Add(filterDelegate);
				}
			}
			catch (Exception ex)
			{
				// Log or handle the exception as needed
				Console.WriteLine($"Error building filter expression: {ex.Message}");
			}
		}

		if (filterDelegates.Count == 0)
		{
			return _ => true;
		}

		return entry => filterDelegates.All(filter => filter(entry));
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

	async Task LoadDirectoryAsync(bool useExistingIndex)
	{
		EditorContext.Logger.Debug($"UseExistingIndex={useExistingIndex}");

		if (IsLocal)
		{
			// local
			await LoadObjDirectoryAsync(CurrentLocalDirectory, useExistingIndex);
		}
		else // remote
		{
			await LoadOnlineDirectoryAsync(useExistingIndex);
		}

		await EditorContext.CheckForDatFilesNotOnServer();
	}

	async Task LoadObjDirectoryAsync(string directory, bool useExistingIndex)
	{
		EditorContext.Logger.Debug($"Directory={directory} UseExistingIndex={useExistingIndex}");

		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			CurrentDirectoryItems.Clear();
			return;
		}

		await EditorContext.LoadObjDirectoryAsync(directory, Progress, useExistingIndex);

		if (EditorContext.ObjectIndex != null)
		{
			var items = EditorContext.ObjectIndex.Objects.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes);
			CurrentDirectoryItems.Clear();
			CurrentDirectoryItems.AddRange(items.Select(x => IndexEntryToFileSystemItem(x, directory, FileLocation.Local)));
		}

		UpdateDirectoryItemsView();
	}

	async Task LoadOnlineDirectoryAsync(bool useExistingIndex)
	{
		EditorContext.Logger.Debug($"UseExistingIndex={useExistingIndex}");

		if (Design.IsDesignMode)
		{
			// DO NOT WEB QUERY AT DESIGN TIME
			return;
		}

		var items = await GetOnlineDirectoryItemsAsync(useExistingIndex);
		CurrentDirectoryItems.Clear();
		CurrentDirectoryItems.AddRange(items);

		UpdateDirectoryItemsView();
	}

	protected void UpdateDirectoryItemsView()
	{
		var _treeGridDataSource = ConstructTreeView(treeDataGridSource);

		TreeDataGridSource = new HierarchicalTreeDataGridSource<FileSystemItem>(_treeGridDataSource)
		{
			Columns =
			{
				new HierarchicalExpanderColumn<FileSystemItem>(
					new TemplateColumn<FileSystemItem>(
						string.Empty, // the column name. it looks better with no name
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
				//new TextColumn<FileSystemItem, string?>("Location", x => x.FileName),
				new TextColumn<FileSystemItem, DateOnly?>("Created", x => x.CreatedDate),
				new TextColumn<FileSystemItem, DateOnly?>("Modified", x => x.ModifiedDate),
				new TextColumn<FileSystemItem, ObjectType?>("Type", x => x.ObjectType),
			},
		};

		Dispatcher.UIThread.Invoke(new Action(() =>
		{
			if (Filters.Any() && Filters.All(x => x.IsValid) && CurrentDirectoryItems.Count != TreeDataGridSourceCount)
			{
				TreeDataGridSource.ExpandAll();
			}
		}));

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
			_ => throw new NotImplementedException($"Unsupported object source: {os}"),
		};

	public static FileSystemItem IndexEntryToFileSystemItem(ObjectIndexEntry x, string baseDirectory, FileLocation fileLocation)
	{
		var computedFileName = fileLocation == FileLocation.Online ? $"{x.DisplayName}-{x.Id}.dat" : x.FileName;
		// fallback - DisplayName is never null
		computedFileName ??= x.DisplayName;

		return new FileSystemItem(x.DisplayName, Path.Combine(baseDirectory, computedFileName), x.Id, x.CreatedDate, x.ModifiedDate, fileLocation, x.ObjectSource, x.ObjectType, x.VehicleType)
		{
			DatChecksum = x.DatChecksum,
			xxHash3 = x.xxHash3,
			OnlineApiEndpointGroup = fileLocation == FileLocation.Online ? OnlineApiEndpointGroup.Objects : null,
		};
	}

	static List<FileSystemItem> ConstructTreeView(IEnumerable<FileSystemItem> items)
	{
		if (!items.Any(x => x.ObjectType != null))
		{
			return [.. items.OrderBy(x => x.DisplayName)];
		}

		var result = new List<FileSystemItem>();
		var groupedObjects = items
			.Where(x => x.ObjectType != null)
			.GroupBy(x => x.ObjectType!.Value)
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
					.OrderBy(x => x.DisplayName));
			}

			result.Add(new FileSystemItem(
				objGroup.Key.ToString(),
				null,
				null,
				//ObjectType: objGroup.Key,
				SubNodes: subNodes));
		}

		return result;
	}

	async Task<IReadOnlyList<FileSystemItem>> GetOnlineDirectoryItemsAsync(bool useExistingIndex)
	{
		if (EditorContext.ObjectServiceClient == null)
		{
			return [];
		}

		var selectedGroup = SelectedOnlineBrowseTarget.Group;
		if (useExistingIndex && onlineDirectoryItemsCache.TryGetValue(selectedGroup, out var existingItems))
		{
			return existingItems;
		}

		var items = selectedGroup switch
		{
			OnlineApiEndpointGroup.Objects => await GetOnlineObjectDirectoryItemsAsync(useExistingIndex),
			OnlineApiEndpointGroup.ObjectPacks => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoItemPackEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineObjectPackFileSystemItem)],
			OnlineApiEndpointGroup.Scenarios => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoScenarioEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineScenarioFileSystemItem)],
			OnlineApiEndpointGroup.SC5FilePacks => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoItemPackEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineSC5FilePackFileSystemItem)],
			OnlineApiEndpointGroup.Tags => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoTagEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineTagFileSystemItem)],
			OnlineApiEndpointGroup.Authors => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoAuthorEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineAuthorFileSystemItem)],
			OnlineApiEndpointGroup.Licences => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoLicenceEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineLicenceFileSystemItem)],
			OnlineApiEndpointGroup.MissingObjects => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoObjectMissingEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.DatName)
				.Select(CreateOnlineMissingObjectFileSystemItem)],
			_ => throw new NotImplementedException($"Unsupported endpoint group: {selectedGroup}"),
		};

		onlineDirectoryItemsCache[selectedGroup] = items;
		return items;
	}

	async Task<IReadOnlyList<FileSystemItem>> GetOnlineObjectDirectoryItemsAsync(bool useExistingIndex)
	{
		if ((!useExistingIndex || EditorContext.ObjectIndexOnline == null) && EditorContext.ObjectServiceClient != null)
		{
			EditorContext.ObjectIndexOnline = new ObjectIndex((await EditorContext.ObjectServiceClient.GetListAsync<DtoObjectEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
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

		return EditorContext.ObjectIndexOnline == null
			? []
			: [.. EditorContext.ObjectIndexOnline.Objects
				.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes)
				.Select(x => IndexEntryToFileSystemItem(x, EditorContext.Settings.DownloadFolder, FileLocation.Online))];
	}

	static FileSystemItem CreateOnlineObjectPackFileSystemItem(DtoItemPackEntry item)
		=> new(item.Name, null, item.Id, item.CreatedDate, item.ModifiedDate, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.ObjectPacks,
		};

	static FileSystemItem CreateOnlineSC5FilePackFileSystemItem(DtoItemPackEntry item)
		=> new(item.Name, null, item.Id, item.CreatedDate, item.ModifiedDate, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.SC5FilePacks,
		};

	static FileSystemItem CreateOnlineScenarioFileSystemItem(DtoScenarioEntry item)
		=> new(item.Name, null, item.Id, null, null, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Scenarios,
		};

	static FileSystemItem CreateOnlineTagFileSystemItem(DtoTagEntry item)
		=> new(item.Name, null, item.Id, null, null, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Tags,
		};

	static FileSystemItem CreateOnlineAuthorFileSystemItem(DtoAuthorEntry item)
		=> new(item.Name, null, item.Id, null, null, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Authors,
		};

	static FileSystemItem CreateOnlineLicenceFileSystemItem(DtoLicenceEntry item)
		=> new(item.Name, null, item.Id, null, null, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Licences,
		};

	static FileSystemItem CreateOnlineMissingObjectFileSystemItem(DtoObjectMissingEntry item)
		=> new(item.DatName, null, item.Id, null, null, FileLocation.Online, ObjectType: item.ObjectType)
		{
			DatChecksum = item.DatChecksum,
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.MissingObjects,
		};
}
