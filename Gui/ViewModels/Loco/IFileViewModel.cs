using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace Gui.ViewModels;

public interface IFileViewModel
{
	public ReactiveCommand<Unit, Unit> ReloadCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveAsCommand { get; init; }
	public ReactiveCommand<Unit, Unit> DeleteLocalFileCommand { get; init; }

	[Reactive]
	public bool SaveIsVisible { get; set; }

	[Reactive]
	public bool SaveAsIsVisible { get; set; }

	[Reactive]
	public FileSystemItem CurrentFile { get; init; }

	[Reactive]
	public bool IsLocalMode
		=> CurrentFile.FileLocation == FileLocation.Local;

	public string ReloadText { get; }
	public string SaveText { get; }
	public string SaveAsText { get; }
	public string DeleteLocalFileText { get; }

	public string ReloadIcon { get; }
	public string SaveIcon { get; }
	public string SaveAsIcon { get; }
	public string DeleteLocalFileIcon { get; }
}
