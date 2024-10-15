using OpenLoco.Gui.Models;
using System;

namespace OpenLoco.Gui.ViewModels
{
	public class SaveGameViewModel : BaseLocoFileViewModel
	{
		public SaveGameViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading save-game from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
