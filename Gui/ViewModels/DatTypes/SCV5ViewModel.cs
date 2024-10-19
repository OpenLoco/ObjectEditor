using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Gui.Models;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	public class SCV5ViewModel : BaseLocoFileViewModel
	{
		public SCV5ViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		[Reactive]
		public BindingList<S5Header>? RequiredObjects { get; set; }

		[Reactive]
		public BindingList<S5Header>? PackedObjects { get; set; }

		[Reactive]
		public BindingList<TileElement>? TileElements { get; set; }

		//[Reactive]
		//public List<TileElement>[,] TileElementMap { get; set; }

		//[Reactive]
		//public Image<Rgba32> Map { get; set; }

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
			RequiredObjects = new BindingList<S5Header>(CurrentS5File!.RequiredObjects);
			PackedObjects = new BindingList<S5Header>(CurrentS5File!.PackedObjects.ConvertAll(x => x.Item1)); // note: cannot bind to this, but it'll allow us to display at least
			TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);

			//Map = new Image<Rgba32>(384, 384);

			// todo: find a way to actually render this in the UI. code here works fine, just dunno the ui
			//TileElementMap = new List<TileElement>[Limits.kMapColumns, Limits.kMapRows];

			//var i = 0;
			//for (var y = 0; y < Limits.kMapRows; ++y)
			//{
			//	for (var x = 0; x < Limits.kMapColumns; ++x)
			//	{
			//		TileElementMap[x, y] = [];
			//		do
			//		{
			//			TileElementMap[x, y].Add(CurrentS5File!.TileElements[i]);
			//			i++;
			//		}
			//		while (!CurrentS5File!.TileElements[i - 1].IsLast());
			//	}
			//}

			//TileElements = new BindingList<TileElement>(CurrentS5File!.TileElements);
		}

		public override void Save() => Logger?.Error("Saving SC5/SV5 is not implemented yet");

		public override void SaveAs() => Logger?.Error("Saving SC5/SV5 is not implemented yet");
	}
}
