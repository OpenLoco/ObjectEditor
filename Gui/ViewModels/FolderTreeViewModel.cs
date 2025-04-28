using Avalonia.Controls;
using DynamicData.Alias;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;
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
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenLoco.Gui.ViewModels
{
	public enum ComparisonOperation
	{
		EqualTo, LessThan, LessThanOrEqualTo, GreaterThan, GreaterThanOrEqualTo
	}

	// for example search for vanilla/custom/openloco types
	// search by author
	public class Filter
	{
		public PropertyInfo CurrentProperty { get; set; }
		public ComparisonOperation Operation { get; set; } = ComparisonOperation.EqualTo;

		// used in the axaml to bind the combobox to the list of swatches
		public static ComparisonOperation[] OpsArr { get; } = Enum.GetValues<ComparisonOperation>();

		public virtual void GetProperties()
		{
			// return generic properties, eg checksum, source, encoding
			// subclasses override this to return the relevant ILocoStruct properties
		}

		//public
	}

	public class ObjectFilter<T> : Filter where T : ILocoStruct
	{
		public PropertyInfo Property { get; set; }

		// dynamic lookup
		public static PropertyInfo[] ObjectProperties { get; } = typeof(T).GetProperties(); // get only properties we can search by. ues attribute to define those?
	}

	public class FolderTreeViewModel : ReactiveObject
	{
		public ObservableCollection<Filter> Filters { get; set; }
		public ICommand AddNewFilter { get; set; }

		ObjectEditorModel Model { get; init; }

		[Reactive]
		public string CurrentLocalDirectory { get; set; } = string.Empty;
		public string CurrentDirectory => SelectedTabIndex == 0
			? CurrentLocalDirectory
			: Model.Settings.UseHttps
				? Model.Settings.ServerAddressHttps
				: Model.Settings.ServerAddressHttp;

		[Reactive]
		public FileSystemItemBase? CurrentlySelectedObject { get; set; }

		[Reactive]
		public string FilenameFilter { get; set; } = string.Empty;

		[Reactive]
		public string AuthorFilter { get; set; } = string.Empty;

		[Reactive]
		public string ModpackFilter { get; set; } = string.Empty;

		[Reactive]
		public ObjectDisplayMode DisplayMode { get; set; } = ObjectDisplayMode.All;

		[Reactive]
		List<FileSystemItemBase> LocalDirectoryItems { get; set; } = [];

		[Reactive]
		List<FileSystemItemBase> OnlineDirectoryItems { get; set; } = [];

		[Reactive]
		public ObservableCollection<FileSystemItemBase> DirectoryItems { get; set; } = [];

		[Reactive]
		public float IndexOrDownloadProgress { get; set; }

		Progress<float> Progress { get; } = new();

		public ReactiveCommand<Unit, Task>? RefreshDirectoryItems { get; }

		public ReactiveCommand<Unit, Unit>? OpenCurrentFolder { get; }

		public ObservableCollection<ObjectDisplayMode> DisplayModeItems { get; } = [.. Enum.GetValues<ObjectDisplayMode>()];

		[Reactive]
		public int SelectedTabIndex { get; set; }

		public bool IsLocal
			=> SelectedTabIndex == 0;
		public string RecreateText
			=> IsLocal ? "Recreate index" : "Download object list";

		public string DirectoryFileCount
			=> $"Objects: {DirectoryItems.Sum(CountNodes)}";

		// used for design-time view
		public FolderTreeViewModel()
		{ }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;
			Progress.ProgressChanged += (_, progress) => IndexOrDownloadProgress = progress;

			RefreshDirectoryItems = ReactiveCommand.Create(() => ReloadDirectoryAsync(false));
			OpenCurrentFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(IsLocal ? CurrentLocalDirectory : Model.Settings.DownloadFolder, Model.Logger));

			Filters = new ObservableCollection<Filter>();
			AddNewFilter = ReactiveCommand.Create(() => { Filters.Add(new Filter()); });

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

			_ = this.WhenAnyValue(o => o.DirectoryItems)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			_ = this.WhenAnyValue(o => o.DirectoryItems)
				.Skip(1)
				.Subscribe(_ => CurrentlySelectedObject = null);

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => SwitchDirectoryItemsView());

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(RecreateText)));

			_ = this.WhenAnyValue(o => o.SelectedTabIndex)
				.Skip(1)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentDirectory)));

			_ = this.WhenAnyValue(o => o.LocalDirectoryItems)
				//.Skip(1)
				.Subscribe(_ => SwitchDirectoryItemsView());

			_ = this.WhenAnyValue(o => o.OnlineDirectoryItems)
				.Skip(1)
				.Subscribe(_ => SwitchDirectoryItemsView());

			// loads the last-viewed folder
			CurrentLocalDirectory = Model.Settings.ObjDataDirectory;
		}

		public static int CountNodes(FileSystemItemBase fib)
		{
			if (fib.SubNodes == null || fib.SubNodes.Count == 0)
			{
				return 0;
			}

			var count = 0;

			foreach (var node in fib.SubNodes)
			{
				if (node is FileSystemItemObject)
				{
					count++;
				}
				else
				{
					count += CountNodes(node);
				}
			}

			return count;
		}

		void SwitchDirectoryItemsView()
			=> DirectoryItems = SelectedTabIndex == 0
				? new(LocalDirectoryItems)
				: new(OnlineDirectoryItems);

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
				//LocalDirectoryItems = [];
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
		}

		async Task LoadOnlineDirectoryAsync(bool useExistingIndex)
		{
			if (Design.IsDesignMode)
			{
				// DO NOT WEB QUERY AT DESIGN TIME
				return;
			}

			if ((!useExistingIndex || Model.ObjectIndexOnline == null) && Model.WebClient != null)
			{
				Model.ObjectIndexOnline = new ObjectIndex((await Client.GetObjectListAsync(Model.WebClient, Model.Logger))
					.Select(x => new ObjectIndexEntry(x.Id.ToString(), x.DatName, x.DatChecksum, x.ObjectType, x.ObjectSource, x.VehicleType))
					.ToList());
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
			}
		}

		static bool MatchesFilter(ObjectIndexEntry o, string filenameFilter, string authorFilter, string modpackFilter, ObjectDisplayMode displayMode)
		{
			var displayable = displayMode == ObjectDisplayMode.All || (displayMode == ObjectDisplayMode.Vanilla == (o.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG));

			var filters =
				   string.IsNullOrEmpty(filenameFilter) || o.DatName.Contains(filenameFilter, StringComparison.CurrentCultureIgnoreCase);
			//&& (string.IsNullOrEmpty(authorFilter)   || o.Author.Contains(authorFilter, StringComparison.CurrentCultureIgnoreCase))
			//&& (string.IsNullOrEmpty(modpackFilter)  || o.DatName.Contains(modpackFilter, StringComparison.CurrentCultureIgnoreCase));

			return displayable && filters;
		}

		static List<FileSystemItemBase> ConstructTreeView(IEnumerable<ObjectIndexEntry> index, string baseDirectory, string filenameFilter, string authorFilter, string modpackFilter, ObjectDisplayMode displayMode, FileLocation fileLocation)
		{
			var result = new List<FileSystemItemBase>();

			var groupedObjects = index
				.OfType<ObjectIndexEntry>() // this won't show errored files - should we??
				.Where(o => MatchesFilter(o, filenameFilter, authorFilter, modpackFilter, displayMode))
				.GroupBy(o => o.ObjectType)
				.OrderBy(fsg => fsg.Key.ToString());

			foreach (var objGroup in groupedObjects)
			{
				ObservableCollection<FileSystemItemBase> subNodes;
				if (objGroup.Key == ObjectType.Vehicle)
				{
					subNodes = [];
					foreach (var vg in objGroup
						.GroupBy(o => o.VehicleType)
						.OrderBy(vg => vg.Key.ToString()))
					{
						var vehicleSubNodes = new ObservableCollection<FileSystemItemBase>(vg
							.Select(o => new FileSystemItemObject(Path.Combine(baseDirectory, o.Filename), o.DatName, fileLocation, o.ObjectSource))
							.OrderBy(o => o.DisplayName));

						if (vg.Key == null)
						{
							// this should be impossible - object says its a vehicle but doesn't have a vehicle type
							// todo: move validation into the loading stage or cstr of IndexObjectHeader
							continue;
						}

						subNodes.Add(new FileSystemVehicleGroup(
							string.Empty,
							vg.Key.Value,
							vehicleSubNodes));
					}
				}
				else
				{
					subNodes = new ObservableCollection<FileSystemItemBase>(objGroup
						.Select(o => new FileSystemItemObject(Path.Combine(baseDirectory, o.Filename), o.DatName, fileLocation, o.ObjectSource))
						.OrderBy(o => o.DisplayName));
				}

				result.Add(new FileSystemItemGroup(
						string.Empty,
						objGroup.Key,
						subNodes));
			}

			return result;
		}
	}
}
