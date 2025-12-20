using Gui.Models;

namespace Gui.ViewModels;

public class LanguageViewModel : BaseFileViewModel
{
	public LanguageViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model) => Load();

	public override void Load()
		=> logger?.Info($"Loading languages from {CurrentFile.FileName}");

	public override void Save()
		=> logger?.Warning("Save is not currently implemented");

	public override string? SaveAs(SaveParameters saveParameters)
	{
		logger?.Warning("SaveAs is not currently implemented");
		return null;
	}
}
