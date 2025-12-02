using Avalonia.Controls;
using Common.Logging;
using Dat.Data;
using Definitions.ObjectModels.Types;
using Gui.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public enum SaveType { JSON, DAT }

// todo: add filename
public record SaveParameters(SaveType SaveType, SawyerEncoding? SawyerEncoding);

public abstract class BaseLocoFileViewModel : ReactiveObject, ILocoFileViewModel
{
	protected BaseLocoFileViewModel(FileSystemItem currentFile, ObjectEditorModel model)
	{
		CurrentFile = currentFile;
		Model = model;

		ReloadCommand = ReactiveCommand.Create(Load);
		SaveCommand = ReactiveCommand.CreateFromTask(SaveWrapper);
		SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAsWrapper);
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
	public abstract void SaveAs(SaveParameters saveParameters);
	public virtual void Delete() { }

	async Task SaveAsWrapper()
	{
		// show save wizard here, asking the user to select a save type (DAT or JSON) and if its DAT, letting them select an option for the DAT encoding

		var buttons = new HashSet<string>()
		{
			"JSON (Experimental)",
			$"DAT ({SawyerEncoding.Uncompressed})",
			$"DAT ({SawyerEncoding.RunLengthSingle})",
			$"DAT ({SawyerEncoding.RunLengthMulti})",
			$"DAT ({SawyerEncoding.Rotate})",
		};

		var box = MessageBoxManager.GetMessageBoxCustom
			(new MessageBoxCustomParams
			{
				ButtonDefinitions = buttons.Select(x => new ButtonDefinition() { Name = x }),
				ContentTitle = "Save As",
				ContentMessage = "Save as DAT object or JSON file?",
				Icon = Icon.Question,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				CanResize = false,
				//MaxWidth = 500,
				MaxHeight = 800,
				SizeToContent = SizeToContent.WidthAndHeight,
				ShowInCenter = true,
				Topmost = false,
			});

		var result = await box.ShowAsync();
		if (!buttons.Contains(result))
		{
			return;
		}

		var type = result == "JSON (Experimental)" ? SaveType.JSON : SaveType.DAT;
		SawyerEncoding? encoding = type == SaveType.DAT
			? result switch
			{
				"DAT (Uncompressed)" => SawyerEncoding.Uncompressed,
				"DAT (RunLengthSingle)" => SawyerEncoding.RunLengthSingle,
				"DAT (RunLengthMulti)" => SawyerEncoding.RunLengthMulti,
				"DAT (Rotate)" => SawyerEncoding.Rotate,
				_ => null
			}
			: null;

		SaveAs(new SaveParameters(type, encoding));
	}

	async Task SaveWrapper()
	{
		// note - this is the DAT file source, not the true source...
		if (CurrentFile.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			var box = MessageBoxManager.GetMessageBoxStandard("Confirm Save", $"{CurrentFile.FileName} is a vanilla Locomotion file - are you sure you want to overwrite it?", ButtonEnum.YesNo);
			var result = await box.ShowAsync();

			if (result == ButtonResult.Yes)
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

	public bool SaveIsVisible { get; set; } = true;
	public bool SaveAsIsVisible { get; set; } = true;
}
