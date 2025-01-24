using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects.Sound;
using OpenLoco.Dat.Types;
using OpenLoco.Gui.Models;
using OpenLoco.Gui.Views;
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

namespace OpenLoco.Gui.ViewModels
{
	public class DatObjectEditorViewModel : BaseLocoFileViewModel
	{
		[Reactive]
		public IObjectViewModel<ILocoStruct>? CurrentObjectViewModel { get; set; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		[Reactive]
		public S5HeaderViewModel? S5HeaderViewModel { get; set; }

		[Reactive]
		public UiDatLocoFile? CurrentObject { get; private set; }

		public ReactiveCommand<Unit, Unit> ViewHexCommand { get; }
		public Interaction<HexWindowViewModel, HexWindowViewModel?> HexViewerShowDialog { get; }

		public DatObjectEditorViewModel(FileSystemItemObject currentFile, ObjectEditorModel model)
			: base(currentFile, model)
		{
			Load();

			HexViewerShowDialog = new();
			_ = HexViewerShowDialog.RegisterHandler(DoShowDialogAsync);

			ViewHexCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				var filename = Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename);
				var vm = new HexWindowViewModel(filename, logger);
				_ = await HexViewerShowDialog.Handle(vm);
			});
		}

		async Task DoShowDialogAsync(IInteractionContext<HexWindowViewModel, HexWindowViewModel?> interaction)
		{
			var dialog = new HexViewerWindow
			{
				DataContext = interaction.Input
			};

			var parentWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
			var result = await dialog.ShowDialog<HexWindowViewModel?>(parentWindow);
			interaction.SetOutput(result);
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

			return asm == null
				? new GenericObjectViewModel() { Object = locoStruct }
				: (Activator.CreateInstance(asm, locoStruct) as IObjectViewModel<ILocoStruct>)!;
		}

		public override void Load()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

			logger.Info($"Loading {CurrentFile.DisplayName} from {CurrentFile.Filename}");

			if (Model.TryLoadObject(CurrentFile, out var newObj))
			{
				CurrentObject = newObj;

				if (CurrentObject?.LocoObject != null)
				{
					CurrentObjectViewModel = GetViewModelFromStruct(CurrentObject.LocoObject.Object);
					StringTableViewModel = new(CurrentObject.LocoObject.StringTable);

					var imageNameProvider = (CurrentObject.LocoObject.Object is IImageTableNameProvider itnp)
						? itnp
						: new DefaultImageTableNameProvider();

					ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject soundObject
						? new SoundViewModel(CurrentObject.DatFileInfo.S5Header.Name, soundObject.SoundObjectData.PcmHeader, soundObject.PcmData)
						: new ImageTableViewModel(CurrentObject.LocoObject, imageNameProvider, Model.PaletteMap, CurrentObject.Images, Model.Logger);
				}
				else
				{
					StringTableViewModel = null;
					ExtraContentViewModel = null;
				}

				if (CurrentObject != null)
				{
					S5HeaderViewModel = new S5HeaderViewModel(CurrentObject.DatFileInfo.S5Header);
				}
			}
			else
			{
				// todo: show warnings here
				CurrentObject = null;
				CurrentObjectViewModel = null;
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
			var path = Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename);
			if (File.Exists(path))
			{
				logger.Info($"Deleting file \"{path}\"");
				File.Delete(path);
			}
			else
			{
				logger.Info($"File already deleted \"{path}\"");
			}

			// remove from object index
			Model.ObjectIndex.Delete(x => x.Filename == CurrentFile.Filename);
		}

		public override void Save()
		{
			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? Path.Combine(Model.Settings.ObjDataDirectory, CurrentFile.Filename)
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));
			SaveCore(savePath);
		}

		public override void SaveAs()
		{
			var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
			if (saveFile != null)
			{
				SaveCore(saveFile.Path.LocalPath);
			}
		}

		void SaveCore(string filename)
		{
			if (CurrentObject?.LocoObject == null)
			{
				logger.Error("Cannot save - loco object was null");
				return;
			}

			if (string.IsNullOrEmpty(filename))
			{
				logger.Error("Cannot save - filename was empty");
				return;
			}

			var saveDir = Path.GetDirectoryName(filename);

			if (string.IsNullOrEmpty(saveDir) || !Directory.Exists(saveDir))
			{
				logger.Error("Cannot save - directory is invalid");
				return;
			}

			logger.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {filename}");
			StringTableViewModel?.WriteTableBackToObject();

			if (CurrentObjectViewModel is not null and not GenericObjectViewModel)
			{
				CurrentObject.LocoObject.Object = CurrentObjectViewModel.GetAsUnderlyingType(CurrentObject.LocoObject.Object);
			}

			SawyerStreamWriter.Save(filename,
				S5HeaderViewModel?.Name ?? CurrentObject.DatFileInfo.S5Header.Name,
				S5HeaderViewModel?.SourceGame ?? CurrentObject.DatFileInfo.S5Header.SourceGame,
				SawyerEncoding.Uncompressed, // todo: change based on what user selected
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
}
