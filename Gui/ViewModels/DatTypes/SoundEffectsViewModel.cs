using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Gui.Models;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoco.Gui.ViewModels
{
	public class SoundEffectsViewModel : BaseLocoFileViewModel
	{
		public SoundEffectsViewModel(FileSystemItem currentFile, ObjectEditorModel model)
			: base(currentFile, model) => Load();

		public override void Load()
		{
			var soundIdNames = Enum.GetValues<SoundId>();
			SoundViewModels = SawyerStreamReader.LoadSoundEffectsFromCSS(CurrentFile.Filename)
				.Select((x, i) => new SoundViewModel(soundIdNames[i].ToString(), x.header, x.data))
				.ToBindingList();
		}

		[Reactive]
		public BindingList<SoundViewModel> SoundViewModels { get; set; } = [];

		public override void Save()
		{
			var savePath = CurrentFile.FileLocation == FileLocation.Local
				? CurrentFile.Filename
				: Path.Combine(Model.Settings.DownloadFolder, Path.ChangeExtension(CurrentFile.DisplayName, ".dat"));

			logger?.Info($"Saving sound effects to {savePath}");
			var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS(SoundViewModels.Select(x => (x.Header, x.Data)).ToList());
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
			logger?.Info($"Saving sound effects to {savePath}");
			var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS(SoundViewModels.Select(x => (x.Header, x.Data)).ToList());
			File.WriteAllBytes(savePath, bytes);
		}
	}
}
