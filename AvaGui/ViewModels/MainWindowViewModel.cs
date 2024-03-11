using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Reactive;

namespace AvaGui.ViewModels
{
	public record FileSystemItem(string Name, bool IsFile);

	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(/*ObjectEditorViewModel objectEditorViewModel*/)
		{
			/*ObjectEditorViewModel = objectEditorViewModel;*/

			LoadFolderCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				// Logic to load directory structure will go here
				try
				{
					var dirInfo = new DirectoryInfo(FolderPath);
					var files = dirInfo.GetFiles();
					var subDirs = dirInfo.GetDirectories();

					// We'll likely need a data model to represent our hierarchy
					HierarchyItems = new ObservableCollection<FileSystemItem>();

					foreach (var file in files)
					{
						HierarchyItems.Add(new FileSystemItem(file.Name, true));
					}

					foreach (var dir in subDirs)
					{
						HierarchyItems.Add(new FileSystemItem(dir.Name, false));
					}
				}
				catch (Exception ex)
				{
					// Handle errors (e.g., invalid path) 
				}
			});
		}

		private ObservableCollection<FileSystemItem> _hierarchyItems;
		public ObservableCollection<FileSystemItem> HierarchyItems
		{
			get => _hierarchyItems;
			set => this.RaiseAndSetIfChanged(ref _hierarchyItems, value);
		}

		private string _folderPath;
		public string FolderPath
		{
			get => _folderPath;
			set => this.RaiseAndSetIfChanged(ref _folderPath, value);
		}

		public ReactiveCommand<Unit, Unit> LoadFolderCommand { get; }

		//public ObjectEditorViewModel ObjectEditorViewModel { get; } = new ObjectEditorViewModel();
	}
}
