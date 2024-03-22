using AvaGui.Models;
using OpenLoco.ObjectEditor.Settings;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System;
using OpenLoco.ObjectEditor.AvaGui.Models;
using Avalonia.Controls;

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; }

		public FolderTreeViewModel(ObjectEditorModel model)
		{
			Model = model;

			this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryFileCount)));

			this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(o => this.RaisePropertyChanged(nameof(DirectoryItems)));

			CurrentDirectory = "Q:\\Games\\Locomotion\\OriginalObjects";
		}

		public void LoadButton()
		{
			//CurrentDirectory = Settings.ObjDataDirectory;
		}

		private ObservableCollection<FileSystemItem> LoadDirectory(string newDir)
			=> new ObservableCollection<FileSystemItem>(_LoadDirectory(newDir));

		private IEnumerable<FileSystemItem> _LoadDirectory(string newDir)
		{
			// ToDo: get Model to do this, it will give us nice infos
			if (newDir == null)
			{
				yield break;
			}

			var dirInfo = new DirectoryInfo(newDir);

			if (!dirInfo.Exists)
			{
				yield break;
			}

			var files = dirInfo.GetFiles();
			var subDirs = dirInfo.GetDirectories();

			Model.LoadObjDirectory(CurrentDirectory, null, false);

			foreach (var file in Model.ObjectCache)
			{
				yield return new FileSystemItem(
					file.Value.DatFileInfo.S5Header.Name,
					file.Value.DatFileInfo.S5Header.ObjectType.ToString());
			}
		}

		string _currentDirectory;
		public string CurrentDirectory
		{
			get => _currentDirectory;
			set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
		}

		//private ObservableCollection<FileSystemItem> _directoryItems;
		public ObservableCollection<FileSystemItem> DirectoryItems
		{
			get
			{
				//_directoryItems = LoadDirectory(CurrentDirectory);
				return LoadDirectory(CurrentDirectory);
			}
			//set => this.RaiseAndSetIfChanged(ref _directoryItems, value);
		}

		public string DirectoryFileCount
			=> $"Files in dir: {DirectoryItems.Count}";

		public FileSystemItem _currentlySelectedObject;
		public FileSystemItem CurrentlySelectedObject
		{
			get => _currentlySelectedObject;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedObject, value);
		}
	}
}
