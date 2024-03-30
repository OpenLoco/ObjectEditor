using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		//public StringTableViewModel StringTableViewModel { get; set; }

		ObjectEditorModel Model { get; }

		public UiLocoFile _currentObject;
		public UiLocoFile CurrentObject
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

		public BindingList<string> _stringTableKeys;
		public BindingList<string> StringTableKeys
		{
			get
			{
				if (CurrentObject != null)
				{
					return new BindingList<string>(CurrentObject.LocoObject.StringTable.Table.Keys.ToList());
				}
				return null;
			}
			set => this.RaiseAndSetIfChanged(ref _stringTableKeys, value);
		}

		public string _currentlySelectedStringTableKey;
		public string CurrentlySelectedStringTableKey
		{
			get => _currentlySelectedStringTableKey;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedStringTableKey, value);
		}

		public BindingList<KeyValuePair<LanguageId, string>> _stringTableStringKeys;
		public BindingList<KeyValuePair<LanguageId, string>> StringTableStringKeys
		{
			get
			{
				if (CurrentObject != null && CurrentlySelectedStringTableKey != null && CurrentObject.LocoObject.StringTable.Table.ContainsKey(CurrentlySelectedStringTableKey))
				{
					return new BindingList<KeyValuePair<LanguageId, string>>(CurrentObject.LocoObject.StringTable.Table[CurrentlySelectedStringTableKey].ToList());
					//.Select(kvp => new(kvp.Key.ToString(), kvp.Value))
					//.ToList());
				}
				return null;
			}
			set => this.RaiseAndSetIfChanged(ref _stringTableStringKeys, value);
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
			//StringTableViewModel = new(new OpenLoco.ObjectEditor.Types.StringTable());

			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentlySelectedUiObjectDatInfo)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObjectViewModel)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObject)));
			_ = this.WhenAnyValue(o => o.CurrentObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(StringTableKeys)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedStringTableKey)
				.Subscribe(o => this.RaisePropertyChanged(nameof(StringTableStringKeys)));
		}
	}
}
