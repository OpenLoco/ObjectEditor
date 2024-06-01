using ReactiveUI;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Objects;
using System.Linq;
using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		//public StringTableViewModel StringTableViewModel { get; set; }

		[Reactive] public ImageTableViewModel ImageTableViewModel { get; set; }

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

		[Reactive] public FileSystemItemBase CurrentlySelectedObject { get; set; }

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

		#region StringTable

		[Reactive] public ObservableCollection<string> Strings { get; set; }

		[Reactive] public string SelectedString { get; set; }

		[Reactive] public ObservableCollection<LanguageTranslation> TranslationTable { get; set; }

		public void SelectedObjectChanged()
		{
			if (CurrentObject?.LocoObject != null)
			{
				Strings = new ObservableCollection<string>(CurrentObject.LocoObject.StringTable.Table.Keys);
				ImageTableViewModel = new(CurrentObject.LocoObject, Model.PaletteMap);
			}
			else
			{
				Strings = new ObservableCollection<string>();
				ImageTableViewModel = null;
			}
		}

		public void SelectedStringChanged()
		{
			if (CurrentObject?.LocoObject != null && SelectedString != null && CurrentObject.LocoObject.StringTable.Table.TryGetValue(SelectedString, out var value))
			{
				TranslationTable = new ObservableCollection<LanguageTranslation>(value.Select(kvp => new LanguageTranslation(kvp.Key, kvp.Value)));

				foreach (var kvp in TranslationTable)
				{
					_ = kvp.WhenAnyValue(o => o.Translation)
						.Subscribe(_ => CurrentObject.LocoObject.StringTable.Table[SelectedString][kvp.Language] = kvp.Translation);
				}
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
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentlySelectedUiObjectDatInfo)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObjectViewModel)));
			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentObject)));
			_ = this.WhenAnyValue(o => o.CurrentObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(Strings)));
			_ = this.WhenAnyValue(o => o.CurrentObject)
				.Subscribe(_ => SelectedObjectChanged());
			//_ = this.WhenAnyValue(o => o.CurrentObject)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(ImageTableViewModel)));
			_ = this.WhenAnyValue(o => o.Strings)
				.Subscribe(o => this.RaisePropertyChanged(nameof(TranslationTable)));
			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(_ => SelectedStringChanged());
			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(o => this.RaisePropertyChanged(nameof(TranslationTable)));
		}
	}
}
