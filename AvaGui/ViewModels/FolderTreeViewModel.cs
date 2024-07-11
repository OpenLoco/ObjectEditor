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

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; init; }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryItems)));
			_ = this.WhenAnyValue(o => o.DisplayVanillaOnly)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryItems)));
			_ = this.WhenAnyValue(o => o.FilenameFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryItems)));
			_ = this.WhenAnyValue(o => o.DirectoryItems)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			// loads the last-viewed folder
			CurrentDirectory = Model.Settings.ObjDataDirectory;
		}

		[Reactive]
		public string CurrentDirectory { get; set; } = string.Empty;

		[Reactive]
		public FileSystemItemBase? CurrentlySelectedObject { get; set; } = null;

		[Reactive]
		public string FilenameFilter { get; set; } = string.Empty;

		[Reactive]
		public bool DisplayVanillaOnly { get; set; }

		public ObservableCollection<FileSystemItemBase> DirectoryItems
			=> LoadObjDirectory(CurrentDirectory);
		private ObservableCollection<FileSystemItemBase> LoadObjDirectory(string newDir)
			=> new(LoadObjDirectoryCore(newDir));

		string prevDir = string.Empty;

		private IEnumerable<FileSystemItemBase> LoadObjDirectoryCore(string newDir)
		{
			if (newDir == null || string.IsNullOrEmpty(newDir))
			{
				yield break;
			}

			var dirInfo = new DirectoryInfo(newDir);

			if (!dirInfo.Exists)
			{
				yield break;
			}

			// todo: load each file
			// check if its object, scenario, save, landscape, g1, sfx, tutorial, etc

			if (prevDir != newDir)
			{
				Model.LoadObjDirectory(newDir, null, true);
			}

			var groupedObjects = Model.HeaderIndex
				.Where(o => (string.IsNullOrEmpty(FilenameFilter) || o.Value.Name.Contains(FilenameFilter, StringComparison.CurrentCultureIgnoreCase)) && (!DisplayVanillaOnly || o.Value.SourceGame == SourceGame.Vanilla))
				.GroupBy(o => o.Value.ObjectType)
				.OrderBy(fsg => fsg.Key.ToString());

			var count = 0;
			foreach (var objGroup in groupedObjects)
			{
				ObservableCollection<FileSystemItemBase> subNodes; //(objGroup.Select(o => new FileSystemItemBase(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim())));
				if (objGroup.Key == ObjectType.Vehicle)
				{
					subNodes = [];
					var vCount = 0;
					foreach (var vg in objGroup
						.GroupBy(o => o.Value.VehicleType)
						.OrderBy(vg => vg.Key.ToString()))
					{
						var vehicleSubNodes = new ObservableCollection<FileSystemItemBase>(vg.Select(o => new FileSystemItem(o.Key, o.Value.Name.Trim(), o.Value.SourceGame)));

						if (vg.Key == null)
						{
							// this should be impossible - object says its a vehicle but doesn't have a vehicle type
							// todo: move validation into the loading stage or cstr of IndexObjectHeader
							continue;
						}

						subNodes.Add(new FileSystemVehicleGroup(
							string.Empty,
							vg.Key.Value,
							vehicleSubNodes,
							vCount++));
					}
				}
				else
				{
					subNodes = new ObservableCollection<FileSystemItemBase>(
						objGroup.Select(o => new FileSystemItem(o.Key, o.Value.Name.Trim(), o.Value.SourceGame)));
				}

				yield return new FileSystemItemGroup(
					string.Empty,
					objGroup.Key,
					subNodes,
					count++);

				prevDir = newDir;
			}
		}

		public string DirectoryFileCount
			=> $"Files in dir: {(CurrentDirectory == null ? 0 : new DirectoryInfo(CurrentDirectory).GetFiles().Length)}";
	}
}
