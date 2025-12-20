using Common;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels.Objects.Sound;
using Gui.Models;
using Gui.Models.Audio;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class SoundEffectsViewModel : BaseFileViewModel
{
	public SoundEffectsViewModel(FileSystemItem currentFile, ObjectEditorModel model)
		: base(currentFile, model)
		=> Load();

	public override void Load()
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(CurrentFile?.FileName, nameof(CurrentFile.FileName));

		var soundIdNames = Enum.GetValues<DatSoundId>();
		AudioViewModels = SawyerStreamReader.LoadSoundEffectsFromCSS(CurrentFile.FileName)
			.Select((x, i) => new AudioViewModel(logger, soundIdNames[i].ToString(), x.header, x.data))
			.ToBindingList();
	}

	[Reactive]
	public BindingList<AudioViewModel> AudioViewModels { get; set; } = [];

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

	public override string? SaveAs(SaveParameters saveParameters)
	{
		var saveFile = Task.Run(async () => await PlatformSpecific.SaveFilePicker(PlatformSpecific.DatFileTypes)).Result;
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
		logger?.Info($"Saving sound effects to {filename}");

		var sfx = AudioViewModels.Select(x => (x.Name, x.GetAsDatWav(LocoAudioType.SoundEffect)));

		var failed = sfx.Where(x => x.Item2 == null).ToList();
		if (failed.Count != 0)
		{
			foreach (var x in failed)
			{
				logger?.Error($"Failed to convert sound effect {x.Name} to the DAT format.");
			}

			logger?.Error($"Failed to convert {failed.Count} sound effects to the DAT format. Cannot save file.");
			return;
		}

		var succeeded = sfx
			.Select(x => x.Item2)
			.Cast<(SoundEffectWaveFormat Header, byte[] Data)>();

		var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS([.. succeeded]);
		File.WriteAllBytes(filename, bytes);
	}
}
