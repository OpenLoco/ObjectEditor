using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels.Objects.Sound;
using Gui.Models;
using Gui.Models.Audio;
using Microsoft.Extensions.Logging;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class SoundEffectsViewModel : BaseFileViewModel<DummyModel>
{
	public SoundEffectsViewModel(FileSystemItem currentFile, ObjectEditorContext editorContext)
		: base(currentFile, editorContext)
		=> Load();

	public override void Load()
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(CurrentFile?.FileName, nameof(CurrentFile.FileName));

		var soundIdNames = Enum.GetValues<DatSoundId>();
		AudioViewModels = SawyerStreamReader.LoadSoundEffectsFromCSS(CurrentFile.FileName)
			.Select((x, i) => new AudioViewModel(Logger, soundIdNames[i].ToString(), x.header, x.data))
			.ToBindingList();
	}

	[Reactive]
	public BindingList<AudioViewModel> AudioViewModels { get; set; } = [];

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
		Logger.LogInformation("Saving sound effects to {Filename}", filename);

		var sfx = AudioViewModels.Select(x => (x.DisplayName, x.GetAsDatWav(LocoAudioType.SoundEffect)));

		var failed = sfx.Where(x => x.Item2 == null).ToList();
		if (failed.Count != 0)
		{
			foreach (var x in failed)
			{
				Logger.LogError("Failed to convert sound effect {DisplayName} to the DAT format.", x.DisplayName);
			}

			Logger.LogError("Failed to convert {Count} sound effects to the DAT format. Cannot save file.", failed.Count);
			return;
		}

		var succeeded = sfx
			.Select(x => x.Item2)
			.Cast<(SoundEffectWaveFormat Header, byte[] Data)>();

		var bytes = SawyerStreamWriter.SaveSoundEffectsToCSS([.. succeeded]);
		File.WriteAllBytes(filename, bytes);
	}
}
