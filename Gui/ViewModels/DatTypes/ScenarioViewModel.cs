using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types.SCV5;
using OpenLoco.Gui.Models;
using ReactiveUI.Fody.Helpers;
using System;

namespace OpenLoco.Gui.ViewModels
{
	public class ScenarioViewModel : BaseLocoFileViewModel
	{
		public ScenarioViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		[Reactive]
		public S5File? CurrentS5File { get; set; }

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
		}

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
