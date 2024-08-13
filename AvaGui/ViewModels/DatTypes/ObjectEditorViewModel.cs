using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using OpenLoco.ObjectEditor.Logging;
using System.Threading.Tasks;
using Core.Objects.Sound;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject, ILocoFileViewModel
	{
		public ReactiveCommand<Unit, Unit> ReloadObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveAsObjectCommand { get; init; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		ObjectEditorModel Model { get; }

		[Reactive]
		public UiLocoFile? CurrentObject { private set; get; }

		[Reactive]
		public FileSystemItemBase? CurrentFile { get; set; }

		public ObjectEditorViewModel(ObjectEditorModel model)
		{
			Model = model;
			_ = this.WhenAnyValue(o => o.CurrentFile)
				.Subscribe(_ => SelectedObjectChanged());

			ReloadObjectCommand = ReactiveCommand.Create(ReloadCurrentObject);
			SaveObjectCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsObjectCommand = ReactiveCommand.Create(SaveAsCurrentObject);
		}

		public void SelectedObjectChanged()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

			ReloadCurrentObject();

			if (CurrentObject?.LocoObject != null)
			{
				StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
				ExtraContentViewModel = CurrentObject.LocoObject.Object is SoundObject
					? new SoundViewModel(CurrentObject.LocoObject)
					: new ImageTableViewModel(CurrentObject.LocoObject, Model.PaletteMap);
			}
			else
			{
				StringTableViewModel = null;
				ExtraContentViewModel = null;
			}
		}

		public void ReloadCurrentObject()
		{
			if (CurrentFile == null)
			{
				return;
			}

			Logger?.Info($"Loading {CurrentObject?.DatFileInfo.S5Header.Name} from {CurrentFile.Path}");
			if (Model.TryLoadObject(CurrentFile.Path, out var newObj))
			{
				CurrentObject = newObj;
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

		ILogger? Logger => Model.Logger;
	}
}
