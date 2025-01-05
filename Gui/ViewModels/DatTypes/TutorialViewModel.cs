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

	public class ScoresViewModel : BaseLocoFileViewModel
	{
		public ScoresViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading scores from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}

	public class LanguageViewModel : BaseLocoFileViewModel
	{
		public LanguageViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load() => Logger?.Info($"Loading languages from {CurrentFile.Filename}");

		public override void Save() => throw new NotImplementedException();

		public override void SaveAs() => throw new NotImplementedException();
	}
}
