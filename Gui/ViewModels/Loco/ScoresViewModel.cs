using Gui.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class ScoresViewModel : BaseFileViewModel<DummyModel>
{
	public ScoresViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext) => Load();

	public override void Load()
		=> Logger.LogInformation("Loading scores from {FileName}", CurrentFile.FileName);

	public override void Save()
		=> Logger.LogWarning("Save is not currently implemented");

	public override Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		Logger.LogWarning("SaveAs is not currently implemented");
		return Task.FromResult<string?>(null);
	}
}
