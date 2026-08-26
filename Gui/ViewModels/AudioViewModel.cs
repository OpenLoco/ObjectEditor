using Definitions.ObjectModels.Objects.Sound;
using Gui.Models.Audio;
using Microsoft.Extensions.Logging;
using Ownaudio.Core;
using Ownaudio.Decoders;
using OwnaudioNET;
using OwnaudioNET.Core;
using OwnaudioNET.Interfaces;
using OwnaudioNET.Mixing;
using OwnaudioNET.Sources;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.ViewModels;

public class AudioViewModel : ReactiveObject, IViewModel, IDisposable
{
	public string DisplayName
		=> "Audio Data";

	ILogger Logger { get; init; }

	// ---- OwnAudioSharp shared engine -------------------------------------------
	private static readonly object s_engineLock = new();
	private static AudioMixer? s_mixer;
	private static int s_engineRate;

	private IAudioSource? _playbackSource;
	private float[] _samples = [];
	private PcmFormat _format = null!;    // always set before any read (samples.Length > 0)

	/// <summary>Exposed for the editor PropertyGrid binding.</summary>
	[Reactive]
	public PcmFormat AudioFormat { get; private set; } = null!;    // always set before any read

	[Reactive]
	public string SoundName { get; init; }

	public string? Duration
	{
		get
		{
			if (_samples.Length == 0) return null;
			var d = _format.DurationFromSampleCount(_samples.Length);
			return $"Duration: {TimeSpan.FromSeconds(d):mm\\:ss\\.ff}";
		}
	}

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
		Logger = logger;
		SoundName = soundName;
		PlaySoundCommand = ReactiveCommand.Create(PlaySound);
		PauseSoundCommand = ReactiveCommand.Create(() => _playbackSource?.Pause());
		StopSoundCommand = ReactiveCommand.Create(() => StopPlayback());
		ImportSoundCommand = ReactiveCommand.Create(ImportSound);
		ExportSoundCommand = ReactiveCommand.Create(ExportSoundAsync);
	}

	public AudioViewModel(ILogger logger, string soundName, string filename)
		: this(logger, soundName)
		=> ImportSoundFromFile(filename);

	public AudioViewModel(ILogger logger, string soundName, SoundEffectWaveFormat locoWaveFormat, byte[] pcmData)
		: this(logger, soundName)
		=> SetPcmData(pcmData, AudioHelpers.SoundEffectFormatToPcmFormat(locoWaveFormat));

	void SetPcmData(byte[] pcmData, PcmFormat format)
	{
		int sampleCount = pcmData.Length / 2;
		float[] samples = new float[sampleCount];
		for (int i = 0; i < sampleCount; i++)
		{
			short s = (short)(pcmData[i * 2] | (pcmData[i * 2 + 1] << 8));
			samples[i] = s / 32768f;
		}

		_samples = samples;
		_format = format;
		AudioFormat = format;
		this.RaisePropertyChanged(nameof(Duration));
	}

	// in future, this method needs to resample the audio to convert to the specific music or sfx format that loco uses
	public (SoundEffectWaveFormat Header, byte[] Data)? GetAsDatWav(LocoAudioType format)
	{
		try
		{
			if (_samples.Length == 0)
				throw new InvalidOperationException("Cannot export: no audio loaded");

			StopPlayback();

			byte[] pcmBytes = new byte[_samples.Length * 2];
			for (int i = 0; i < _samples.Length; i++)
			{
				short s = (short)Math.Clamp(_samples[i] * 32768f, -32768, 32767);
				pcmBytes[i * 2] = (byte)(s & 0xFF);
				pcmBytes[i * 2 + 1] = (byte)((s >> 8) & 0xFF);
			}

			var waveFormat = AudioHelpers.PcmFormatToSoundEffectFormat(_format);
			return (waveFormat, pcmBytes[RiffHeaderSize..]); // skip the wave header
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error while converting audio to Loco format");
			return null;
		}
	}

	const int RiffHeaderSize = 44;

	public void PlaySound()
	{
		if (_samples.Length == 0) return;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			return;

		var src = _playbackSource;
		if (src != null)
		{
			var state = src.State;
			if (state == AudioState.Playing) return;
			if (state == AudioState.Paused) { src.Play(); return; }
		}

		// capture locals for the task
		var samples = _samples;
		var fmt = _format;
		_ = Task.Run(() =>
		{
			if (_playbackSource?.State == AudioState.Stopped)
				Thread.Sleep(100);

			StopPlayback();
			EnsureEngineStarted();

			var mixer = s_mixer;
			if (mixer == null) return;

			// Resample to engine rate if needed
			var playSamples = samples;
			if (fmt.SampleRate != s_engineRate)
				playSamples = Resample(samples, fmt, s_engineRate);

			var cfg = new AudioConfig
			{
				SampleRate = s_engineRate,
				Channels = fmt.Channels,
				BufferSize = 512,
				EnableOutput = true,
				EnableInput = false
			};

			var source = new SampleSource(playSamples, cfg);
			_playbackSource = source;
			mixer.AddSource(source);
			source.Play();

			Logger.LogInformation("Playing: src={SrcRate}Hz {SrcCh}ch engine={EngRate}Hz {Samples} samples",
				fmt.SampleRate, fmt.Channels, s_engineRate, samples.Length);

			while (source.State != AudioState.Stopped && source.State != AudioState.Error)
			{
				if (_playbackSource != source) break;
				Thread.Sleep(100);
			}

			if (_playbackSource == source)
			{
				mixer.RemoveSource(source);
				_playbackSource = null;
			}
		});

	}

	void StopPlayback()
	{
		var src = _playbackSource;
		if (src == null) return;

		src.Stop();
		s_mixer?.RemoveSource(src);
		src.Dispose();
		_playbackSource = null;
	}

	public async Task ImportSound()
	{
		var fsi = await MainWindowViewModel.GetFileSystemItemFromUser(PlatformSpecific.AudioFileImportTypes);
		if (fsi?.FileName == null) return;
		ImportSoundFromFile(fsi.FileName);
	}

	void ImportSoundFromFile(string filename)
	{
		StopPlayback();
		try
		{
			_samples = [];
			this.RaisePropertyChanged(nameof(Duration));

			Logger.LogInformation("Loading {Filename}", filename);

			var decoder = AudioDecoderFactory.Create(filename, targetSampleRate: 0, targetChannels: 0);
			try
			{
				var info = decoder.StreamInfo;
				var list = new System.Collections.Generic.List<float>();
				byte[] buffer = new byte[4096 * 8];
				while (true)
				{
					var result = decoder.ReadFrames(buffer);
					if (result.FramesRead == 0) break;
					int total = result.FramesRead * info.Channels;
					for (int i = 0; i < total; i++)
					{
						short s = (short)(buffer[i * 2] | (buffer[i * 2 + 1] << 8));
						list.Add(s / 32768f);
					}
					if (result.IsEOF) break;
				}

				var samples = list.ToArray();
				if (samples.Length == 0)
				{
					Logger.LogError("Audio file produced no samples: {Filename}", filename);
					return;
				}

				_samples = samples;
				_format = new PcmFormat(info.SampleRate, info.Channels,
					BitsPerSample: (int)info.BitDepth);
				AudioFormat = _format;
			}
			finally
			{
				decoder.Dispose();
			}

			this.RaisePropertyChanged(nameof(Duration));
			Logger.LogInformation("Successfully loaded {Filename} ({Samples} samples, {SampleRate}Hz {Channels}ch)",
				filename, _samples.Length, _format.SampleRate, _format.Channels);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Failed to load audio file \"{Filename}\".", filename);
			_samples = [];
			this.RaisePropertyChanged(nameof(Duration));
		}
	}

	public async Task ExportSoundAsync()
	{
		if (_samples.Length == 0) return;

		var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.AudioFileExportTypes);
		if (saveFile?.Path != null)
		{
			using var writer = new WaveFileWriter(saveFile.Path.LocalPath,
				new AudioConfig
				{
					SampleRate = _format.SampleRate,
					Channels = _format.Channels,
					BufferSize = 512,
					EnableOutput = true,
					EnableInput = false
				});
			writer.WriteSamples(new ReadOnlySpan<float>(_samples));
		}
	}

	// ---- helpers --------------------------------------------------------------

	static float[] Resample(float[] src, PcmFormat srcFmt, int dstRate)
	{
		int ch = srcFmt.Channels;
		int srcRate = srcFmt.SampleRate;
		int srcFrames = src.Length / ch;
		int dstFrames = (int)((long)srcFrames * dstRate / srcRate);
		float[] dst = new float[dstFrames * ch];
		for (int chIdx = 0; chIdx < ch; chIdx++)
		{
			for (int df = 0; df < dstFrames; df++)
			{
				double srcPos = df * (double)srcRate / dstRate;
				int si = (int)srcPos;
				float frac = (float)(srcPos - si);
				int s0 = Math.Min(si, srcFrames - 1) * ch + chIdx;
				int s1 = Math.Min(si + 1, srcFrames - 1) * ch + chIdx;
				dst[df * ch + chIdx] = src[s0] + (src[s1] - src[s0]) * frac;
			}
		}
		return dst;
	}

	static void EnsureEngineStarted()
	{
		lock (s_engineLock)
		{
			if (s_mixer != null) return;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

			try
			{
				var config = OwnaudioNet.CreateDefaultConfig(); // 48 kHz stereo
				OwnaudioNet.Initialize(config);
				OwnaudioNet.Start();
				s_engineRate = config.SampleRate;

				s_mixer = new AudioMixer(OwnaudioNet.Engine!.UnderlyingEngine);
				s_mixer.Start();
			}
			catch
			{
				// Engine init can fail if no audio device
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				StopPlayback();
			}

			_playbackSource = null;
			_samples = [];
			disposed = true;
		}
	}
}
