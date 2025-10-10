using Common;
using Dat.Data;
using Dat.FileParsing;
using Gui.Models;
using Gui.Models.Audio;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class MusicViewModel : BaseLocoFileViewModel
{
	[Reactive]
	public AudioViewModel AudioViewModel { get; set; }

	public MusicViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model)
		=> Load();

	public override void Load()
		=> AudioViewModel = new AudioViewModel(
			logger,
			GetDisplayName(CurrentFile.DisplayName),
			CurrentFile.FileName!);

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

	public override void Save()
	{
		var savePath = CurrentFile.FileLocation == FileLocation.Local
			? CurrentFile.FileName
			: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

		if (savePath == null)
		{
			return;
		}

		SaveCore(savePath);
	}

	public override void SaveAs()
	{
		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
		var savePath = saveFile?.Path.LocalPath;

		if (savePath == null)
		{
			return;
		}

		SaveCore(savePath);
	}

	void SaveCore(string filename)
	{
		logger?.Info($"Saving music to {filename}");

		var datWav = AudioViewModel.GetAsDatWav(LocoAudioType.Music);
		if (datWav == null)
		{
			logger?.Error("Failed to get music data for saving.");
			return;
		}

		var bytes = SawyerStreamWriter.SaveMusicToDat(
			SawyerStreamWriter.LocoWaveFormatToRiff(datWav.Value.Header, datWav.Value.Data.Length), datWav.Value.Data);

		File.WriteAllBytes(filename, bytes);
	}
}
