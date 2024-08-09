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

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; init; }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;

			RecreateIndex = ReactiveCommand.Create(() => LoadObjDirectory(CurrentDirectory, false));

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(_ => LoadObjDirectory(CurrentDirectory, true));
			_ = this.WhenAnyValue(o => o.DisplayVanillaOnly)
				.Subscribe(_ => LoadObjDirectory(CurrentDirectory, true));
			_ = this.WhenAnyValue(o => o.FilenameFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Subscribe(_ => LoadObjDirectory(CurrentDirectory, true));
			_ = this.WhenAnyValue(o => o.DirectoryItems)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			// loads the last-viewed folder
			CurrentDirectory = Model.Settings.ObjDataDirectory;
		}
		public ReactiveCommand<Unit, Unit> RecreateIndex { get; }

		[Reactive]
		public string CurrentDirectory { get; set; } = string.Empty;

		[Reactive]
		public FileSystemItemBase? CurrentlySelectedObject { get; set; }

		[Reactive]
		public string FilenameFilter { get; set; } = string.Empty;

		[Reactive]
		public bool DisplayVanillaOnly { get; set; }

		[Reactive]
		public ObservableCollection<FileSystemItemBase> DirectoryItems { get; set; }

		private void LoadObjDirectory(string directory, bool useExistingIndex)
		{
			DirectoryItems = new(LoadObjDirectoryCore(directory, useExistingIndex));

			IEnumerable<FileSystemItemBase> LoadObjDirectoryCore(string directory, bool useExistingIndex)
			{
				if (string.IsNullOrEmpty(directory))
				{
					yield break;
				}

				var dirInfo = new DirectoryInfo(directory);

				if (!dirInfo.Exists)
				{
					yield break;
				}

				// todo: load each file
				// check if its object, scenario, save, landscape, g1, sfx, tutorial, etc

				Model.LoadObjDirectory(directory, null, useExistingIndex);

				var groupedObjects = Model.HeaderIndex
					.Where(o => (string.IsNullOrEmpty(FilenameFilter) || o.Value.Name.Contains(FilenameFilter, StringComparison.CurrentCultureIgnoreCase)) && (!DisplayVanillaOnly || o.Value.SourceGame == SourceGame.Vanilla))
					.GroupBy(o => o.Value.ObjectType)
					.OrderBy(fsg => fsg.Key.ToString());

				foreach (var objGroup in groupedObjects)
				{
					ObservableCollection<FileSystemItemBase> subNodes; //(objGroup.Select(o => new FileSystemItemBase(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim())));
					if (objGroup.Key == ObjectType.Vehicle)
					{
						subNodes = [];
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
								vehicleSubNodes));
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
						subNodes);
				}
			}
		}

		public string DirectoryFileCount
			=> $"Objects: {Model.HeaderIndex.Count}";
	}
}
