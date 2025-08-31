using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Sound;
using Gui.Models;
using Gui.Models.Audio;
using Gui.ViewModels.Graphics;
using Gui.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ObjectEditorViewModel : BaseLocoFileViewModel
{
	[Reactive]
	public IObjectViewModel<ILocoStruct>? CurrentObjectViewModel { get; set; }

	[Reactive]
	public StringTableViewModel? StringTableViewModel { get; set; }

	[Reactive]
	public IExtraContentViewModel? ExtraContentViewModel { get; set; }

	[Reactive]
	public ObjectModelHeaderViewModel? ObjectModelHeaderViewModel { get; set; }

	[Reactive]
	public ObjectHeaderViewModel? ObjectHeaderViewModel { get; set; }

	[Reactive]
	public MetadataViewModel? MetadataViewModel { get; set; }

	[Reactive]
	public UiDatLocoFile? CurrentObject { get; private set; }

	public ReactiveCommand<Unit, Unit> ExportUncompressedCommand { get; }

	public ReactiveCommand<GameObjDataFolder, Unit> CopyToGameObjDataCommand { get; }

	[Reactive]
	public GameObjDataFolder LastGameObjDataFolder { get; set; } = GameObjDataFolder.Locomotion;

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

		SelectObjectShowDialog = new();
		_ = SelectObjectShowDialog.RegisterHandler(DoShowDialogAsync<ObjectSelectionWindowViewModel, ObjectSelectionWindow>);

		//SelectObjectCommand = ReactiveCommand.CreateFromTask(async () =>
		//{
		//	var objects = model.ObjectIndex.Objects.Where(x => x.ObjectType == ObjectType.Tree);
		//	var vm = new ObjectSelectionWindowViewModel(objects);
		//	var result = await SelectObjectShowDialog.Handle(vm);
		//	return result.SelectedObject;
		//});
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

	public static IObjectViewModel<ILocoStruct> GetViewModelFromStruct(ILocoStruct locoStruct)
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

		//return asm == null
		//	? new GenericObjectViewModel() { Object = locoStruct }
		//	: (Activator.CreateInstance(asm, locoStruct) as IObjectViewModel<ILocoStruct>)!;

		return Activator.CreateInstance(asm, locoStruct) as IObjectViewModel<ILocoStruct>;
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

				//var imageNameProvider = (CurrentObject.LocoObject.Object is IImageTableNameProvider itnp)
				//	? itnp
				//	: new DefaultImageTableNameProvider();

				ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject soundObject
					? new AudioViewModel(logger, CurrentObject.DatFileInfo.S5Header.Name, soundObject.SoundObjectData.PcmHeader, soundObject.PcmData)
					: new ImageTableViewModel(CurrentObject.LocoObject.ImageTable, Model.PaletteMap, Model.Logger, (CurrentObject.LocoObject.Object as IHasBuildingComponents)?.BuildingComponents);
			}
			else
			{
				StringTableViewModel = null;
				ExtraContentViewModel = null;
			}

			if (CurrentObject != null)
			{
				ObjectModelHeaderViewModel = new ObjectModelHeaderViewModel(CurrentObject.DatFileInfo.S5Header.Convert());
				ObjectHeaderViewModel = new ObjectHeaderViewModel(CurrentObject.DatFileInfo.ObjectHeader);
			}

			if (CurrentObject?.Metadata != null)
			{
				MetadataViewModel = new MetadataViewModel(CurrentObject.Metadata);
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

	public string ExtraContentViewModelTabName
		=> ExtraContentViewModel == null ? "<no content>" : ExtraContentViewModel.Name;

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
		SaveCore(savePath);
	}

	public override void SaveAs()
		=> SaveAsCore();

	void SaveAsUncompressedDat()
		=> SaveAsCore(SawyerEncoding.Uncompressed);

	void SaveAsCore(SawyerEncoding? encodingToUse = null)
	{
		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
		if (saveFile != null)
		{
			SaveCore(saveFile.Path.LocalPath, encodingToUse);
		}
	}

	void SaveCore(string filename, SawyerEncoding? encodingToUse = null)
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

		logger.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {filename}");
		StringTableViewModel?.WriteTableBackToObject();

		CurrentObject.LocoObject.Object = CurrentObjectViewModel.GetAsModel();

		if (ExtraContentViewModel is ImageTableViewModel itvm)
		{
			CurrentObject.LocoObject.ImageTable.GraphicsElements = itvm.GroupedImageViewModels.SelectMany(x => x.Images).Select(x => x.ToGraphicsElement()).ToList();
		}

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

		SawyerStreamWriter.Save(filename,
			ObjectModelHeaderViewModel?.Name ?? CurrentObject.DatFileInfo.S5Header.Name,
			ObjectModelHeaderViewModel?.ObjectSource ?? CurrentObject.DatFileInfo.S5Header.ObjectSource.Convert(),
			encodingToUse ?? ObjectHeaderViewModel?.Encoding ?? SawyerEncoding.Uncompressed,
			CurrentObject.LocoObject,
			logger,
			Model.Settings.AllowSavingAsVanillaObject);
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
