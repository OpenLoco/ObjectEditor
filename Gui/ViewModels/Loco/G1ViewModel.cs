using Dat.FileParsing;
using Gui.Models;
using Gui.ViewModels.Graphics;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class G1ViewModel : BaseFileViewModel
{
	public G1ViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext) => Load();

	[Reactive]
	public ImageTableViewModel ImageTableViewModel { get; set; }

	public override void Load()
	{
		logger.Info($"Loading G1 from {CurrentFile.FileName}");
		EditorContext.G1 = SawyerStreamReader.LoadG1(CurrentFile.FileName, EditorContext.Logger);

		if (EditorContext.G1 == null)
		{
			logger.Error($"G1 was unable to be loaded from {CurrentFile.FileName}");
			return;
		}

		EditorContext.G1.ImageTable.PaletteMap = EditorContext.PaletteMap;
		ImageTableViewModel = new ImageTableViewModel(EditorContext.G1.ImageTable, logger);
	}

	public override void Save()
	{
		if (EditorContext.G1 == null)
		{
			logger?.Error("G1 was null and was unable to saved");
			return;
		}

		//Model.G1.ImageTable.GraphicsElements = [.. ImageTableViewModel.ImageViewModels.Select(x => x.ToGraphicsElement())];

		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? Path.Combine(EditorContext.Settings.ObjDataDirectory, CurrentFile.FileName)
			: Path.Combine(EditorContext.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

		logger?.Info($"Saving G1.dat to {savePath}");
		SawyerStreamWriter.SaveG1(savePath, EditorContext.G1);
	}

	public override string? SaveAs(SaveParameters saveParameters)
	{
		if (EditorContext.G1 == null)
		{
			logger?.Error("G1 was null and was unable to saved");
			return null;
		}

		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
		if (saveFile == null)
		{
			return null;
		}

		var savePath = saveFile.Path.LocalPath;
		logger?.Info($"Saving G1.dat to {savePath}");
		SawyerStreamWriter.SaveG1(savePath, EditorContext.G1);

		return savePath;
	}
}
