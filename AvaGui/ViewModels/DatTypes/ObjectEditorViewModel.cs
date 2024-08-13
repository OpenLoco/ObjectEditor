using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using OpenLoco.ObjectEditor.Logging;
using System.Threading.Tasks;
using Core.Objects.Sound;
using SixLabors.ImageSharp;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject, ILocoFileViewModel
	{
		public ReactiveCommand<Unit, Unit> ReloadObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveMetadataCommand { get; init; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		ObjectEditorModel Model { get; init; }

		[Reactive]
		public UiLocoFile? CurrentObject { get; private set; }

		[Reactive]
		public ObjectMetadata CurrentMetadata { get; private set; }

		[Reactive]
		public FileSystemItemBase CurrentFile { get; init; }

		ILogger? Logger => Model.Logger;

		public ObjectEditorViewModel(FileSystemItemBase currentFile, ObjectEditorModel model)
		{
			CurrentFile = currentFile;
			Model = model;

			LoadObject();

			ReloadObjectCommand = ReactiveCommand.Create(LoadObject);
			SaveObjectCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsObjectCommand = ReactiveCommand.Create(SaveAsCurrentObject);
			SaveMetadataCommand = ReactiveCommand.Create(SaveCurrentMetadata);
		}

		public void LoadObject()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

			if (CurrentFile == null)
			{
				return;
			}

			Logger?.Info($"Loading {CurrentFile.Name} from {CurrentFile.Path}");

			if (Model.TryLoadObject(CurrentFile.Path, out var newObj))
			{
				CurrentObject = newObj;

				if (CurrentObject?.LocoObject != null)
				{
					StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
					ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject
						? new SoundViewModel(CurrentObject.LocoObject)
						: new ImageTableViewModel(CurrentObject.LocoObject, Model.PaletteMap);

					var name = CurrentObject.DatFileInfo.S5Header.Name.Trim();
					CurrentMetadata = Model.LoadObjectMetadata(name, CurrentObject.DatFileInfo.S5Header.Checksum);
				}
				else
				{
					StringTableViewModel = null;
					ExtraContentViewModel = null;
				}
			}
			else
			{
				// todo: show warnings here
				CurrentObject = null;
			}
		}

		public void SaveCurrentObject()
		{
			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {CurrentFile.Path}");
			SawyerStreamWriter.Save(CurrentFile.Path, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		public void SaveAsCurrentObject()
		{
			var saveFile = Task.Run(PlatformSpecific.SaveFilePicker).Result;
			if (saveFile == null)
			{
				return;
			}

			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {saveFile.Path.AbsolutePath}");
			SawyerStreamWriter.Save(saveFile.Path.AbsolutePath, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		public void SaveCurrentMetadata()
		{
			Model.SaveMetadata();
		}
	}
}
