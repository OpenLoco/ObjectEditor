using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using ReactiveUI.Fody.Helpers;
using OpenLoco.ObjectEditor.Types;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		[Reactive] public StringTableViewModel StringTableViewModel { get; set; }

		[Reactive] public ImageTableViewModel ImageTableViewModel { get; set; }

		ObjectEditorModel Model { get; }

		public UiLocoFile _currentObject;
		public UiLocoFile? CurrentObject
		{
			get => CurrentlySelectedObject == null || !Model.TryGetObject(CurrentlySelectedObject.Path, out var value)
				? null
				: value;
			set => Model.ObjectCache[CurrentlySelectedObject.Path] = value;
		}

		[Reactive] public FileSystemItemBase CurrentlySelectedObject { get; set; }

		DatFileInfo _currentlySelectedUiObjectDatInfo;
		public DatFileInfo CurrentlySelectedUiObjectDatInfo
		{
			get
			{
				if (CurrentlySelectedObject is null or not FileSystemItem)
				{
					return new(null, null);
				}
				_currentlySelectedUiObjectDatInfo = CurrentObject.DatFileInfo;
				return _currentlySelectedUiObjectDatInfo;
			}
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedUiObjectDatInfo, value);
		}

		#region StringTable

		public void SelectedObjectChanged()
		{
			if (CurrentObject?.LocoObject != null)
			{
				StringTableViewModel = new(CurrentObject.LocoObject.StringTable);
				ImageTableViewModel = new(CurrentObject.LocoObject, Model.PaletteMap);
			}
			else
			{
				StringTableViewModel = null;
				ImageTableViewModel = null;
			}
		}

		#endregion StringTable

		IObjectViewModel _currentObjectViewModel;
		public IObjectViewModel CurrentObjectViewModel
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
