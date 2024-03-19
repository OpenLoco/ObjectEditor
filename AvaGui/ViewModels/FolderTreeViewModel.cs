using AvaGui.Models;
using Avalonia.Reactive;
using OpenLoco.ObjectEditor.Settings;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;

namespace AvaGui.ViewModels
{
	public class FolderTreeViewModel : ReactiveObject
	{
		EditorSettings Settings;

		public FolderTreeViewModel(EditorSettings settings)
		{
			Settings = settings;

			_ = this.WhenAnyValue(o => o.CurrentDirectory)
				.Subscribe(new Avalonia.Reactive.AnonymousObserver<string>(
					onNext: value => DirectoryItems = LoadDirectory(value),
					onError: error => { /* Handle error */ },
					onCompleted: () => { /* Perform actions on completion (optional) */ }));

			//LoadDirectory();

			//LoadFolderCommand = ReactiveCommand.CreateFromTask(async () =>
			//{
			//	// Logic to load directory structure will go here
			//	try
			//	{
			//		LoadDirectory();
			//	}
			//	catch (Exception ex)
			//	{
			//		// Handle errors (e.g., invalid path) 
			//	}
			//});
		}

		public void LoadButton()
		{
			//CurrentDirectory = Settings.ObjDataDirectory;
		}

		private static ObservableCollection<FileSystemItem> LoadDirectory(string newDir)
			=> new ObservableCollection<FileSystemItem>(_LoadDirectory(newDir));

		private static IEnumerable<FileSystemItem> _LoadDirectory(string newDir)
		{
			if (newDir == null)
			{
				yield break;
			}

			var dirInfo = new DirectoryInfo(newDir);
			var files = dirInfo.GetFiles();
			var subDirs = dirInfo.GetDirectories();

			foreach (var file in files)
			{
				yield return new FileSystemItem(file.Name, true);
			}

			foreach (var dir in subDirs)
			{
				yield return new FileSystemItem(dir.Name, false);
			}
		}

		string _currentDirectory;
		public string CurrentDirectory
		{
			get => _currentDirectory;
			set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
		}

		//public ReactiveCommand<Unit, Unit> LoadFolderCommand { get; }

		private ObservableCollection<FileSystemItem> _directoryItems;
		public ObservableCollection<FileSystemItem> DirectoryItems
		{
			get => _directoryItems;
			set => this.RaiseAndSetIfChanged(ref _directoryItems, value);
		}
	}
}
