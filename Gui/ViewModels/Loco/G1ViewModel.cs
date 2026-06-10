using Dat.FileParsing;
using Dat.Types;
using Gui.Models;
using Gui.ViewModels.Graphics;
using Microsoft.Extensions.Logging;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class G1ViewModel : BaseFileViewModel<G1Dat>
{
	public G1ViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
		=> _ = LoadAsync();

	[Reactive]
	public ImageTableViewModel? ImageTableViewModel { get; set; }

	public override void Load()
	{
		Logger.LogInformation("Loading G1 from {FileName}", CurrentFile.FileName);
		if (CurrentFile.FileName == null)
		{
			Logger.LogError("G1 file name was null");
			return;
		}

		Model = SawyerStreamReader.LoadG1(CurrentFile.FileName, EditorContext.Logger)!;

		if (Model == null)
		{
			Logger.LogError("G1 was unable to be loaded from {FileName}", CurrentFile.FileName);
			return;
		}

		Model.ImageTable.PaletteMap = EditorContext.PaletteMap;
		EditorContext.G1 = Model; // todo: do we still need? can we do another way?
		ImageTableViewModel = new ImageTableViewModel(Model.ImageTable, Logger) { OperationQueue = EditorContext.OperationQueue };
	}

	public override void Save()
	{
		if (Model == null)
		{
			Logger.LogError("G1 was null and was unable to saved");
			return;
		}

		//Model.G1.ImageTable.GraphicsElements = [.. ImageTableViewModel.ImageViewModels.Select(x => x.ToGraphicsElement())];

		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? Path.Combine(EditorContext.Settings.ObjDataDirectory, CurrentFile.FileName ?? string.Empty)
			: Path.Combine(EditorContext.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName ?? string.Empty, ".dat"));

		Logger.LogInformation("Saving G1.dat to {SavePath}", savePath);
		SawyerStreamWriter.SaveG1(savePath, Model);
	}

	public override async Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		if (Model == null)
		{
			Logger.LogError("G1 was null and was unable to saved");
			return null;
		}

		var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes);
		if (saveFile == null)
		{
			return null;
		}

		var savePath = saveFile.Path.LocalPath;
		Logger.LogInformation("Saving G1.dat to {SavePath}", savePath);
		SawyerStreamWriter.SaveG1(savePath, Model);

		return savePath;
	}
}
