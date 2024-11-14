using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Gui.Models;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace OpenLoco.Gui.ViewModels
{
	public class MusicViewModel : BaseLocoFileViewModel
	{
		public MusicViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model)
			=> Load();

		public override void Load()
		{
			var (header, data) = SawyerStreamReader.LoadWavFile(CurrentFile.Filename);
			SoundViewModel = new SoundViewModel(
				GetDisplayName(CurrentFile.DisplayName),
				header,
				data);
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
		public SoundViewModel SoundViewModel { get; set; }

		public override void Save()
		{
			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? CurrentFile.Filename
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			Logger?.Info($"Saving music to {savePath}");
			var bytes = SawyerStreamWriter.SaveMusicToDat(SoundViewModel.Header, SoundViewModel.Data);
			File.WriteAllBytes(savePath, bytes);
		}

		public override void SaveAs()
		{
			var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
			if (saveFile == null)
			{
				return;
			}

			var savePath = saveFile.Path.LocalPath;
			Logger?.Info($"Saving music to {savePath}");
			var bytes = SawyerStreamWriter.SaveMusicToDat(SoundViewModel.Header, SoundViewModel.Data);
			File.WriteAllBytes(savePath, bytes);
		}
	}
}
