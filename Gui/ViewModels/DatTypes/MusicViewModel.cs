using Dat.Data;
using Gui.Models;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class MusicViewModel : BaseLocoFileViewModel
{
	public MusicViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model)
		=> Load();

	public override void Load()
	{
		//var (header, data) = SawyerStreamReader.LoadWavFile(CurrentFile.FileName);
		AudioViewModel = new AudioViewModel(
			logger,
			GetDisplayName(CurrentFile.DisplayName),
			CurrentFile.FileName);
	}

	static string GetDisplayName(string filename)
	{
		if (OriginalDataFiles.Music.TryGetValue(filename, out var musicName))
		{
			return $"{musicName} ({filename})";
		}

		if (OriginalDataFiles.MiscellaneousTracks.TryGetValue(filename, out var miscTrackName))
		{
			return $"{miscTrackName} ({filename})";
		}

		return filename;
	}

	[Reactive]
	public AudioViewModel AudioViewModel { get; set; }

	public override void Save()
	{
		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? CurrentFile.FileName
			: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

		logger?.Info($"Saving music to {savePath}");
		//var bytes = SawyerStreamWriter.SaveMusicToDat(AudioViewModel.GetAsDatWav());
		//File.WriteAllBytes(savePath, bytes);
	}

	public override void SaveAs()
	{
		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
		if (saveFile == null)
		{
			return;
		}

		var savePath = saveFile.Path.LocalPath;
		logger?.Info($"Saving music to {savePath}");
		//var bytes = SawyerStreamWriter.SaveMusicToDat(AudioViewModel.GetAsDatWav());
		//File.WriteAllBytes(savePath, bytes);
	}
}
