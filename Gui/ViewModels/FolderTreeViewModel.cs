using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Common;
using Dat.Data;
using Definitions.DTO;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using DynamicData;
using DynamicData.Binding;
using Gui.Models;
using Gui.ViewModels.Filters;
using Index;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
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
		CurrentDirectoryItems.AddRange(
		  [
			  new(
				"Andromeda International",
				"test/directory/airports/andromeda_international.dat",
				null,
				new DateOnly(2025, 1, 8),
				new DateOnly(2025, 3, 14),
				FileLocation.Local,
				ObjectSource.LocomotionSteam,
				ObjectType.Airport),
			new(
				"Crescent Central",
				"test/directory/buildings/crescent_central.dat",
				null,
				new DateOnly(2024, 11, 20),
				new DateOnly(2025, 2, 2),
				FileLocation.Local,
				ObjectSource.LocomotionGoG,
				ObjectType.Building),
			new(
				"Westhaven Steelworks",
				"test/directory/industries/westhaven_steelworks.dat",
				null,
				new DateOnly(2024, 10, 4),
				new DateOnly(2025, 1, 17),
				FileLocation.Local,
				ObjectSource.OpenLoco,
				ObjectType.Industry),
			new(
			 "Maple Street Road Stop",
				"test/directory/stations/maple_street_road_stop.dat",
				null,
				new DateOnly(2025, 2, 10),
				new DateOnly(2025, 2, 28),
				FileLocation.Local,
				ObjectSource.Custom,
				ObjectType.RoadStation),
			new(
				"Atlas Express",
				"test/directory/vehicles/atlas_express.dat",
				null,
				new DateOnly(2024, 9, 12),
				new DateOnly(2025, 1, 9),
				FileLocation.Local,
				ObjectSource.Custom,
				ObjectType.Vehicle,
				VehicleType.Train),
			new(
				"Harbor Queen",
				"test/directory/vehicles/harbor_queen.dat",
				null,
				new DateOnly(2024, 12, 1),
				new DateOnly(2025, 2, 6),
				FileLocation.Local,
				ObjectSource.Custom,
				ObjectType.Vehicle,
				VehicleType.Ship),
			new(
				"CityLink Tram",
				"test/directory/vehicles/citylink_tram.dat",
				null,
				new DateOnly(2025, 1, 30),
				new DateOnly(2025, 3, 5),
				FileLocation.Local,
				ObjectSource.Custom,
				ObjectType.Vehicle,
				VehicleType.Tram),
			new(
				"Oak Valley Trees",
				"test/directory/walls/oak_valley_trees.dat",
				null,
				new DateOnly(2024, 8, 19),
				new DateOnly(2024, 12, 22),
				FileLocation.Local,
				ObjectSource.Custom,
				ObjectType.Tree),
		]);

		UpdateDirectoryItemsView();

		var availableFilterCategories = new List<FilterTypeViewModel>
		{
			new() { Type = typeof(FileSystemItem), DisplayName = "Index data", IconName = nameof(FileSystemItem) },
			new() { Type = typeof (ObjectMetadata), DisplayName = "Metadata", IconName = nameof(ObjectMetadata) }
		};

		//Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
		//Filters.Add(new FilterViewModel(availableFilterCategories, RemoveFilter));
	}
}

