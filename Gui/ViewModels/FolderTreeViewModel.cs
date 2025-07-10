using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Threading;
using OpenLoco.Dat.Data;
using OpenLoco.Definitions.Database;
using OpenLoco.Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OpenLoco.Gui.ViewModels
{
	public class DesignerFolderTreeViewModel : FolderTreeViewModel
	{
		public DesignerFolderTreeViewModel()
		{
			SelectedTabIndex = 0;
			CurrentLocalDirectory = "test/directory";
			LocalDirectoryItems = [new("local-displayname1", "local-filename1", null)];
			OnlineDirectoryItems = [new("online-displayname1", null, 123)];

			UpdateDirectoryItemsView();
		}
	}

	public class FolderTreeViewModel : ReactiveObject
	{
		public HierarchicalTreeDataGridSource<FileSystemItem> TreeDataGridSource { get; set; }
		ObservableCollection<FileSystemItem> treeDataGridSource;

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
		public string FilenameFilter { get; set; } = string.Empty;

		[Reactive]
		public string AuthorFilter { get; set; } = string.Empty;

		[Reactive]
		public string ModpackFilter { get; set; } = string.Empty;

		[Reactive]
		public ObjectDisplayMode DisplayMode { get; set; } = ObjectDisplayMode.All;

		[Reactive]
		protected List<FileSystemItem> LocalDirectoryItems { get; set; } = [];

		[Reactive]
		protected List<FileSystemItem> OnlineDirectoryItems { get; set; } = [];

		[Reactive]
		public float IndexOrDownloadProgress { get; set; }

		Progress<float> Progress { get; } = new();

		public ReactiveCommand<Unit, Task>? RefreshDirectoryItems { get; }

		public ReactiveCommand<Unit, Unit>? OpenCurrentFolder { get; }

		public ReactiveCommand<FileSystemItem, Unit>? OpenFolderFor { get; }

		public ObservableCollection<ObjectDisplayMode> DisplayModeItems { get; } = [.. Enum.GetValues<ObjectDisplayMode>()];

		[Reactive]
		public int SelectedTabIndex { get; set; }

		public bool IsLocal
			=> SelectedTabIndex == 0;
		public string RecreateText
			=> IsLocal ? "Recreate index" : "Download object list";

		public string DirectoryFileCount
			=> $"Objects: {CurrentDirectoryItems.Sum(CountNodes)}";

		// used for design-time view
		public FolderTreeViewModel()
		{ }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;
			Progress.ProgressChanged += (_, progress) => IndexOrDownloadProgress = progress;

			RefreshDirectoryItems = ReactiveCommand.Create(() => ReloadDirectoryAsync(false));
			OpenCurrentFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(IsLocal ? CurrentLocalDirectory : Model.Settings.DownloadFolder, Model.Logger));

			OpenFolderFor = ReactiveCommand.Create((FileSystemItem clickedOn) =>
			{
				if (IsLocal
				&& clickedOn is FileSystemItem clickedOnObject
				&& clickedOnObject.FileLocation == FileLocation.Local
				&& (clickedOnObject.SubNodes == null || clickedOnObject.SubNodes.Count == 0)
				&& File.Exists(clickedOnObject.FileName))
				{
					var dir = Directory.GetParent(clickedOnObject.FileName)?.FullName;
					PlatformSpecific.FolderOpenInDesktop(dir, Model.Logger, Path.GetFileName(clickedOnObject.FileName));

				}
			});

			_ = this.WhenAnyValue(o => o.CurrentLocalDirectory)
				.Skip(1)
				.Subscribe(async _ => await ReloadDirectoryAsync(true));

			_ = this.WhenAnyValue(o => o.CurrentLocalDirectory)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));

			_ = this.WhenAnyValue(o => o.DisplayMode)
				.Throttle(TimeSpan.FromMilliseconds(1000))
				.Skip(1)
				.Subscribe(async _ => await ReloadDirectoryAsync(true));

			_ = this.WhenAnyValue(o => o.FilenameFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Skip(1)
				.Subscribe(async _ => await ReloadDirectoryAsync(true));

			_ = this.WhenAnyValue(o => o.AuthorFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Skip(1)
				.Subscribe(async _ => await ReloadDirectoryAsync(true));

			_ = this.WhenAnyValue(o => o.ModpackFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Skip(1)
				.Subscribe(async _ => await ReloadDirectoryAsync(true));

			_ = this.WhenAnyValue(o => o.TreeDataGridSource)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			_ = this.WhenAnyValue(o => o.TreeDataGridSource)
				.Skip(1)
				.Subscribe(_ => CurrentlySelectedObject = null);

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => UpdateDirectoryItemsView());

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(RecreateText)));

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));

			_ = this.WhenAnyValue(o => o.LocalDirectoryItems)
				//.Skip(1)
				.Subscribe(_ => UpdateDirectoryItemsView());

			_ = this.WhenAnyValue(o => o.OnlineDirectoryItems)
				.Skip(1)
				.Subscribe(_ => UpdateDirectoryItemsView());

			// loads the last-viewed folder
			CurrentLocalDirectory = Model.Settings.ObjDataDirectory;
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

		List<FileSystemItem> CurrentDirectoryItems => IsLocal ? LocalDirectoryItems : OnlineDirectoryItems;

		protected void UpdateDirectoryItemsView()
			=> UpdateGrid(CurrentDirectoryItems);

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
				LocalDirectoryItems = [];
				return;
			}

			await Model.LoadObjDirectoryAsync(directory, Progress, useExistingIndex);
			LocalDirectoryItems = ConstructTreeView(
				Model.ObjectIndex.Objects.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes),
				Model.Settings.ObjDataDirectory,
				FilenameFilter,
				AuthorFilter,
				ModpackFilter,
				DisplayMode,
				FileLocation.Local);

			UpdateGrid(LocalDirectoryItems);
		}

		void UpdateGrid(List<FileSystemItem> items)
		{
			treeDataGridSource = [.. items];
			TreeDataGridSource = new HierarchicalTreeDataGridSource<FileSystemItem>(treeDataGridSource)
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
				OnlineDirectoryItems = ConstructTreeView(
					Model.ObjectIndexOnline.Objects.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes),
					Model.Settings.DownloadFolder,
					FilenameFilter,
					AuthorFilter,
					ModpackFilter,
					DisplayMode,
					FileLocation.Online);

				UpdateGrid(OnlineDirectoryItems);
			}
		}

		static bool MatchesFilter(ObjectIndexEntry o, string filenameFilter, string authorFilter, string modpackFilter, ObjectDisplayMode displayMode)
		{
			var displayable = displayMode == ObjectDisplayMode.All || (displayMode == ObjectDisplayMode.Vanilla == (o.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG));

			var filters =
				   string.IsNullOrEmpty(filenameFilter) || o.DisplayName.Contains(filenameFilter, StringComparison.CurrentCultureIgnoreCase);
			//&& (string.IsNullOrEmpty(authorFilter)   || o.Author.Contains(authorFilter, StringComparison.CurrentCultureIgnoreCase))
			//&& (string.IsNullOrEmpty(modpackFilter)  || o.DatName.Contains(modpackFilter, StringComparison.CurrentCultureIgnoreCase));

			return displayable && filters;
		}

		public static FileSystemItem IndexEntryToFileSystemItem(ObjectIndexEntry x, string baseDirectory, FileLocation fileLocation)
		{
			var computedFileName = fileLocation == FileLocation.Online ? $"{x.DisplayName}-{x.Id}.dat" : x.FileName;
			// fallback - DisplayName is never null
			computedFileName ??= x.DisplayName;

			return new FileSystemItem(x.DisplayName, Path.Combine(baseDirectory, computedFileName), x.Id, x.CreatedDate, x.ModifiedDate, fileLocation, x.ObjectSource);
		}

		static List<FileSystemItem> ConstructTreeView(IEnumerable<ObjectIndexEntry> index, string baseDirectory, string filenameFilter, string authorFilter, string modpackFilter, ObjectDisplayMode displayMode, FileLocation fileLocation)
		{
			var result = new List<FileSystemItem>();
			var groupedObjects = index
				.OfType<ObjectIndexEntry>() // this won't show errored files - should we??
				.Where(x => MatchesFilter(x, filenameFilter, authorFilter, modpackFilter, displayMode))
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
}
