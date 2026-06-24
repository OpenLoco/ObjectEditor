using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions.DTO;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Types;
using Gui.Models;
using Gui.Models.Audio;
using Gui.ViewModels.Graphics;
using Gui.ViewModels.Loco.Objects.Building;
using Gui.Views;
using Microsoft.Extensions.Logging;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ObjectEditorViewModel : BaseFileViewModel<LocoUIObjectModel>
{
	public ReactiveCommand<Unit, Unit> ExportUncompressedCommand { get; }
	public ReactiveCommand<GameObjDataFolder, Unit> CopyToGameObjDataCommand { get; }
	public ReactiveCommand<Unit, bool> ValidateObjectCommand { get; }
	public ReactiveCommand<Unit, bool> ValidateForOGCommand { get; }

	[Reactive]
	public GameObjDataFolder LastGameObjDataFolder { get; set; } = GameObjDataFolder.LocomotionSteam;

	public Interaction<ObjectSelectionWindowViewModel, ObjectSelectionWindowViewModel?> SelectObjectShowDialog { get; }

	public ObjectEditorViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
	{
		Load();

		ExportUncompressedCommand = ReactiveCommand.CreateFromTask(SaveAsUncompressedDatAsync);
		CopyToGameObjDataCommand = ReactiveCommand.Create((GameObjDataFolder targetFolder) => CopyToGameObjDataFolder(targetFolder, currentFile, editorContext));
		ValidateObjectCommand = ReactiveCommand.Create(() => ValidateObject(showPopupOnSuccess: true));
		ValidateForOGCommand = ReactiveCommand.Create(() => ValidateForOG(showPopupOnSuccess: true));

		SelectObjectShowDialog = new();
		_ = SelectObjectShowDialog.RegisterHandler(DoShowDialogAsync<ObjectSelectionWindowViewModel, ObjectSelectionWindow>);
	}

	private void CopyToGameObjDataFolder(GameObjDataFolder targetFolder, FileSystemItem currentFile, ObjectEditorContext editorContext)
	{
		var folder = editorContext.Settings.GetGameObjDataFolder(targetFolder);
		if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
		{
			Logger.LogError("The specified [{TargetFolder}] ObjData directory is invalid: \"{Folder}\"", targetFolder, folder);
			return;
		}

		LastGameObjDataFolder = targetFolder;

		try
		{
			var srcFile = currentFile.FileName ?? string.Empty;
			File.Copy(srcFile, Path.Combine(folder, Path.GetFileName(srcFile) ?? string.Empty));
			Logger.LogInformation("Copied {Path} to [[{TargetFolder}]] {Folder}", Path.GetFileName(srcFile), targetFolder, folder);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Could not copy {FileName} to {Folder}:", currentFile.FileName, folder);
		}
	}

	bool ValidateObject(bool showPopupOnSuccess)
	{
		var obj = Model?.LocoObject?.Object;
		var validationErrors = obj?.Validate(new ValidationContext(obj)).ToList() ?? [];
		ShowValidationMessageBox(validationErrors, showPopupOnSuccess);
		return validationErrors != null && validationErrors.Count == 0;
	}

	static void ShowValidationMessageBox<T>(IEnumerable<T> validationErrors, bool showPopupOnSuccess)
	{
		// Show message box
		IMsBox<ButtonResult> box;
		if (validationErrors.Any())
		{
			var errorMsg = string.Join(Environment.NewLine, validationErrors);
			box = MessageBoxManager.GetMessageBoxStandard(
				"Validation failed",
				errorMsg,
				ButtonEnum.Ok,
				Icon.Error,
				windowStartupLocation: WindowStartupLocation.CenterOwner);

			_ = box.ShowAsync();
		}
		else
		{
			if (showPopupOnSuccess)
			{
				box = MessageBoxManager.GetMessageBoxStandard(
					"Validation succeeded",
					"✔ No issues found. Object is valid.",
					ButtonEnum.Ok,
					Icon.Success,
					windowStartupLocation: WindowStartupLocation.CenterOwner);

				_ = box.ShowAsync();
			}
		}
	}

	bool ValidateForOG(bool showPopupOnSuccess)
	{
		var validationErrors = new List<string>();

		if (Model?.DatInfo is null)
		{
			validationErrors.Add("Object DAT info is null");
			return false;
		}

		var filename = CurrentFile.FileName;
		if (string.IsNullOrEmpty(filename))
		{
			validationErrors.Add("Filename is null or empty");
			return false;
		}

		// split the CurrentFile path on "opengraphics" folder
		_ = Path.GetRelativePath(EditorContext.Settings.ObjDataDirectory, filename);
		var parentDirName = Path.GetFileName(Path.GetDirectoryName(CurrentFile.FileName));

		if (string.IsNullOrEmpty(parentDirName))
		{
			validationErrors.Add("Parent directory name is null or empty");
			return false;
		}

		if (OriginalObjectFiles.Names.TryGetValue(parentDirName, out var fileInfo))
		{
			// DAT name is the expected dat name
			if (Model.DatInfo.S5Header.Name != fileInfo.OpenGraphicsName)
			{
				validationErrors.Add($"✖ Internal DAT header name is not correct. Actual=\"{Model.DatInfo.S5Header.Name}\" Expected=\"{fileInfo.OpenGraphicsName}\" ");
			}
		}
		else
		{
			validationErrors.Add($"✖ Unable to find file info for the vanilla file. Name=\"{parentDirName}\".");
		}

		var expectedFilename = $"OG_{parentDirName}.dat";
		var actualFilename = Path.GetFileName(CurrentFile.FileName);
		if (expectedFilename != actualFilename)
		{
			validationErrors.Add($"✖ Filename not correct. Actual=\"{actualFilename}\" Expected=\"{expectedFilename}\" ");
		}

		// DAT name is NOT prefixed by OG_
		if (Model.DatInfo.S5Header.Name.Contains('_'))
		{
			validationErrors.Add("✖ Internal header name should not contain an underscore");
		}

		// DAT name is prefixed by OG
		if (!Model.DatInfo.S5Header.Name.StartsWith("OG"))
		{
			validationErrors.Add("✖ Internal header name is not prefixed with OG");
		}

		// OpenGraphics object source set
		if (Model.DatInfo.S5Header.ObjectSource != DatObjectSource.OpenLoco)
		{
			validationErrors.Add("✖ Object source is not set to OpenLoco");
		}

		// if Vehicle - use RunLengthSingle
		if (Model.DatInfo.S5Header.ObjectType == DatObjectType.Vehicle && Model.DatInfo.ObjectHeader.Encoding != SawyerEncoding.RunLengthSingle)
		{
			validationErrors.Add("✖ Object is a Vehicle but doesn't have encoding set to RunLengthSingle");
		}

		ShowValidationMessageBox(validationErrors, showPopupOnSuccess);
		return validationErrors != null && validationErrors.Count == 0;
	}

	static async Task DoShowDialogAsync<TViewModel, TWindow>(IInteractionContext<TViewModel, TViewModel?> interaction) where TWindow : Window, new()
	{
		var dialog = new TWindow
		{
			DataContext = interaction.Input
		};

		if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
		{
			var parentWindow = app.MainWindow;
			if (parentWindow != null)
			{
				var result = await dialog.ShowDialog<TViewModel?>(parentWindow);
				interaction.SetOutput(result);
			}
		}
	}

	public IViewModel? GetViewModelFromStruct(LocoObject locoObject)
	{
		var locoStruct = locoObject.Object;
		var asm = Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.SingleOrDefault(type
				=> type.IsClass
				&& !type.IsAbstract
				&& type.BaseType?.IsGenericType == true
				&& type.BaseType.GetGenericTypeDefinition() == typeof(BaseViewModel<>)
				&& type.BaseType.GenericTypeArguments.Single() == locoStruct.GetType());

		if (asm == null)
		{
			return null;
		}

		// Try to create with (locoStruct, editorContext) for ViewModels that support context-aware features
		try
		{
			return (IViewModel?)Activator.CreateInstance(asm, locoStruct, EditorContext);
		}
		catch (MissingMethodException)
		{
			// Fall back to single-argument constructor for ViewModels that don't need the context
			return (IViewModel?)Activator.CreateInstance(asm, locoStruct);
		}
	}

	public override void Load()
	{
		// this stops any currently-playing sounds
		foreach (var vm in ViewModelGroups.SelectMany(x => x.ViewModels))
		{
			if (vm is AudioViewModel avm)
			{
				avm.Dispose();
			}
		}

		ResetViewModelGroups("Object");

		Logger.LogInformation("Loading {DisplayName} from {FileName}", CurrentFile.DisplayName, CurrentFile.FileName);

		if (EditorContext.TryLoadObject(CurrentFile, out var newObj))
		{
			Model = newObj!;

			var objectGroup = DefaultViewModelGroup;

			if (Model?.LocoObject != null)
			{
				AddViewModelToGroup(GetViewModelFromStruct(Model.LocoObject), objectGroup);
				AddViewModelToGroup(new StringTableViewModel(Model.LocoObject.StringTable), objectGroup);

				var mediaGroup = AddViewModelGroup("Media");

				if (Model.LocoObject.Object is SoundObject soundObject)
				{
					AddViewModelToGroup(new AudioViewModel(Logger, Model.Metadata?.InternalName ?? Model.DatInfo?.S5Header.Name ?? "unk sound name", soundObject.SoundObjectData.PcmHeader, soundObject.PcmData), mediaGroup);
				}
				else
				{
					_ = Model.LocoObject.ImageTable?.PaletteMap = EditorContext.PaletteMap;

					if (Model.LocoObject.ImageTable != null)
					{
						var configFilePath = Path.Combine(EditorContext.Settings.ConfigFolder, ObjectEditorContext.ImageTableGroupsFileName);
						AddViewModelToGroup(new ImageTableViewModel(Model.LocoObject.ImageTable, EditorContext.Logger, Model.LocoObject.ObjectType, Model.LocoObject.Object, configFilePath), mediaGroup);

						var bc = Model.LocoObject.ObjectType == ObjectType.Building ? (Model.LocoObject.Object as IHasBuildingComponents)?.BuildingComponents : null;
						if (bc != null)
						{
							AddViewModelToGroup(new BuildingComponentsViewModel(bc, Model.LocoObject.ImageTable), mediaGroup);
						}
					}
				}

				if (!mediaGroup.ViewModels.Any())
				{
					_ = RemoveViewModelGroup(mediaGroup);
				}
			}

			if (Model?.DatInfo != null)
			{
				var objectHeaderViewModel = new ObjectModelHeaderViewModel(Model.DatInfo.S5Header.Convert());

				// an object saved as 'Vanilla' but isn't truly Vanilla will have its source auto-set to Custom
				// but in this case, we do want to show the user this, so we'll default to Steam
				if (Model.DatInfo.S5Header.ObjectSource == DatObjectSource.Vanilla && objectHeaderViewModel.ObjectSource == ObjectSource.Custom)
				{
					objectHeaderViewModel.ObjectSource = ObjectSource.LocomotionSteam;
				}

				AddViewModelToGroup(
					objectHeaderViewModel,
					objectGroup);

				AddViewModelToGroup(new ObjectDatHeaderViewModel(
					Model.DatInfo.ObjectHeader.Encoding,
					Model.DatInfo.ObjectHeader.DataLength),
					objectGroup);
			}

			if (Model?.Metadata != null)
			{
				AddViewModelToGroup(new ObjectMetadataViewModel(Model.Metadata, EditorContext), objectGroup);
			}
		}
	}

	public override void Delete()
	{
		if (CurrentFile.FileLocation != FileLocation.Local)
		{
			Logger.LogError("Cannot delete non-local files");
			return;
		}

		// delete file
		if (File.Exists(CurrentFile.FileName))
		{
			Logger.LogInformation("Deleting file \"{FileName}\"", CurrentFile.FileName);
			File.Delete(CurrentFile.FileName);
		}
		else
		{
			Logger.LogInformation("File already deleted \"{FileName}\"", CurrentFile.FileName);
		}

		// note: it is not really possible to delete the entry from the index since if the user
		// has changed objdata folders but still has this item tab open, then there is no way
		// to delete. user can reindex to fix, or rely on automatic reindex at startup
	}

	public override void Save()
	{
		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? CurrentFile.FileName
			: Path.Combine(EditorContext.Settings.DownloadFolder, Path.ChangeExtension($"{CurrentFile.DisplayName}-{CurrentFile.Id}", ".dat"));

		if (string.IsNullOrEmpty(savePath))
		{
			Logger.LogError("Cannot save: savePath was null. EditorContext.Settings.DownloadsFolder=\"{DownloadsFolder}\" CurrentFile.Location=\"{FileLocation}\" CurrentFile.DisplayName=\"{DisplayName}\" CurrentFile.Id=\"{Id}\"", EditorContext.Settings.DownloadFolder, CurrentFile.FileLocation, CurrentFile.DisplayName, CurrentFile.Id);
			return;
		}

		SaveCore(savePath, new SaveParameters(SaveType.DAT, GetViewModel<ObjectDatHeaderViewModel>()?.Encoding ?? SawyerEncoding.Uncompressed));

		// Upload metadata to server when in online mode
		if (CurrentFile.FileLocation == FileLocation.Online && CurrentFile.Id.HasValue)
		{
			_ = UploadMetadataAsync(CurrentFile.Id.Value).ContinueWith(t =>
			{
				// Observe any exceptions to prevent unobserved task exceptions
				if (t.Exception != null)
				{
					Logger.LogError(t.Exception, "Unhandled exception in metadata upload");
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}

	T? GetViewModel<T>() where T : class, IViewModel
		=> ViewModelGroups.SelectMany(x => x.ViewModels).SingleOrDefault(x => x is T) as T;

	async Task UploadMetadataAsync(UniqueObjectId objectId)
	{
		var metadataModel = GetViewModel<ObjectMetadataViewModel>()?.Model;

		if (metadataModel == null)
		{
			Logger.LogWarning("Cannot upload metadata - metadata is null");
			return;
		}

		if (Model?.DatInfo == null)
		{
			Logger.LogWarning("Cannot upload metadata - DatInfo is null");
			return;
		}

		try
		{
			Logger.LogInformation("Uploading metadata for object {ObjectId}", objectId);

			// Create DTO from current metadata
			var dtoRequest = new DtoObjectPostResponse(
				Id: objectId,
				Name: metadataModel.InternalName,
				DisplayName: CurrentFile.DisplayName,
				DatChecksum: Model.DatInfo.S5Header.Checksum,
				Description: metadataModel.Description,
				ObjectSource: Model.DatInfo.S5Header.ObjectSource.Convert(
					Model.DatInfo.S5Header.Name,
					Model.DatInfo.S5Header.Checksum),
				ObjectType: Model.DatInfo.S5Header.ObjectType.Convert(),
				VehicleType: null,
				Availability: metadataModel.Availability,
				CreatedDate: metadataModel.CreatedDate.HasValue
					? DateOnly.FromDateTime(metadataModel.CreatedDate.Value.UtcDateTime)
					: null,
				ModifiedDate: metadataModel.ModifiedDate.HasValue
					? DateOnly.FromDateTime(metadataModel.ModifiedDate.Value.UtcDateTime)
					: null,
				UploadedDate: DateOnly.FromDateTime(metadataModel.UploadedDate.UtcDateTime),
				Licence: metadataModel.Licence,
				Authors: metadataModel.Authors,
				Tags: metadataModel.Tags,
				ObjectPacks: metadataModel.ObjectPacks,
				DatObjects: metadataModel.DatObjects,
				StringTable: new DtoStringTableDescriptor([], objectId)
			);

			var result = await EditorContext.ObjectServiceClient.UpdateObjectAsync(objectId, dtoRequest);

			if (result != null)
			{
				Logger.LogInformation("Successfully uploaded metadata for object {ObjectId}", objectId);
			}
			else
			{
				Logger.LogError("Failed to upload metadata for object {ObjectId}", objectId);
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error uploading metadata for object {ObjectId}", objectId);
		}
	}

	public override Task<string?> SaveAsAsync(SaveParameters saveParameters)
		=> SaveAsCoreAsync(saveParameters);

	Task SaveAsUncompressedDatAsync()
		=> SaveAsCoreAsync(new SaveParameters(SaveType.DAT, SawyerEncoding.Uncompressed));

	async Task<string?> SaveAsCoreAsync(SaveParameters saveParameters)
	{
		var fileTypes = saveParameters.SaveType == SaveType.JSON
			? PlatformSpecific.JsonFileTypes
			: PlatformSpecific.DatFileTypes;

		var saveFile = await PlatformSpecific.SaveFilePicker(fileTypes);
		if (saveFile != null)
		{
			SaveCore(saveFile.Path.LocalPath, saveParameters);
			return saveFile.Path.LocalPath;
		}

		return null;
	}

	void SaveCore(string filename, SaveParameters saveParameters)
	{
		if (Model?.LocoObject == null)
		{
			Logger.LogError("Cannot save - loco object was null");
			return;
		}

		//var ovm = GetViewModel<IViewModel>();
		//if (ovm == null)
		//{
		//	logger.Error("Cannot save - loco object viewmodel was null");
		//	return;
		//}

		if (string.IsNullOrEmpty(filename))
		{
			Logger.LogError("Cannot save - filename was empty");
			return;
		}

		var saveDir = Path.GetDirectoryName(filename);

		if (string.IsNullOrEmpty(saveDir))
		{
			Logger.LogError("Cannot save - directory is null or empty");
			return;
		}
		else if (!Directory.Exists(saveDir))
		{
			Logger.LogError("Cannot save - directory does not exist: \"{SaveDir}\"", saveDir);
			return;
		}

		_ = ValidateObject(showPopupOnSuccess: false);

		Logger.LogInformation("Saving {Name} to {Filename}", Model.DatInfo?.S5Header.Name, filename);

		// this is hacky but it should work
		var avm = GetViewModel<AudioViewModel>();
		if (avm != null && Model.LocoObject.Object is SoundObject)
		{
			var datWav = avm.GetAsDatWav(LocoAudioType.SoundEffect);
			if (datWav == null)
			{
				Logger.LogError("AudioViewModel returned null data when trying to save as a sound object");
				return;
			}
		}

		var header = Model.DatInfo?.S5Header;
		if (saveParameters.SaveType == SaveType.DAT && header != null)
		{
			var objectModelHeader = GetViewModel<ObjectModelHeaderViewModel>();

			SawyerStreamWriter.Save(filename,
				objectModelHeader?.Name ?? header.Name,
				objectModelHeader?.ObjectSource ?? header.ObjectSource.Convert(header.Name, header.Checksum),
				saveParameters.SawyerEncoding ?? GetViewModel<ObjectDatHeaderViewModel>()?.Encoding ?? SawyerEncoding.Uncompressed,
				Model.LocoObject,
				Logger,
				EditorContext.Settings.AllowSavingAsVanillaObject);
		}
		else
		{
			JsonSerializer.Serialize(
				new FileStream(filename, FileMode.Create, FileAccess.Write),
				Model.LocoObject,
				options);
		}
	}

	readonly JsonSerializerOptions options = new()
	{
		WriteIndented = true,
		//Converters =
		//{
		//	new LocoStructJsonConverterFactory(),
		//	new ObjectTypeJsonConverter(),
		//	new ObjectSourceJsonConverter(),
		//}
	};
}
