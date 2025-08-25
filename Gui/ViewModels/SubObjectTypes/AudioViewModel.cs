using Common.Logging;
using Definitions.ObjectModels.Objects.Sound;
using Gui.Models.Audio;
using NAudio.Wave;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.ViewModels;

public class AudioViewModel : ReactiveObject, IExtraContentViewModel, IDisposable
{
	public string Name => "Audio Data";

	ILogger Logger { get; init; }

	WaveOutEvent? CurrentWOEvent { get; set; }

	[Reactive]
	WaveStream? WaveStream { get; set; }

	[Reactive]
	public string SoundName { get; init; }

	public string? Duration => $"Duration: {WaveStream?.TotalTime.ToString(@"mm\:ss\.ff")}";

	[Reactive]
	public ICommand PlaySoundCommand { get; set; }

	[Reactive]
	public ICommand PauseSoundCommand { get; set; }

	[Reactive]
	public ICommand StopSoundCommand { get; set; }

	[Reactive]
	public ICommand ImportSoundCommand { get; set; }

	[Reactive]
	public ICommand ExportSoundCommand { get; set; }

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	bool disposed;

	public AudioViewModel(ILogger logger, string soundName)
	{
		_ = this.WhenAnyValue(o => o.WaveStream)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(Duration)));

		Logger = logger;
		SoundName = soundName;
		PlaySoundCommand = ReactiveCommand.Create(PlaySound);
		PauseSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Pause());
		StopSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Stop());
		ImportSoundCommand = ReactiveCommand.Create(ImportSound);
		ExportSoundCommand = ReactiveCommand.Create(ExportSoundAsync);
	}

	public AudioViewModel(ILogger logger, string soundName, string filename)
		: this(logger, soundName)
		=> ImportSoundFromFile(filename);

	public AudioViewModel(ILogger logger, string soundName, SoundEffectWaveFormat locoWaveFormat, byte[] pcmData)
		: this(logger, soundName)
		=> WaveStream = new RawSourceWaveStream(
			new MemoryStream(pcmData),
			AudioHelpers.SoundEffectFormatToWaveFormat(locoWaveFormat));

	// in future, this method needs to resample the audio to convert to the specific music or sfx format that loco uses
	public (SoundEffectWaveFormat Header, byte[] Data)? GetAsDatWav(LocoAudioType format)
	{
		if (WaveStream == null)
		{
			throw new InvalidOperationException("Cannot export a null WaveStream");
		}

		try
		{
			CurrentWOEvent?.Stop();
			WaveStream.Position = 0;

			var outFormat = AudioHelpers.LocoAudioTypeToWaveFormat[format];
			using var ms = new MemoryStream();
			WaveStream.CopyTo(ms);
			var bytes = ms.ToArray();
			var waveFormat = AudioHelpers.WaveFormatToSoundEffectFormat(WaveStream.WaveFormat);
			return (waveFormat, bytes[RiffHeaderSize..]); // skip the wave header

		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error while converting audio to Loco format");
			return null;
		}
	}

	const int RiffHeaderSize = 44;

	public void PlaySound()
	{
		if (WaveStream == null)
		{
			return;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			// unfortunately NAudio is not cross-platform! this code just crashes on Linux
			// so there isn't anything to do here until a cross-platform audio lib is used
			return;
		}

		if (CurrentWOEvent != null)
		{
			if (CurrentWOEvent.PlaybackState == PlaybackState.Playing)
			{
				return;
			}

			if (CurrentWOEvent.PlaybackState == PlaybackState.Paused)
			{
				CurrentWOEvent.Play();
				return;
			}
		}

		// do it async to a) give user ui control and b) allow multiple sounds to play at once
		_ = Task.Run(() =>
		{
			if (CurrentWOEvent?.PlaybackState == PlaybackState.Stopped)
			{
				Thread.Sleep(100); // give time to wait until previous sound is disposed
			}

			CurrentWOEvent?.Dispose();

			using (CurrentWOEvent = new WaveOutEvent())
			{
				Logger.Info($"{WaveStream.WaveFormat}");
				WaveStream.Position = 0;
				CurrentWOEvent.Init(WaveStream);
				CurrentWOEvent.Play();

				// need to wait for music to finish
				while (CurrentWOEvent?.PlaybackState != PlaybackState.Stopped)
				{
					if (CurrentWOEvent == null)
					{
						break;
					}

					Thread.Sleep(100);
				}
			}

			CurrentWOEvent = null;
		});
	}

	public async Task ImportSound()
	{
		var fsi = await MainWindowViewModel.GetFileSystemItemFromUser(PlatformSpecific.AudioFileImportTypes);
		if (fsi?.FileName == null)
		{
			return;
		}

		ImportSoundFromFile(fsi.FileName);
	}

	void ImportSoundFromFile(string filename)
	{
		// stop currently playing file
		CurrentWOEvent?.Stop();
		CurrentWOEvent = null;
		try
		{
			WaveStream?.Dispose();
			WaveStream = null;

			Logger.Info($"Loading {filename}");
			var extension = Path.GetExtension(filename).ToLower();

			if (extension == ".mp3")
			{
				WaveStream = new Mp3FileReader(filename);
			}
			else if (extension is ".wav" or ".dat")
			{
				WaveStream = new WaveFileReader(filename);
			}

			if (WaveStream == null)
			{
				Logger.Error($"Unsupported audio file format: {extension}");
				return;
			}

			WaveStream.Position = 0;
			Logger.Info($"Successfully loaded {filename}");
		}
		catch (Exception)
		{
			WaveStream?.Dispose();
			WaveStream = null;
		}
	}

	public async Task ExportSoundAsync()
	{
		if (WaveStream == null)
		{
			return;
		}

		var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.AudioFileExportTypes);
		if (saveFile?.Path != null)
		{
			WaveStream.Position = 0;
			WaveFileWriter.CreateWaveFile(saveFile.Path.LocalPath, WaveStream);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		// Check to see if Dispose has already been called.
		if (!disposed)
		{
			// If disposing equals true, dispose all managed
			// and unmanaged resources.
			if (disposing)
			{
				// Dispose managed resources.
				CurrentWOEvent?.Stop();
			}

			CurrentWOEvent = null;

			// Note disposing has been done.
			disposed = true;
		}
	}
}
