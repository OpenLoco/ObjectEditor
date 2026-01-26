using Avalonia.Controls;
using Common.Logging;
using Dat.Data;
using Definitions.ObjectModels.Types;
using DynamicData;
using Gui.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public enum SaveType { JSON, DAT }

// todo: add filename
public record SaveParameters(SaveType SaveType, SawyerEncoding? SawyerEncoding);

public abstract class BaseViewModel<T> : ReactiveObject, IViewModel where T : class
{
	protected BaseViewModel(T? model = default)
	{
		Model = model;

		_ = _subViewModels.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out _viewModels)
			.Subscribe();
	}

	[Reactive]
	public T? Model { get; protected set; }

	private readonly SourceList<IViewModel> _subViewModels = new();
	private readonly ReadOnlyObservableCollection<IViewModel> _viewModels;
	public ReadOnlyObservableCollection<IViewModel> ViewModels
		=> _viewModels;

	protected void AddViewModel(IViewModel? vm)
	{
		if (vm != null)
		{
			_subViewModels.Add(vm);
		}
	}

	protected void ClearViewModels()
		=> _subViewModels.Clear();

	public virtual string ViewModelDisplayName
		=> typeof(T).Name;
}

public abstract class BaseViewModelWithEditorContext<T> : BaseViewModel<T> where T : class
{
	protected BaseViewModelWithEditorContext(ObjectEditorContext editorContext, T? model = default)
		: base(model)
		=> EditorContext = editorContext;

	public ObjectEditorContext EditorContext { get; init; }

	protected ILogger logger
		=> EditorContext.Logger;
}

public abstract class BaseFileViewModel<T> : BaseViewModelWithEditorContext<T>, IFileViewModel where T : class
{
	protected BaseFileViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext, T? model = default)
		: base(editorContext, model)
	{
		CurrentFile = currentFile;
		ReloadCommand = ReactiveCommand.Create(Load);
		SaveCommand = ReactiveCommand.CreateFromTask(SaveWrapper);
		SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAsWrapper);
		DeleteLocalFileCommand = ReactiveCommand.CreateFromTask(DeleteWrapper);
	}

	[Reactive]
	public FileSystemItem CurrentFile { get; init; }

	public ReactiveCommand<Unit, Unit> ReloadCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveCommand { get; init; }
	public ReactiveCommand<Unit, Unit> SaveAsCommand { get; init; }
	public ReactiveCommand<Unit, Unit> DeleteLocalFileCommand { get; init; }

	public abstract void Load();
	public abstract void Save();
	public abstract string? SaveAs(SaveParameters saveParameters);
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

		var filename = SaveAs(new SaveParameters(type, encoding));

		// Open the newly-saved document as a new tab in the tabviewpagemodel
		if (filename != null)
		{
			// todo:
		}
	}

	async Task SaveWrapper()
	{
		// note - this is the DAT file source, not the true source...
		if (CurrentFile.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			var msbParams = GetDefaultParams();
			msbParams.ContentTitle = "Confirm Save";
			msbParams.ContentMessage = $"{CurrentFile.FileName} is a vanilla Locomotion file - are you sure you want to overwrite it?";
			msbParams.ButtonDefinitions = ButtonEnum.YesNo;
			msbParams.Icon = Icon.Database;

			var box = MessageBoxManager.GetMessageBoxStandard(msbParams);

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
		var msbParams = GetDefaultParams();
		msbParams.ContentTitle = "Confirm Delete";
		msbParams.ContentMessage = $"Are you sure you would like to delete {CurrentFile.FileName}?";
		msbParams.ButtonDefinitions = ButtonEnum.YesNo;
		msbParams.Icon = Icon.Stop;

		var box = MessageBoxManager.GetMessageBoxStandard(msbParams);

		var result = await box.ShowAsync();

		if (result == ButtonResult.Yes)
		{
			Delete();
		}
	}

	MessageBoxStandardParams GetDefaultParams()
		=> new()
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Topmost = true,
			SizeToContent = SizeToContent.WidthAndHeight,
			ShowInCenter = true,
			MinHeight = 170,
		};

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
