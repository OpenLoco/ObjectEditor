using Avalonia.Controls;
using AvaGui.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Linq;
using System.Reactive.Linq;
using OpenLoco.ObjectEditor.Objects;
using OpenLoco.ObjectEditor.Data;

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryItems)));
			_ = this.WhenAnyValue(o => o.DisplayVanillaOnly)
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryItems)));
			_ = this.WhenAnyValue(o => o.FilenameFilter)
				.Throttle(TimeSpan.FromMilliseconds(500))
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryItems)));

			CurrentDirectory = "Q:\\Games\\Locomotion\\OriginalObjects";
		}

		string _filenameFilter;
		public string FilenameFilter
		{
			get => _filenameFilter;
			set => this.RaiseAndSetIfChanged(ref _filenameFilter, value);
		}

		bool _displayVanillaOnly;
		public bool DisplayVanillaOnly
		{
			get => _displayVanillaOnly;
			set => this.RaiseAndSetIfChanged(ref _displayVanillaOnly, value);
		}

		private ObservableCollection<FileSystemItemBase> LoadDirectory(string newDir)
			=> new(_LoadDirectory(newDir));

		private IEnumerable<FileSystemItemBase> _LoadDirectory(string newDir)
		{
			if (newDir == null)
			{
				yield break;
			}

			var dirInfo = new DirectoryInfo(newDir);

			if (!dirInfo.Exists)
			{
				yield break;
			}

			Model.LoadObjDirectory(CurrentDirectory, null, false);

			var groupedObjects = Model.ObjectCache
				.Where(o => string.IsNullOrEmpty(FilenameFilter) || o.Value.DatFileInfo.S5Header.Name.Contains(FilenameFilter, StringComparison.CurrentCultureIgnoreCase))
				.GroupBy(o => o.Value.DatFileInfo.S5Header.ObjectType)
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
						.GroupBy(o => (o.Value.LocoObject.Object as VehicleObject)!.Type)
						.OrderBy(vg => vg.Key.ToString()))
					{
						var vehicleSubNodes = new ObservableCollection<FileSystemItemBase>(vg.Select(o => new FileSystemItem(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim(), o.Value.DatFileInfo.S5Header.SourceGame)));
						subNodes.Add(new FileSystemVehicleGroup(
							string.Empty,
							vg.Key,
							vehicleSubNodes,
							vCount++));
					}
				}
				else
				{
					subNodes = new ObservableCollection<FileSystemItemBase>(objGroup.Select(o => new FileSystemItem(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim(), o.Value.DatFileInfo.S5Header.SourceGame)));
				}

				if (DisplayVanillaOnly)
				{
					subNodes = new ObservableCollection<FileSystemItemBase>(subNodes.Where(o => o is FileSystemItem item && item.SourceGame == SourceGame.Vanilla));
				}

				yield return new FileSystemItemGroup(
					string.Empty,
					objGroup.Key,
					subNodes,
					count++);
			}
		}

		string _currentDirectory;
		public string CurrentDirectory
		{
			get => _currentDirectory;
			set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
		}

		public ObservableCollection<FileSystemItemBase> DirectoryItems
			=> LoadDirectory(CurrentDirectory);

		public string DirectoryFileCount
			=> $"Files in dir: {new DirectoryInfo(CurrentDirectory).GetFiles().Length}";

		public FileSystemItemBase _currentlySelectedObject;
		public FileSystemItemBase CurrentlySelectedObject
		{
			get => _currentlySelectedObject;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedObject, value);
		}
	}
}
