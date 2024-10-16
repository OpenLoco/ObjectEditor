using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Gui.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	public class ScenarioSaveGameViewModel : BaseLocoFileViewModel
	{
		public ScenarioSaveGameViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) =>
			//_ = this.WhenAnyValue(o => o.CurrentS5File)
			//	.Subscribe(_ => this.RaisePropertyChanged(nameof(RequiredObjects)));

			Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		[Reactive]
		public BindingList<S5Header>? RequiredObjects { get; set; }

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
			RequiredObjects = new BindingList<S5Header>(CurrentS5File!.RequiredObjects);
		}

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
