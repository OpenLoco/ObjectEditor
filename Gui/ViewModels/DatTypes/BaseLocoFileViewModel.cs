using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Common.Logging;
using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public abstract class BaseLocoFileViewModel : ReactiveObject, ILocoFileViewModel
{
	protected BaseLocoFileViewModel(FileSystemItem currentFile, ObjectEditorModel model)
	{
		CurrentFile = currentFile;
		Model = model;

		ReloadCommand = ReactiveCommand.Create(Load);
		SaveCommand = ReactiveCommand.CreateFromTask(SaveWrapper);
		SaveAsCommand = ReactiveCommand.Create(SaveAs);
		DeleteLocalFileCommand = ReactiveCommand.CreateFromTask(DeleteWrapper);
	}

	[Reactive]
	public FileSystemItem CurrentFile { get; init; }
	public ObjectEditorModel Model { get; init; }

	protected ILogger logger => Model.Logger;

	public ReactiveCommand<Unit, Unit> ReloadCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveAsCommand { get; init; }
	public ReactiveCommand<Unit, Unit> DeleteLocalFileCommand { get; init; }

	public abstract void Load();
	public abstract void Save();
	public abstract void SaveAs();
	public virtual void Delete() { }

	async Task SaveWrapper()
	{
		// note - this is the DAT file source, not the true source...
		if (CurrentFile.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Confirm Save", $"{CurrentFile.FileName} is a vanilla Locomotion file - are you sure you want to overwrite it?", ButtonEnum.YesNo);
			var result = await box.ShowAsync();

			if (result != ButtonResult.Yes)
			{
				return;
			}
		}

		Save();
	}

	async Task DeleteWrapper()
	{
		var box = MessageBoxManager.GetMessageBoxStandard("Confirm Delete", $"Are you sure you would like to delete {CurrentFile.FileName}?", ButtonEnum.YesNo);
		var result = await box.ShowAsync();

		if (result == ButtonResult.Yes)
		{
			Delete();
		}
	}

	public string ReloadText => CurrentFile.FileLocation == FileLocation.Local ? "Reload" : "Redownload";
	public string SaveText => CurrentFile.FileLocation == FileLocation.Local ? "Save" : "Download";
	public string SaveAsText => $"{SaveText} As";
	public string DeleteLocalFileText => "Delete File";

	public string ReloadIcon => CurrentFile.FileLocation == FileLocation.Local ? "DatabaseRefresh" : "FileSync";
	public string SaveIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSave" : "FileDownload";
	public string SaveAsIcon => CurrentFile.FileLocation == FileLocation.Local ? "ContentSavePlus" : "FileDownloadOutline";
	public string DeleteLocalFileIcon => "DeleteForever";
}
