using OpenLoco.Common.Logging;
using OpenLoco.Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace OpenLoco.Gui.ViewModels
{
	public record FileViewButton(ReactiveCommand<Unit, Unit> Command, string Text, string Icon);

	public interface ILocoFileViewModelControl
	{
		[Reactive]
		public FileSystemItem CurrentFile { get; init; }

		ListObservable<FileViewButton> Buttons { get; }
	}

	public interface ILocoFileViewModel
	{
		public ReactiveCommand<Unit, Unit> ReloadCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsCommand { get; init; }
		public ReactiveCommand<Unit, Unit> DeleteLocalFileCommand { get; init; }

		[Reactive]
		public FileSystemItem CurrentFile { get; init; }

		[Reactive]
		public bool IsLocalMode => CurrentFile.FileLocation == FileLocation.Local;

		public string ReloadText { get; }
		public string SaveText { get; }
		public string SaveAsText { get; }
		public string DeleteLocalFileText { get; }

		public string ReloadIcon { get; }
		public string SaveIcon { get; }
		public string SaveAsIcon { get; }
		public string DeleteLocalFileIcon { get; }
	}

	public abstract class BaseLocoFileViewModel : ReactiveObject, ILocoFileViewModel
	{
		protected BaseLocoFileViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			ReloadCommand = ReactiveCommand.Create(Load);
			SaveCommand = ReactiveCommand.Create(Save);
			SaveAsCommand = ReactiveCommand.Create(SaveAs);
			DeleteLocalFileCommand = ReactiveCommand.Create(Delete);
		}

		[Reactive]
		public FileSystemItem CurrentFile { get; init; }
		public ObjectEditorModel Model { get; init; }

		protected ILogger Logger => Model.Logger;

		public ReactiveCommand<Unit, Unit> ReloadCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsCommand { get; init; }
		public ReactiveCommand<Unit, Unit> DeleteLocalFileCommand { get; init; }

		public abstract void Load();

		public abstract void Save();

		public abstract void SaveAs();
		public virtual void Delete() { }

		public string ReloadText => CurrentFile.FileLocation == FileLocation.Local ? "Reload" : "Redownload";
		public string SaveText => CurrentFile.FileLocation == FileLocation.Local ? "Save" : "Download";
		public string SaveAsText => $"{SaveText} As";
		public string DeleteLocalFileText => "Delete File";

		public string ReloadIcon => CurrentFile.FileLocation == FileLocation.Local ? "DatabaseRefresh" : "FileSync";
		public string SaveIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSave" : "FileDownload";
		public string SaveAsIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSavePlus" : "FileDownloadOutline";
		public string DeleteLocalFileIcon => "DeleteForever";
	}
}
