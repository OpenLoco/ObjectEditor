using Gui.Models;

namespace Gui.ViewModels;

public class ScoresViewModel : BaseFileViewModel<DummyModel>
{
	public ScoresViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext) => Load();

	public override void Load()
		=> logger?.Info($"Loading scores from {CurrentFile.FileName}");

	public override void Save()
		=> logger?.Warning("Save is not currently implemented");

	public override string? SaveAs(SaveParameters saveParameters)
	{
		logger?.Warning("SaveAs is not currently implemented");
		return null;
	}
}
