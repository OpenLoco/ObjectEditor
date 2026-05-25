using Gui.Models;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ScoresViewModel : BaseFileViewModel<DummyModel>
{
	public ScoresViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext) => Load();

	public override void Load()
		=> logger?.Info($"Loading scores from {CurrentFile.FileName}");

	public override void Save()
		=> logger?.Warning("Save is not currently implemented");

	public override Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		logger?.Warning("SaveAs is not currently implemented");
		return Task.FromResult<string?>(null);
	}
}
