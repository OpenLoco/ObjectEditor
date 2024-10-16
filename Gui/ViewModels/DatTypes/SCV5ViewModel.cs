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

		public override void Load()
		{
			Logger?.Info($"Loading scenario from {CurrentFile.Filename}");
			CurrentS5File = SawyerStreamReader.LoadSave(CurrentFile.Filename, Model.Logger);
			RequiredObjects = new BindingList<S5Header>(CurrentS5File!.RequiredObjects);
		}

		public override void Save() => Logger?.Error("Saving SC5/SV5 is not implemented yet");

		public override void SaveAs() => Logger?.Error("Saving SC5/SV5 is not implemented yet");
	}
}
