using OpenLoco.Gui.Models;
using System;

namespace OpenLoco.Gui.ViewModels
{
	public class ScenarioViewModel : BaseLocoFileViewModel
	{
		public ScenarioViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading scenario from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
