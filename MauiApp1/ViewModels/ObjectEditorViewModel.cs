using OpenLoco.ObjectEditor.Data;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace MauiApp1.ViewModels
{
	public class ObjectEditorViewModel2 : ReactiveObject
	{
		public ObservableCollection<DatFile> Filenames { get; init; }

		public ObjectEditorViewModel2(string rootDirectory)
		{
			Filenames = new ObservableCollection<DatFile>(Directory
				.GetFiles(rootDirectory)
				.Where(f => Path.GetExtension(f).ToLower() == ".dat")
				.Select(f => new DatFile(Path.GetFileName(f), ObjectType.InterfaceSkin, "<none>")));

			_ = this.WhenAnyValue(o => o.Filenames)
				.Subscribe(o => this.RaisePropertyChanged(nameof(FilenameCount)));
		}

		public int FilenameCount => Filenames.Count;

	}
}
