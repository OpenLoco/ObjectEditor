using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; }

		public UiLocoObject _currentObject;
		public UiLocoObject CurrentObject
		{
			get
			{
				if (CurrentlySelectedObject == null || !Model.ObjectCache.ContainsKey(CurrentlySelectedObject.Path))
				{
					return null;
				}
				return Model.ObjectCache[CurrentlySelectedObject.Path];
			}
			set => Model.ObjectCache[CurrentlySelectedObject.Path] = value;
		}

		public FileSystemItemBase _currentlySelectedObject;
		public FileSystemItemBase CurrentlySelectedObject
		{
			get => _currentlySelectedObject;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedObject, value);
		}

		DatFileInfo _currentlySelectedUiObjectDatInfo;
		public DatFileInfo CurrentlySelectedUiObjectDatInfo
		{
			get
			{
				if (CurrentlySelectedObject == null || CurrentlySelectedObject is not FileSystemItem)
				{
					return new(null, null);
				}
				_currentlySelectedUiObjectDatInfo = CurrentObject.DatFileInfo;
				return _currentlySelectedUiObjectDatInfo;
			}
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedUiObjectDatInfo, value);
		}

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

			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentlySelectedUiObjectDatInfo)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObjectViewModel)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObject)));
		}
	}
}
