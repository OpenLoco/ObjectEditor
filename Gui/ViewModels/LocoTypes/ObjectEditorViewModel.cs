using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Common;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Types;
using Gui.Models;
using Gui.Models.Audio;
using Gui.ViewModels.Graphics;
using Gui.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ObjectEditorViewModel : BaseFileViewModel
{
	[Reactive]
	public IObjectViewModel? CurrentObjectViewModel { get; set; }

	[Reactive]
	public StringTableViewModel? StringTableViewModel { get; set; }

	[Reactive]
	public IExtraContentViewModel? ExtraContentViewModel { get; set; }

	[Reactive]
	public string ExtraContentViewModelTabName { get; set; }

	[Reactive]
	public LocoUIObjectModel? CurrentObject { get; private set; }

	[Reactive]
	public LocoObjectMetadataViewModel? MetadataViewModel { get; set; }

	[Reactive]
	public ObjectModelHeaderViewModel? ObjectModelHeaderViewModel { get; set; }

	[Reactive]
	public ObjectDatHeaderViewModel? ObjectDatHeaderViewModel { get; set; }

	public ReactiveCommand<Unit, Unit> ExportUncompressedCommand { get; }

	public ReactiveCommand<GameObjDataFolder, Unit> CopyToGameObjDataCommand { get; }
	public ReactiveCommand<Unit, Unit> ValidateForOGCommand { get; }

	[Reactive]
	public GameObjDataFolder LastGameObjDataFolder { get; set; } = GameObjDataFolder.LocomotionSteam;

	//public ReactiveCommand<Unit, ObjectIndexEntry?> SelectObjectCommand { get; }
	public Interaction<ObjectSelectionWindowViewModel, ObjectSelectionWindowViewModel?> SelectObjectShowDialog { get; }

	public ObjectEditorViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model)
	{
		Load();

		ExportUncompressedCommand = ReactiveCommand.Create(SaveAsUncompressedDat);

		CopyToGameObjDataCommand = ReactiveCommand.Create((GameObjDataFolder targetFolder) =>
		{
			var folder = model.Settings.GetGameObjDataFolder(targetFolder);
			if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
			{
				logger.Error($"The specified [{targetFolder}] ObjData directory is invalid: \"{folder}\"");
				return;
			}

			LastGameObjDataFolder = targetFolder;

			try
			{
				File.Copy(currentFile.FileName, Path.Combine(folder, Path.GetFileName(currentFile.FileName)));
				logger.Info($"Copied {Path.GetFileName(currentFile.FileName)} to [[{targetFolder}]] {folder}");
			}
			catch (Exception ex)
			{
				logger.Error($"Could not copy {currentFile.FileName} to {folder}:", ex);
			}
		});

		ValidateForOGCommand = ReactiveCommand.Create(ValidateForOG);

		SelectObjectShowDialog = new();
		_ = SelectObjectShowDialog.RegisterHandler(DoShowDialogAsync<ObjectSelectionWindowViewModel, ObjectSelectionWindow>);

		_ = this.WhenAnyValue(x => x.ExtraContentViewModel)
			.Subscribe(x => ExtraContentViewModelTabName = ExtraContentViewModel?.Name ?? "<no-extra-content>");

		//SelectObjectCommand = ReactiveCommand.CreateFromTask(async () =>
		//{
		//	var objects = model.ObjectIndex.Objects.Where(x => x.ObjectType == ObjectType.Tree);
		//	var vm = new ObjectSelectionWindowViewModel(objects);
		//	var result = await SelectObjectShowDialog.Handle(vm);
		//	return result.SelectedObject;
		//});
	}

	void ValidateForOG()
	{
		var validationErrors = new List<string>();

		if (CurrentObject?.DatInfo is null)
		{
			validationErrors.Add("Object DAT info is null");
			return;
		}

		// split the CurrentFile path on "opengraphics" folder
		var dir = Path.GetRelativePath(Model.Settings.ObjDataDirectory, CurrentFile.FileName);
		var parentDirName = Path.GetFileName(Path.GetDirectoryName(CurrentFile.FileName));

		if (OriginalObjectFiles.Names.TryGetValue(parentDirName, out var fileInfo))
		{
			// DAT name is the expected dat name
			if (CurrentObject.DatInfo.S5Header.Name != fileInfo.OpenGraphicsName)
			{
				validationErrors.Add($"✖ Internal DAT header name is not correct. Actual=\"{CurrentObject.DatInfo.S5Header.Name}\" Expected=\"{fileInfo.OpenGraphicsName}\" ");
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
		if (CurrentObject.DatInfo.S5Header.Name.Contains('_'))
		{
			validationErrors.Add("✖ Internal header name should not contain an underscore");
		}

		// DAT name is prefixed by OG
		if (!CurrentObject.DatInfo.S5Header.Name.StartsWith("OG"))
		{
			validationErrors.Add("✖ Internal header name is not prefixed with OG");
		}

		// OpenGraphics object source set
		if (CurrentObject.DatInfo.S5Header.ObjectSource != DatObjectSource.OpenLoco)
		{
			validationErrors.Add("✖ Object source is not set to OpenLoco");
		}

		// if Vehicle - use RunLengthSingle
		if (CurrentObject.DatInfo.S5Header.ObjectType == DatObjectType.Vehicle && CurrentObject.DatInfo.ObjectHeader.Encoding != SawyerEncoding.RunLengthSingle)
		{
			validationErrors.Add("✖ Object is a Vehicle but doesn't have encoding set to RunLengthSingle");
		}

		// Show message box
		IMsBox<ButtonResult> box;
		if (validationErrors.Count != 0)
		{
			var errorMsg = string.Join(Environment.NewLine, validationErrors);

			box = MessageBoxManager.GetMessageBoxStandard(
				"OG validation failed",
				errorMsg,
				ButtonEnum.Ok,
				Icon.Error);
		}
		else
		{
			box = MessageBoxManager.GetMessageBoxStandard(
				"OG validation succeeded",
				"✔ No issues found. Object is valid for OpenGraphics.",
				ButtonEnum.Ok,
				Icon.Success);
		}

		_ = box.ShowAsync();
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

	public static IObjectViewModel? GetViewModelFromStruct(ILocoStruct locoStruct)
	{
		var asm = Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.SingleOrDefault(type
				=> type.IsClass
				&& !type.IsAbstract
				&& type.BaseType?.IsGenericType == true
				&& type.BaseType.GetGenericTypeDefinition() == typeof(LocoObjectViewModel<>)
				&& type.BaseType.GenericTypeArguments.Single() == locoStruct.GetType());

		return asm == null
			? null
			: (IObjectViewModel?)Activator.CreateInstance(asm, locoStruct);
	}

	public override void Load()
	{
		// this stops any currently-playing sounds
		if (ExtraContentViewModel is AudioViewModel svm)
		{
			svm.Dispose();
		}

		logger.Info($"Loading {CurrentFile.DisplayName} from {CurrentFile.FileName}");

		if (Model.TryLoadObject(CurrentFile, out var newObj))
		{
			CurrentObject = newObj;

			if (CurrentObject?.LocoObject != null)
			{
				CurrentObjectViewModel = GetViewModelFromStruct(CurrentObject.LocoObject.Object);
				StringTableViewModel = new(CurrentObject.LocoObject.StringTable);

				if (CurrentObject.LocoObject.Object is SoundObject soundObject)
				{
					ExtraContentViewModel = new AudioViewModel(logger, CurrentObject.Metadata.InternalName, soundObject.SoundObjectData.PcmHeader, soundObject.PcmData);
				}
				else
				{
					CurrentObject.LocoObject.ImageTable?.PaletteMap = Model.PaletteMap;
					if (CurrentObject.LocoObject.ImageTable == null)
					{
						logger.Info($"{CurrentFile.DisplayName} has no image table");
					}
					else
					{
						var bc = CurrentObject.LocoObject.ObjectType == ObjectType.Building ? (CurrentObject.LocoObject.Object as IHasBuildingComponents)?.BuildingComponents : null;
						ExtraContentViewModel = new ImageTableViewModel(CurrentObject.LocoObject.ImageTable, Model.Logger, bc);
					}
				}
			}
			else
			{
				StringTableViewModel = null;
				ExtraContentViewModel = null;
			}

			if (CurrentObject != null)
			{
				ObjectModelHeaderViewModel = new ObjectModelHeaderViewModel(CurrentObject.DatInfo.S5Header.Convert());

				// an object saved as 'Vanilla' but isn't truly Vanilla will have its source auto-set to Custom
				// but in this case, we do want to show the user this, so we'll default to Steam
				if (CurrentObject.DatInfo.S5Header.ObjectSource == DatObjectSource.Vanilla && ObjectModelHeaderViewModel.ObjectSource == ObjectSource.Custom)
				{
					ObjectModelHeaderViewModel.ObjectSource = ObjectSource.LocomotionSteam;
				}

				ObjectDatHeaderViewModel = new ObjectDatHeaderViewModel(
					CurrentObject.DatInfo.S5Header.Checksum,
					CurrentObject.DatInfo.ObjectHeader.Encoding,
					CurrentObject.DatInfo.ObjectHeader.DataLength);
			}

			if (CurrentObject?.Metadata != null)
			{
				MetadataViewModel = new LocoObjectMetadataViewModel(CurrentObject.Metadata);
			}
			else
			{
				// todo: show warnings here
				// in online mode, vanilla objects won't be downloaded so they hit this case, which is a valid use-case
				CurrentObject = null;
				CurrentObjectViewModel = null;
			}
		}
	}

	public override void Delete()
	{
		if (CurrentFile.FileLocation != FileLocation.Local)
		{
			logger.Error("Cannot delete non-local files");
			return;
		}

		// delete file
		if (File.Exists(CurrentFile.FileName))
		{
			logger.Info($"Deleting file \"{CurrentFile.FileName}\"");
			File.Delete(CurrentFile.FileName);
		}
		else
		{
			logger.Info($"File already deleted \"{CurrentFile.FileName}\"");
		}

		// note: it is not really possible to delete the entry from the index since if the user
		// has changed objdata folders but still has this item tab open, then there is no way
		// to delete. user can reindex to fix, or rely on automatic reindex at startup
	}

	public override void Save()
	{
		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? CurrentFile.FileName
			: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension($"{CurrentFile.DisplayName}-{CurrentFile.Id}", ".dat"));
		SaveCore(savePath, new SaveParameters(SaveType.DAT, ObjectDatHeaderViewModel?.DatEncoding));
	}

	public override string? SaveAs(SaveParameters saveParameters)
		=> SaveAsCore(saveParameters);

	void SaveAsUncompressedDat()
		=> _ = SaveAsCore(new SaveParameters(SaveType.DAT, SawyerEncoding.Uncompressed));

	string? SaveAsCore(SaveParameters saveParameters)
	{
		var fileTypes = saveParameters.SaveType == SaveType.JSON
			? PlatformSpecific.JsonFileTypes
			: PlatformSpecific.DatFileTypes;

		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(fileTypes)).Result;
		if (saveFile != null)
		{
			SaveCore(saveFile.Path.LocalPath, saveParameters);
			return saveFile.Path.LocalPath;
		}
		return null;
	}

	void SaveCore(string filename, SaveParameters saveParameters)
	{
		if (CurrentObject?.LocoObject == null)
		{
			logger.Error("Cannot save - loco object was null");
			return;
		}

		if (CurrentObjectViewModel == null)
		{
			logger.Error("Cannot save - loco object viewmodel was null");
			return;
		}

		if (string.IsNullOrEmpty(filename))
		{
			logger.Error("Cannot save - filename was empty");
			return;
		}

		var saveDir = Path.GetDirectoryName(filename);

		if (string.IsNullOrEmpty(saveDir))
		{
			logger.Error("Cannot save - directory is null or empty");
			return;
		}
		else if (!Directory.Exists(saveDir))
		{
			logger.Error($"Cannot save - directory does not exist: \"{saveDir}\"");
			return;
		}

		logger.Info($"Saving {CurrentObject.DatInfo.S5Header.Name} to {filename}");
		StringTableViewModel?.WriteTableBackToObject();

		// VM should auto-copy back now for everything but VehicleObject and BuildingObject
		CurrentObjectViewModel.CopyBackToModel();

		// this is hacky but it should work
		if (ExtraContentViewModel is AudioViewModel avm && CurrentObject.LocoObject.Object is SoundObject so)
		{
			var datWav = avm.GetAsDatWav(LocoAudioType.SoundEffect);
			if (datWav == null)
			{
				logger.Error("AudioViewModel returned null data when trying to save as a sound object");
				return;
			}
			//CurrentObject.LocoObject.Object = so with
			//{
			//	PcmData = datWav.Value.Data,
			//	SoundObjectData = so.SoundObjectData with
			//	{
			//		PcmHeader = datWav.Value.Header,
			//		Length = (uint)datWav.Value.Data.Length
			//	}
			//};
		}

		if (saveParameters.SaveType == SaveType.DAT)
		{
			var header = CurrentObject.DatInfo.S5Header;

			SawyerStreamWriter.Save(filename,
				ObjectModelHeaderViewModel?.Name ?? header.Name,
				ObjectModelHeaderViewModel?.ObjectSource ?? header.ObjectSource.Convert(header.Name, header.Checksum),
				saveParameters.SawyerEncoding ?? ObjectDatHeaderViewModel?.DatEncoding ?? SawyerEncoding.Uncompressed,
				CurrentObject.LocoObject,
				logger,
				Model.Settings.AllowSavingAsVanillaObject);
		}
		else
		{
			JsonSerializer.Serialize(
				new FileStream(filename, FileMode.Create, FileAccess.Write),
				CurrentObject.LocoObject,
				new JsonSerializerOptions
				{
					WriteIndented = true,
					//Converters =
					//{
					//	new LocoStructJsonConverterFactory(),
					//	new ObjectTypeJsonConverter(),
					//	new ObjectSourceJsonConverter(),
					//}
				});
		}
	}
}

public class TreeNode(string title, string offsetText, ObservableCollection<TreeNode> nodes)
{
	public ObservableCollection<TreeNode> Nodes { get; } = nodes;
	public string Title { get; } = title;
	public string OffsetText { get; } = offsetText;
	public TreeNode() : this("<empty>", "<empty>") { }

	public TreeNode(string title, string offsetText) : this(title, offsetText, []) { }
}
