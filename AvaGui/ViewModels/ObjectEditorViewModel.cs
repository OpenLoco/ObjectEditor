using ReactiveUI;
using OpenLoco.ObjectEditor.AvaGui.Models;
using AvaGui.Models;
using OpenLoco.ObjectEditor.DatFileParsing;
using System;

namespace AvaGui.ViewModels
{
	public class ObjectEditorViewModel : ReactiveObject
	{
		ObjectEditorModel Model { get; }

		public FileSystemItemBase _currentlySelectedObject;
		public FileSystemItemBase CurrentlySelectedObject
		{
			get => _currentlySelectedObject;
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedObject, value);
		}

		public DatFileInfo _currentlySelectedUiObject;
		public DatFileInfo CurrentlySelectedUiObject
		{
			get
			{
				if (CurrentlySelectedObject == null || CurrentlySelectedObject is FileSystemItemGroup)
				{
					return new(null, null);
				}
				_currentlySelectedUiObject = Model.ObjectCache[CurrentlySelectedObject.Path].DatFileInfo;
				return _currentlySelectedUiObject;
			}
			set => this.RaiseAndSetIfChanged(ref _currentlySelectedUiObject, value);
		}

		public ObjectEditorViewModel(ObjectEditorModel model)
		{
			Model = model;

			_ = this.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => this.RaisePropertyChanged(nameof(CurrentlySelectedUiObject)));
		}
	}
}