public class DesignerOnlineBrowseResultsViewModel : FolderTreeViewModel
{
	public DesignerOnlineBrowseResultsViewModel()
	{
		SelectedTabIndex = 1;
		SelectedOnlineBrowseTarget = new OnlineBrowseTargetOption(OnlineApiEndpointGroup.ObjectPacks, "Object packs", "Object packs", Client.ObjectPacksEndpointGroup);

		CurrentOnlineBrowseResults.Add(new OnlineItemPackBrowseResult(
			default,
			"Industrial Expansion Vol. 2",
			"A curated pack of late-game factories, freight stock, and heavy infrastructure objects.",
			new DateOnly(2025, 10, 12),
			new DateOnly(2026, 3, 18),
			new DateOnly(2026, 3, 19),
			new DtoLicenceEntry(default, "CC BY-SA 4.0", "Share and adapt with attribution."),
			[new DtoAuthorEntry(default, "Alex Mercer"), new DtoAuthorEntry(default, "Jordan Pike")],
			[new DtoTagEntry(default, "industry"), new DtoTagEntry(default, "steel"), new DtoTagEntry(default, "late-game")],
			[
				new FileSystemItem("Red River Steelworks", null, default, new DateOnly(2025, 10, 12), new DateOnly(2026, 3, 18), FileLocation.Online, ObjectSource.Custom, ObjectType.Industry),
				new FileSystemItem("Heavy Haul Hopper", null, default, new DateOnly(2025, 10, 12), new DateOnly(2026, 3, 18), FileLocation.Online, ObjectSource.Custom, ObjectType.Vehicle, VehicleType.Train),
				new FileSystemItem("Portside Container Crane", null, default, new DateOnly(2025, 10, 12), new DateOnly(2026, 3, 18), FileLocation.Online, ObjectSource.Custom, ObjectType.Scaffolding)
			],
			OnlineApiEndpointGroup.ObjectPacks));

		CurrentOnlineBrowseResults.Add(new OnlineItemPackBrowseResult(
			default,
			"Continental Challenge Scenarios",
			"Five medium-to-large maps focused on passenger demand and mountainous routing.",
			new DateOnly(2025, 7, 4),
			null,
			new DateOnly(2026, 2, 2),
			null,
			[new DtoAuthorEntry(default, "Marin Voss")],
			[new DtoTagEntry(default, "scenario"), new DtoTagEntry(default, "challenging")],
			[
				new FileSystemItem("Highland Corridor", null, default, null, null, FileLocation.Online, ObjectType: ObjectType.ScenarioText),
				new FileSystemItem("Three Seas Express", null, default, null, null, FileLocation.Online, ObjectType: ObjectType.ScenarioText)
			],
			OnlineApiEndpointGroup.SC5FilePacks));

		CurrentOnlineBrowseResults.Add(new OnlineLicenceBrowseResult(
			default,
			"OpenLoco Community Licence",
			"Free to use, modify, and redistribute within the OpenLoco ecosystem provided authors remain credited and derivatives document substantive changes."));

		CurrentOnlineBrowseResults.Add(new OnlineAuthorBrowseResult(default, "Casey Rowan"));
		CurrentOnlineBrowseResults.Add(new OnlineTagBrowseResult(default, "stations"));
		CurrentOnlineBrowseResults.Add(new OnlineMissingObjectBrowseResult(default, "STEELMILL.DAT", 0x4A91C2F0, ObjectType.Industry));
	}
}

public class FolderTreeViewModel : ReactiveObject, IDisposable
{
	readonly CompositeDisposable _subscriptions = [];
	bool _disposed;

	[Reactive]
	protected SourceList<FileSystemItem> CurrentDirectoryItems { get; set; } = new();

	[Reactive]
	public IEnumerable<FileSystemItem>? TreeRoot { get; set; }

	ReadOnlyObservableCollection<FileSystemItem>? treeDataGridSource;
	public int TreeDataGridSourceCount => treeDataGridSource?.Count ?? 0;

	readonly Subject<Unit> _expandAllRequests = new();
	readonly Subject<Unit> _collapseAllRequests = new();
	public IObservable<Unit> ExpandAllRequests => _expandAllRequests;
	public IObservable<Unit> CollapseAllRequests => _collapseAllRequests;

	public void OnSelectionChanged(IReadOnlyList<FileSystemItem> selectedItems)
		=> CurrentlySelectedObject = selectedItems.Count == 1 ? selectedItems[0] : null;
	public ObservableCollection<object> CurrentOnlineBrowseResults { get; } = [];

	public ObservableCollection<FilterViewModel> Filters { get; } = [];
	public ReactiveCommand<Unit, Unit>? AddFilterCommand { get; }
	public ReactiveCommand<Unit, Unit>? ExpandAllCommand { get; }
	public ReactiveCommand<Unit, Unit>? CollapseAllCommand { get; }

	private readonly BehaviorSubject<Func<FileSystemItem, bool>> _filterSubject = new(t => true);

