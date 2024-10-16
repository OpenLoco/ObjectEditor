using OpenLoco.Gui.Models;
using System;

namespace OpenLoco.Gui.ViewModels
{
	public class TutorialViewModel : BaseLocoFileViewModel
	{
		public TutorialViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading tutorial from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
