using NAudio.Wave;
using Dat.FileParsing;
using Dat.Objects;
using Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Logging;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public class AudioViewModel : ReactiveObject, IExtraContentViewModel, IDisposable
{
	public string Name => "Sound Name";

	ILogger Logger { get; init; }

	WaveOutEvent? CurrentWOEvent { get; set; }

	[Reactive]
	WaveStream? WaveStream { get; set; }

	[Reactive]
	public string SoundName { get; init; }

	[Reactive, Editable(false)]
	public string Duration { get; set; }

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

	WaveFormat LocoWaveFormatToWaveFormat(LocoWaveFormat locoWaveFormat)
		=> WaveFormat.CreateCustomFormat(
			(WaveFormatEncoding)locoWaveFormat.WaveFormatTag,
			locoWaveFormat.SampleRate,
			locoWaveFormat.Channels,
			locoWaveFormat.AverageBytesPerSecond,
			2, //locoWaveFormat.BlockAlign,
			16); //locoWaveFormat.BitsPerSample);

	LocoWaveFormat WaveFormatToLocoWaveFormat(WaveFormat waveFormat)
		=> new()
		{
			WaveFormatTag = (int16_t)waveFormat.Encoding,
			Channels = (int16_t)waveFormat.Channels,
			SampleRate = (int32_t)waveFormat.SampleRate,
			AverageBytesPerSecond = (int32_t)waveFormat.AverageBytesPerSecond,
			BlockAlign = (int16_t)waveFormat.BlockAlign,
			BitsPerSample = (int16_t)waveFormat.BitsPerSample,
			ExtraSize = (int16_t)waveFormat.ExtraSize
		};

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

	public AudioViewModel(ILogger logger, string soundName, LocoWaveFormat locoWaveFormat, byte[] pcmData)
		: this(logger, soundName)
		=> WaveStream = new RawSourceWaveStream(
			new MemoryStream(pcmData),
			LocoWaveFormatToWaveFormat(locoWaveFormat));

	public (LocoWaveFormat Header, byte[] Data) GetAsDatWav()
	{
		if (WaveStream == null)
		{
			throw new NullReferenceException(nameof(WaveStream));
		}

		WaveStream.Position = 0;
		using var ms = new MemoryStream();
		WaveStream?.CopyTo(ms);
		var bytes = ms.ToArray();
		var waveFormat = WaveFormatToLocoWaveFormat(WaveStream.WaveFormat);
		return (waveFormat, bytes[44..]); // skip the wave header
	}

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

		// do it asyncly to a) give user ui control and b) allow multiple sounds to play at once
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

					Thread.Sleep(50);
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

	public void ImportSoundFromFile(string filename)
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
			else if (extension == ".wav" || extension == ".dat")
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
		catch (Exception ex)
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