	ObjectEditorContext EditorContext { get; init; } = null!;

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
	public ReactiveCommand<FileSystemItem, Unit>? SelectOnlineBrowseFileSystemItem { get; }
	public ReactiveCommand<FileSystemItem, Unit>? DownloadOnlineItemCommand { get; private set; }
	public ReactiveCommand<OnlineItemPackBrowseResult, Unit>? DownloadOnlinePackCommand { get; private set; }

	public ObservableCollection<ObjectDisplayMode> DisplayModeItems { get; } = [.. Enum.GetValues<ObjectDisplayMode>()];
	static OnlineBrowseTargetOption ObjectOnlineBrowseTarget { get; } = new(OnlineApiEndpointGroup.Objects, "Objects", "Objects", Client.ObjectsEndpointGroup);
	public ObservableCollection<OnlineBrowseTargetOption> OnlineBrowseTargets { get; } =
	[
		ObjectOnlineBrowseTarget,
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
	public OnlineBrowseTargetOption SelectedOnlineBrowseTarget { get; set; } = ObjectOnlineBrowseTarget;

	readonly Dictionary<OnlineApiEndpointGroup, IReadOnlyList<FileSystemItem>> onlineDirectoryItemsCache = [];
	readonly Dictionary<OnlineApiEndpointGroup, IReadOnlyList<object>> onlineBrowseResultsCache = [];

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
		=> $"{CurrentItemLabelPlural}: {ResultsCount}";

	public string CurrentItemLabelPlural
		=> IsLocal
			? "Objects"
			: SelectedOnlineBrowseTarget?.ItemLabelPlural ?? "Items";

	public bool IsOnline
		=> !IsLocal;

	public bool UsesTreeDataGrid
		=> IsLocal || SelectedOnlineBrowseTarget.Group is OnlineApiEndpointGroup.Objects or OnlineApiEndpointGroup.Scenarios;

	public bool HasOnlineBrowseResults
		=> IsOnline && !UsesTreeDataGrid;

	public bool SupportsFilters
		=> UsesTreeDataGrid;

	public bool SupportsHierarchyCommands
		=> UsesTreeDataGrid;

	public int ResultsCount
		=> UsesTreeDataGrid ? TreeDataGridSourceCount : CurrentOnlineBrowseResults.Count;

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
		SelectOnlineBrowseFileSystemItem = ReactiveCommand.Create<FileSystemItem>(item => CurrentlySelectedObject = item);
		DownloadOnlineItemCommand = ReactiveCommand.CreateFromTask<FileSystemItem>(DownloadOnlineItemAsync);
		DownloadOnlinePackCommand = ReactiveCommand.CreateFromTask<OnlineItemPackBrowseResult>(DownloadOnlinePackAsync);
		OpenFolderFor = ReactiveCommand.Create((FileSystemItem clickedOn) =>
		{
			if (IsLocal
			&& clickedOn is FileSystemItem clickedOnObject
			&& clickedOnObject.FileLocation == FileLocation.Local
			&& (clickedOnObject.SubNodes == null || clickedOnObject.SubNodes.Count == 0)
			&& File.Exists(clickedOnObject.FileName))
			{
				var dir = Directory.GetParent(clickedOnObject.FileName)?.FullName;
				if (!string.IsNullOrEmpty(dir))
				{
					PlatformSpecific.FolderOpenInDesktop(dir, this.EditorContext.Logger, Path.GetFileName(clickedOnObject.FileName));
				}

			}
		});

		ExpandAllCommand = ReactiveCommand.Create(() => _expandAllRequests.OnNext(Unit.Default));
		CollapseAllCommand = ReactiveCommand.Create(() => _collapseAllRequests.OnNext(Unit.Default));

		CurrentOnlineBrowseResults.CollectionChanged += (_, _) =>
		{
			this.RaisePropertyChanged(nameof(ResultsCount));
			this.RaisePropertyChanged(nameof(DirectoryFileCount));
		};

		var filtersChanged = Filters.ToObservableChangeSet()
			.Skip(1)
			.AutoRefresh(f => f.SelectedProperty)
			.AutoRefresh(f => f.SelectedOperator)
			.AutoRefresh(f => f.BoolValue)
			.AutoRefresh(f => f.DateValue)
			.AutoRefresh(f => f.EnumValue)
			.AutoRefresh(f => f.TextValue)
			.ToCollection()
			.Throttle(TimeSpan.FromMilliseconds(500), RxSchedulers.MainThreadScheduler)
			.Select(_ => Observable.Start(CreateFilterPredicate, RxSchedulers.TaskpoolScheduler))
			.Switch()
		   .ObserveOn(RxSchedulers.MainThreadScheduler)
			.Subscribe(_filterSubject)
			.DisposeWith(_subscriptions);

		_ = CurrentDirectoryItems.Connect()
			.Filter(_filterSubject)
			.Bind(out treeDataGridSource)
			.Subscribe(_ => UpdateDirectoryItemsView())
			.DisposeWith(_subscriptions);

		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(async _ => await LoadDirectoryAsync(true)).DisposeWith(_subscriptions);
		_ = this.WhenAnyValue(o => o.CurrentLocalDirectory).Skip(1).Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory))).DisposeWith(_subscriptions);

		//_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => UpdateDirectoryItemsView());
		_ = this.WhenAnyValue(o => o.SelectedTabIndex).Skip(1).Subscribe(_ => RaiseBrowseModeProperties()).DisposeWith(_subscriptions);
		_ = this.WhenAnyValue(o => o.CurrentDirectory).Skip(1).Subscribe(async _ => await LoadDirectoryAsync(true)).DisposeWith(_subscriptions);
		_ = this.WhenAnyValue(o => o.CurrentDirectoryItems).Skip(1).Subscribe(_ => UpdateDirectoryItemsView()).DisposeWith(_subscriptions);
		_ = this.WhenAnyValue(o => o.SelectedOnlineBrowseTarget).Skip(1).Subscribe(async _ =>
		{
			RaiseBrowseModeProperties();
			await LoadDirectoryAsync(false);
		}).DisposeWith(_subscriptions);

		_ = this.WhenAnyValue(o => o.TreeRoot).Skip(1).Subscribe(_ =>
		{
			CurrentlySelectedObject = null;
			this.RaisePropertyChanged(nameof(TreeDataGridSourceCount));
			RaiseBrowseModeProperties();
		}).DisposeWith(_subscriptions);

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

	void RaiseBrowseModeProperties()
	{
		this.RaisePropertyChanged(nameof(DirectoryFileCount));
		this.RaisePropertyChanged(nameof(ResultsCount));
		this.RaisePropertyChanged(nameof(RecreateText));
		this.RaisePropertyChanged(nameof(CurrentDirectory));
		this.RaisePropertyChanged(nameof(CurrentItemLabelPlural));
		this.RaisePropertyChanged(nameof(IsOnline));
		this.RaisePropertyChanged(nameof(UsesTreeDataGrid));
		this.RaisePropertyChanged(nameof(HasOnlineBrowseResults));
		this.RaisePropertyChanged(nameof(SupportsFilters));
		this.RaisePropertyChanged(nameof(SupportsHierarchyCommands));
	}

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
		EditorContext.Logger.LogDebug("UseExistingIndex={UseExistingIndex}", useExistingIndex);

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
		EditorContext.Logger.LogDebug("Directory={Directory} UseExistingIndex={UseExistingIndex}", directory, useExistingIndex);
		CurrentOnlineBrowseResults.Clear();

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
		EditorContext.Logger.LogDebug("UseExistingIndex={UseExistingIndex}", useExistingIndex);

		if (Design.IsDesignMode)
		{
			// DO NOT WEB QUERY AT DESIGN TIME
			return;
		}

		if (UsesTreeDataGrid)
		{
			var items = await GetOnlineTreeItemsAsync(useExistingIndex);
			CurrentOnlineBrowseResults.Clear();
			CurrentDirectoryItems.Clear();
			CurrentDirectoryItems.AddRange(items);
		}
		else
		{
			var results = await GetOnlineBrowseResultsAsync(useExistingIndex);
			CurrentDirectoryItems.Clear();

			CurrentOnlineBrowseResults.Clear();
			CurrentOnlineBrowseResults.AddRange(results);

		}

		//UpdateDirectoryItemsView();
	}

	protected void UpdateDirectoryItemsView()
	{
		IEnumerable<FileSystemItem> treeItems = treeDataGridSource is null
			? CurrentDirectoryItems.Items
			: treeDataGridSource;
		TreeRoot = ConstructTreeView(treeItems);

		Dispatcher.UIThread.Invoke(new Action(() =>
		{
			if (Filters.Any() && Filters.All(x => x.IsValid) && CurrentDirectoryItems.Count != TreeDataGridSourceCount)
			{
				_expandAllRequests.OnNext(Unit.Default);
			}
		}));

		this.RaisePropertyChanged(nameof(TreeRoot));
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
				subNodes = [with(objGroup
					.OrderBy(x => x.DisplayName))];
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

	async Task<IReadOnlyList<FileSystemItem>> GetOnlineTreeItemsAsync(bool useExistingIndex)
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
			OnlineApiEndpointGroup.Scenarios => [.. (await EditorContext.ObjectServiceClient.GetListAsync<DtoScenarioEntry>(SelectedOnlineBrowseTarget.EndpointGroup))
				.OrderBy(x => x.Name)
				.Select(CreateOnlineScenarioFileSystemItem)],
			_ => throw new NotImplementedException($"Unsupported endpoint group: {selectedGroup}"),
		};

		onlineDirectoryItemsCache[selectedGroup] = items;
		return items;
	}

	async Task<IReadOnlyList<object>> GetOnlineBrowseResultsAsync(bool useExistingIndex)
	{
		if (EditorContext.ObjectServiceClient == null)
		{
			return [];
		}

		var selectedGroup = SelectedOnlineBrowseTarget.Group;
		if (useExistingIndex && onlineBrowseResultsCache.TryGetValue(selectedGroup, out var existingItems))
		{
			return existingItems;
		}

		IReadOnlyList<object> items = selectedGroup switch
		{
			OnlineApiEndpointGroup.ObjectPacks => [.. (await GetOnlineObjectPackBrowseResultsAsync()).Cast<object>()],
			OnlineApiEndpointGroup.SC5FilePacks => [.. (await GetOnlineSC5FilePackBrowseResultsAsync()).Cast<object>()],
			OnlineApiEndpointGroup.Tags => [.. (await EditorContext.ObjectServiceClient.GetTagsAsync())
				.OrderBy(x => x.Name)
				.Select(x => (object)new OnlineTagBrowseResult(x.Id, x.Name))],
			OnlineApiEndpointGroup.Authors => [.. (await EditorContext.ObjectServiceClient.GetAuthorsAsync())
				.OrderBy(x => x.Name)
				.Select(x => (object)new OnlineAuthorBrowseResult(x.Id, x.Name))],
			OnlineApiEndpointGroup.Licences => [.. (await EditorContext.ObjectServiceClient.GetLicencesAsync())
				.OrderBy(x => x.Name)
				.Select(x => (object)new OnlineLicenceBrowseResult(x.Id, x.Name, x.Text))],
			OnlineApiEndpointGroup.MissingObjects => [.. (await EditorContext.ObjectServiceClient.GetMissingObjectsAsync())
				.OrderBy(x => x.DatName)
				.Select(x => (object)new OnlineMissingObjectBrowseResult(x.Id, x.DatName, x.DatChecksum, x.ObjectType))],
			_ => throw new NotImplementedException($"Unsupported endpoint group: {selectedGroup}"),
		};

		onlineBrowseResultsCache[selectedGroup] = items;
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

	async Task<IReadOnlyList<OnlineItemPackBrowseResult>> GetOnlineObjectPackBrowseResultsAsync()
	{
		if (EditorContext.ObjectServiceClient == null)
		{
			return [];
		}

		var packs = (await EditorContext.ObjectServiceClient.GetObjectPacksAsync())
			.OrderBy(x => x.Name)
			.ToList();

		return [.. await Task.WhenAll(packs.Select(async pack =>
			CreateOnlineItemPackBrowseResult(
				pack,
				await EditorContext.ObjectServiceClient.GetObjectPackAsync(pack.Id),
				OnlineApiEndpointGroup.ObjectPacks,
				descriptor => descriptor.Items.OrderBy(x => x.DisplayName).Select(CreateOnlineObjectFileSystemItem))))];
	}

	async Task<IReadOnlyList<OnlineItemPackBrowseResult>> GetOnlineSC5FilePackBrowseResultsAsync()
	{
		if (EditorContext.ObjectServiceClient == null)
		{
			return [];
		}

		var packs = (await EditorContext.ObjectServiceClient.GetSC5FilePacksAsync())
			.OrderBy(x => x.Name)
			.ToList();

		return [.. await Task.WhenAll(packs.Select(async pack =>
			CreateOnlineItemPackBrowseResult(
				pack,
				await EditorContext.ObjectServiceClient.GetSC5FilePackAsync(pack.Id),
				OnlineApiEndpointGroup.SC5FilePacks,
				descriptor => descriptor.Items.OrderBy(x => x.Name).Select(CreateOnlineScenarioFileSystemItem))))];
	}

	static OnlineItemPackBrowseResult CreateOnlineItemPackBrowseResult<T>(
		DtoItemPackEntry item,
		DtoItemPackDescriptor<T>? descriptor,
		OnlineApiEndpointGroup group,
		Func<DtoItemPackDescriptor<T>, IEnumerable<FileSystemItem>> itemFactory)
	{
		var authors = descriptor?.Authors.OrderBy(x => x.Name).ToArray() ?? [];
		var tags = descriptor?.Tags.OrderBy(x => x.Name).ToArray() ?? [];
		IReadOnlyList<FileSystemItem> items = descriptor == null ? Array.Empty<FileSystemItem>() : [.. itemFactory(descriptor)];

		return new OnlineItemPackBrowseResult(
			item.Id,
			descriptor?.Name ?? item.Name,
			descriptor?.Description ?? item.Description,
			descriptor?.CreatedDate ?? item.CreatedDate,
			descriptor?.ModifiedDate ?? item.ModifiedDate,
			descriptor?.UploadedDate ?? item.UploadedDate,
			descriptor?.Licence ?? item.Licence,
			authors,
			tags,
			items,
			group);
	}

	FileSystemItem CreateOnlineObjectFileSystemItem(DtoObjectEntry item)
	{
		// temp hack since object service returns these weird <---> names
		var displayName = item.DisplayName == "<--->" ? item.Description : item.DisplayName;
		var computedFileName = Path.Combine(EditorContext.Settings.DownloadFolder, $"{item.DisplayName}-{item.Id}.dat");

		return new FileSystemItem(item.DisplayName, computedFileName, item.Id, item.CreatedDate, item.ModifiedDate, FileLocation.Online, item.ObjectSource, item.ObjectType, item.VehicleType)
		{
			DatChecksum = item.DatChecksum,
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Objects,
		};
	}

	static FileSystemItem CreateOnlineScenarioFileSystemItem(DtoScenarioEntry item)
		=> new(item.Name, null, item.Id, null, null, FileLocation.Online)
		{
			OnlineApiEndpointGroup = OnlineApiEndpointGroup.Scenarios,
		};

	static async Task ShowDownloadFailureDialogAsync(string contentMessage)
	{
		var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
		{
			ContentTitle = "Download failed",
			ContentMessage = contentMessage,
			ButtonDefinitions = ButtonEnum.Ok,
			Icon = Icon.Warning,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Topmost = true,
			ShowInCenter = true,
			SizeToContent = SizeToContent.WidthAndHeight,
			MinHeight = 170,
		});
		_ = await box.ShowAsync();
	}

	async Task DownloadOnlineItemAsync(FileSystemItem item)
	{
		if (EditorContext.ObjectServiceClient == null || item.Id == null)
		{
			return;
		}

		var fileBytes = item.OnlineApiEndpointGroup switch
		{
			OnlineApiEndpointGroup.Objects => await EditorContext.ObjectServiceClient.GetObjectFileAsync(item.Id.Value),
			OnlineApiEndpointGroup.Scenarios => await EditorContext.ObjectServiceClient.GetScenarioFileAsync(item.Id.Value),
			_ => null,
		};

		if (fileBytes == null || fileBytes.Length == 0)
		{
			EditorContext.Logger.LogError("Failed to download \"{DisplayName}\" (Id={Id})", item.DisplayName, item.Id);
			return;
		}

		var extension = item.OnlineApiEndpointGroup == OnlineApiEndpointGroup.Scenarios ? ".SC5" : ".dat";
		var safeName = DownloadNameHelper.SanitizeBaseName(item.DisplayName, stripDirectoryComponents: true, stripExtension: true, fallbackName: "download");
		var filename = Path.Combine(EditorContext.Settings.DownloadFolder, $"{safeName}-{item.Id}{extension}");
		try
		{
			await using var outputStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			await outputStream.WriteAsync(fileBytes);
			EditorContext.Logger.LogInformation("Downloaded \"{DisplayName}\" to \"{Filename}\"", item.DisplayName, filename);
		}
		catch (IOException ex)
		{
			EditorContext.Logger.LogError(ex, "Failed to download \"{DisplayName}\" to \"{Filename}\"", item.DisplayName, filename);
			await ShowDownloadFailureDialogAsync($"Could not create:\n{filename}\n\nThe destination file may already exist, be locked by another process, or the download folder may be unavailable.");
		}
		catch (UnauthorizedAccessException ex)
		{
			EditorContext.Logger.LogError(ex, "Failed to download \"{DisplayName}\" to \"{Filename}\" due to insufficient permissions", item.DisplayName, filename);
			await ShowDownloadFailureDialogAsync($"You do not have permission to write to:\n{filename}\n\nPlease check folder permissions or choose a different download folder.");
		}
	}

	async Task DownloadOnlinePackAsync(OnlineItemPackBrowseResult pack)
	{
		if (EditorContext.ObjectServiceClient == null)
		{
			return;
		}

		var fileBytes = pack.Group switch
		{
			OnlineApiEndpointGroup.ObjectPacks => await EditorContext.ObjectServiceClient.GetObjectPackFileAsync(pack.Id),
			OnlineApiEndpointGroup.SC5FilePacks => await EditorContext.ObjectServiceClient.GetSC5FilePackFileAsync(pack.Id),
			_ => null,
		};

		if (fileBytes == null || fileBytes.Length == 0)
		{
			EditorContext.Logger.LogError("Failed to download pack \"{Name}\" (Id={Id})", pack.Name, pack.Id);
			return;
		}

		var safePackName = DownloadNameHelper.SanitizeBaseName(pack.Name, stripDirectoryComponents: false, stripExtension: false, fallbackName: "pack");
		var filename = Path.Combine(EditorContext.Settings.DownloadFolder, $"{safePackName}-{pack.Id}.zip");
		try
		{
			await using var outputStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			await outputStream.WriteAsync(fileBytes);
			EditorContext.Logger.LogInformation("Downloaded pack \"{Name}\" to \"{Filename}\"", pack.Name, filename);
		}
		catch (IOException ex)
		{
			EditorContext.Logger.LogError(ex, "Failed to download pack \"{Name}\" to \"{Filename}\"", pack.Name, filename);
			await ShowDownloadFailureDialogAsync($"Could not create:\n{filename}\n\nThe destination file may already exist, be locked by another process, or the download folder may be unavailable.");
		}
		catch (UnauthorizedAccessException ex)
		{
			EditorContext.Logger.LogError(ex, "Failed to download pack \"{Name}\" to \"{Filename}\" due to insufficient permissions", pack.Name, filename);
			await ShowDownloadFailureDialogAsync($"You do not have permission to write to:\n{filename}\n\nPlease check folder permissions or choose a different download folder.");
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		_subscriptions.Dispose();
		_filterSubject.Dispose();
		_expandAllRequests.Dispose();
		_collapseAllRequests.Dispose();
		GC.SuppressFinalize(this);
	}
}
