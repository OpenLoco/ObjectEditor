using AvaGui.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Linq;
using System.Reactive.Linq;
using OpenLoco.ObjectEditor.Data;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; init; }

		[Reactive]
		public string CurrentDirectory { get; set; } = string.Empty;

		[Reactive]
		public FileSystemItemBase? CurrentlySelectedObject { get; set; }

		[Reactive]
		public string FilenameFilter { get; set; } = string.Empty;

		[Reactive]
		public bool DisplayVanillaOnly { get; set; }

		[Reactive]
		public ObservableCollection<FileSystemItemBase> DirectoryItems { get; private set; }

		[Reactive]
		public float IndexingProgress { get; set; }

		Progress<float> Progress { get; }

		public ReactiveCommand<Unit, Task> RecreateIndex { get; }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;
			Progress = new();
			Progress.ProgressChanged += (_, progress) => IndexingProgress = progress;

			RecreateIndex = ReactiveCommand.Create(async () => await LoadObjDirectoryAsync(CurrentDirectory, false));

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(async _ => await LoadObjDirectoryAsync(CurrentDirectory, false));
			_ = this.WhenAnyValue(o => o.DisplayVanillaOnly)
				.Subscribe(async _ => await LoadObjDirectoryAsync(CurrentDirectory, true));
			_ = this.WhenAnyValue(o => o.FilenameFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Subscribe(async _ => await LoadObjDirectoryAsync(CurrentDirectory, true));
			_ = this.WhenAnyValue(o => o.DirectoryItems)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			// loads the last-viewed folder
			CurrentDirectory = Model.Settings.ObjDataDirectory;
		}

		private async Task LoadObjDirectoryAsync(string directory, bool useExistingIndex)
		{
			DirectoryItems = new(await LoadObjDirectoryCoreAsync(directory, useExistingIndex));

			// really just for debugging - puts all dat file types in the collection, even if they don't have anything in them
			//foreach (var dat in Enum.GetValues<DatFileType>().Except(DirectoryItems.Select(x => ((FileSystemDatGroup)x).DatFileType)))
			//{
			//	DirectoryItems.Add(new FileSystemDatGroup(string.Empty, dat, new ObservableCollection<FileSystemItemBase>()));
			//}

			async Task<List<FileSystemItemBase>> LoadObjDirectoryCoreAsync(string directory, bool useExistingIndex)
			{
				var result = new List<FileSystemItemBase>();

				if (string.IsNullOrEmpty(directory))
				{
					return result;
				}

				var dirInfo = new DirectoryInfo(directory);

				if (!dirInfo.Exists)
				{
					return result;
				}

				// todo: load each file
				// check if its object, scenario, save, landscape, g1, sfx, tutorial, etc

				await Model.LoadObjDirectoryAsync(directory, Progress, useExistingIndex);

				var groupedDatObjects = Model.HeaderIndex
					.Where(o => (string.IsNullOrEmpty(FilenameFilter) || o.Value.Name.Contains(FilenameFilter, StringComparison.CurrentCultureIgnoreCase)) && (!DisplayVanillaOnly || o.Value.SourceGame == SourceGame.Vanilla))
					.GroupBy(o => o.Value.DatFileType)
					.OrderBy(x => x.Key.ToString());

				foreach (var datObjGroup in groupedDatObjects)
				{
					ObservableCollection<FileSystemItemBase> groups = [];

					var groupedObjects = datObjGroup
						.GroupBy(x => x.Value.ObjectType)
						.OrderBy(x => x.Key.ToString());

					foreach (var objGroup in groupedObjects)
					{
						ObservableCollection<FileSystemItemBase> subNodes = [];
						if (objGroup.Key == ObjectType.Vehicle)
						{
							foreach (var vg in objGroup
								.GroupBy(o => o.Value.VehicleType)
								.OrderBy(vg => vg.Key.ToString()))
							{
								var vehicleSubNodes = new ObservableCollection<FileSystemItemBase>(
									vg
									.Select(o => new FileSystemItem(o.Key, o.Value.Name.Trim(), o.Value.SourceGame))
									.OrderBy(o => o.Name));

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
							subNodes = new ObservableCollection<FileSystemItemBase>(
								objGroup
								.Select(o => new FileSystemItem(o.Key, o.Value.Name.Trim(), o.Value.SourceGame))
								.OrderBy(o => o.Name));
						}

						groups.Add(new FileSystemItemGroup(
							string.Empty,
							objGroup.Key,
							subNodes));
					}

					result.Add(new FileSystemDatGroup(
						string.Empty,
						datObjGroup.Key,
						groups));
				}

				return result;
			}
		}

		public string DirectoryFileCount
			=> $"Objects: {Model.HeaderIndex.Count}";
	}
}
