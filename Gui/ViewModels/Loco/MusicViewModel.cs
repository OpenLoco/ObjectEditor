using Definitions;
using Dat.Data;
using Dat.FileParsing;
using Gui.Models;
using Gui.Models.Audio;
using Microsoft.Extensions.Logging;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class MusicViewModel : BaseFileViewModel<DummyModel>
{
	[Reactive]
	public AudioViewModel? AudioViewModel { get; set; }

	public MusicViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
		=> Load();

	public override void Load()
		=> AudioViewModel = new AudioViewModel(
			Logger,
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
			: Path.Combine(EditorContext.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

		if (savePath == null)
		{
			return;
		}

		SaveCore(savePath);
	}

	public override async Task<string?> SaveAsAsync(SaveParameters saveParameters)
	{
		var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes);
		var savePath = saveFile?.Path.LocalPath;

		if (savePath == null)
		{
			return null;
		}

		SaveCore(savePath);
		return savePath;
	}

	void SaveCore(string filename)
	{
		Logger.LogInformation("Saving music to {Filename}", filename);

		if (AudioViewModel == null)
		{
			Logger.LogError("AudioViewModel is null, cannot save.");
			return;
		}

		var datWav = AudioViewModel.GetAsDatWav(LocoAudioType.Music);
		if (datWav == null)
		{
			Logger.LogError("Failed to get music data for saving.");
			return;
		}

		var bytes = SawyerStreamWriter.SaveMusicToDat(
			SawyerStreamWriter.LocoWaveFormatToRiff(datWav.Value.Header, datWav.Value.Data.Length), datWav.Value.Data);

		File.WriteAllBytes(filename, bytes);
	}
}
