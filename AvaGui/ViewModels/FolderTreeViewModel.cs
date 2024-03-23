using AvaGui.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System;
using OpenLoco.ObjectEditor.AvaGui.Models;
using System.Linq;
using System.Reactive.Linq;
using OpenLoco.ObjectEditor.Objects;

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

			CurrentDirectory = "Q:\\Games\\Locomotion\\OriginalObjects";
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

			//var files = dirInfo.GetFiles();
			//var subDirs = dirInfo.GetDirectories();

			Model.LoadObjDirectory(CurrentDirectory, null, false);

			var groupedObjects = Model.ObjectCache.GroupBy(o => o.Value.DatFileInfo.S5Header.ObjectType).OrderBy(fsg => fsg.Key.ToString());
			foreach (var objGroup in groupedObjects)
			{
				ObservableCollection<FileSystemItemBase> subNodes; //(objGroup.Select(o => new FileSystemItemBase(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim())));
				if (objGroup.Key == OpenLoco.ObjectEditor.Data.ObjectType.Vehicle)
				{
					subNodes = [];
					foreach (var vg in objGroup.GroupBy(o => (o.Value.LocoObject.Object as VehicleObject)!.Type).OrderBy(vg => vg.Key.ToString()))
					{
						var vehicleSubNodes = new ObservableCollection<FileSystemItemBase>(vg.Select(o => new FileSystemItemBase(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim())));
						subNodes.Add(new FileSystemItemGroup(
							string.Empty,
							vg.Key.ToString(),
							vehicleSubNodes));
					}
				}
				else
				{
					subNodes = new ObservableCollection<FileSystemItemBase>(objGroup.Select(o => new FileSystemItemBase(o.Key, o.Value.DatFileInfo.S5Header.Name.Trim())));
				}

				var fsg = new FileSystemItemGroup(
					string.Empty,
					objGroup.Key.ToString(),
					subNodes);

				yield return fsg;
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
