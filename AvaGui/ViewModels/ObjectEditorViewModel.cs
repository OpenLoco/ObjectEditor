using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using ReactiveUI.Fody.Helpers;
using OpenLoco.ObjectEditor.Headers;
using System.Reactive;
using OpenLoco.ObjectEditor.Logging;
using System.Threading.Tasks;
using Core.Objects.Sound;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		public ReactiveCommand<Unit, Unit> ReloadObjectCommand { get; init; }
		public ReactiveCommand<Unit, Unit> SaveObjectCommand { get; init; }
		public ReactiveCommand<Unit, Task> SaveAsObjectCommand { get; init; }

		[Reactive]
		public StringTableViewModel? StringTableViewModel { get; set; }

		[Reactive]
		public IExtraContentViewModel? ExtraContentViewModel { get; set; }

		ObjectEditorModel Model { get; }

		public UiLocoFile _currentObject;
		public UiLocoFile? CurrentObject
		{
			get => CurrentlySelectedObject == null || !Model.TryGetObject(CurrentlySelectedObject.Path, out var value)
				? null
				: value;
			set
			{
				Model.ObjectCache[CurrentlySelectedObject.Path] = value;
				_ = this.RaiseAndSetIfChanged(ref _currentObject, value);
			}
		}

		[Reactive]
		public FileSystemItemBase CurrentlySelectedObject { get; set; }

		DatFileInfo _currentlySelectedUiObjectDatInfo;
		public DatFileInfo CurrentlySelectedUiObjectDatInfo
		{
			get
			{
				if (CurrentlySelectedObject is null or not FileSystemItem || CurrentObject is null)
				{
					return new(S5Header.NullHeader, ObjectHeader.NullHeader);
				}
				_currentlySelectedUiObjectDatInfo = CurrentObject.DatFileInfo;
				return _currentlySelectedUiObjectDatInfo;
			}
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedUiObjectDatInfo, value);
		}

		#region StringTable

		public void SelectedObjectChanged()
		{
			// this stops any currently-playing sounds
			if (ExtraContentViewModel is SoundViewModel svm)
			{
				svm.Dispose();
			}

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

		#endregion StringTable

		IObjectViewModel _currentObjectViewModel;
		public IObjectViewModel? CurrentObjectViewModel
		{
			get
			{
				if (CurrentlySelectedUiObjectDatInfo.S5Header == null)
				{
					return null;
				}
				return CurrentlySelectedUiObjectDatInfo.S5Header.ObjectType switch
				{
					ObjectType.Vehicle => new VehicleViewModel() { Object = CurrentObject.LocoObject.Object as VehicleObject },
					//ObjectType.InterfaceSkin => throw new NotImplementedException(),
					//ObjectType.Sound => throw new NotImplementedException(),
					//ObjectType.Currency => throw new NotImplementedException(),
					//ObjectType.Steam => throw new NotImplementedException(),
					//ObjectType.CliffEdge => throw new NotImplementedException(),
					//ObjectType.Water => throw new NotImplementedException(),
					//ObjectType.Land => throw new NotImplementedException(),
					//ObjectType.TownNames => throw new NotImplementedException(),
					//ObjectType.Cargo => throw new NotImplementedException(),
					//ObjectType.Wall => throw new NotImplementedException(),
					//ObjectType.TrainSignal => throw new NotImplementedException(),
					//ObjectType.LevelCrossing => throw new NotImplementedException(),
					//ObjectType.StreetLight => throw new NotImplementedException(),
					//ObjectType.Tunnel => throw new NotImplementedException(),
					//ObjectType.Bridge => throw new NotImplementedException(),
					//ObjectType.TrainStation => throw new NotImplementedException(),
					//ObjectType.TrackExtra => throw new NotImplementedException(),
					//ObjectType.Track => throw new NotImplementedException(),
					//ObjectType.RoadStation => throw new NotImplementedException(),
					//ObjectType.RoadExtra => throw new NotImplementedException(),
					//ObjectType.Road => throw new NotImplementedException(),
					//ObjectType.Airport => throw new NotImplementedException(),
					//ObjectType.Dock => throw new NotImplementedException(),
					//ObjectType.Tree => throw new NotImplementedException(),
					//ObjectType.Snow => throw new NotImplementedException(),
					//ObjectType.Climate => throw new NotImplementedException(),
					//ObjectType.HillShapes => throw new NotImplementedException(),
					//ObjectType.Building => throw new NotImplementedException(),
					//ObjectType.Scaffolding => throw new NotImplementedException(),
					//ObjectType.Industry => throw new NotImplementedException(),
					//ObjectType.Region => throw new NotImplementedException(),
					//ObjectType.Competitor => throw new NotImplementedException(),
					//ObjectType.ScenarioText => throw new NotImplementedException(),
					_ => new BuildingViewModel(),
				};
			}
			set => this.RaiseAndSetIfChanged(ref _currentObjectViewModel, value);
		}

		public void SaveCurrentObject()
		{
			if (CurrentObject?.LocoObject == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			Logger?.Info($"Saving {CurrentObject.DatFileInfo.S5Header.Name} to {CurrentlySelectedObject.Path}");
			SawyerStreamWriter.Save(CurrentlySelectedObject.Path, CurrentObject.DatFileInfo.S5Header.Name, CurrentObject.LocoObject);
		}

		public void ReloadCurrentObject()
		{
			Logger?.Info($"Reloading {CurrentObject?.DatFileInfo.S5Header.Name} from {CurrentlySelectedObject.Path}");
			if (Model.TryGetObject(CurrentlySelectedObject.Path, out var newObj, reload: true))
			{
				CurrentObject = newObj;
			}
		}

		public async Task SaveAsCurrentObject()
		{
			var saveFile = await PlatformSpecific.SaveFilePicker();
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

		public ObjectEditorViewModel(ObjectEditorModel model)
		{
			Model = model;
			//StringTableViewModel = new(new OpenLoco.ObjectEditor.Types.StringTable());

			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentlySelectedUiObjectDatInfo)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentObjectViewModel)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentObject)));
			_ = this.WhenAnyValue(o => o.CurrentObject)
				.Subscribe(_ => SelectedObjectChanged());

			ReloadObjectCommand = ReactiveCommand.Create(ReloadCurrentObject);
			SaveObjectCommand = ReactiveCommand.Create(SaveCurrentObject);
			SaveAsObjectCommand = ReactiveCommand.Create(SaveAsCurrentObject);

			//_ = this.WhenAnyValue(o => o.CurrentObject)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(StringTableViewModel))); // done in SelectedObjectChanged()
			//_ = this.WhenAnyValue(o => o.CurrentObject)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(ImageTableViewModel))); // done in SelectedObjectChanged()
			//_ = this.WhenAnyValue(o => o.Strings)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(TranslationTable)));
			//_ = this.WhenAnyValue(o => o.SelectedString)
			//	.Subscribe(_ => SelectedStringChanged());
			//_ = this.WhenAnyValue(o => o.SelectedString)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(TranslationTable)));
		}
	}
}
