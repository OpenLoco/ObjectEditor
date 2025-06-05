using Dat.Objects;
using NAudio.Wave;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenLoco.Gui.ViewModels
{
	public class AudioViewModel : ReactiveObject, IExtraContentViewModel, IDisposable
	{
		public string Name => "Sound Table";

		WaveOutEvent? CurrentWOEvent { get; set; }

		public AudioViewModel(string soundName, WaveFormatEx pcmHeader, byte[] pcmData)
			: this(soundName, SawyerStreamWriter.WaveFormatExToRiff(pcmHeader, pcmData.Length), pcmData) { }

		public AudioViewModel(string soundName, RiffWavHeader riffHeader, byte[] pcmData)
		{
			SoundName = soundName;
			Header = riffHeader;
			Data = pcmData;
			Duration = $"{CalculateDuration():0.##}s";

			PlaySoundCommand = ReactiveCommand.Create(PlaySound);
			PauseSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Pause());
			StopSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Stop());
			ImportSoundCommand = ReactiveCommand.Create(ImportSound);
			ExportSoundCommand = ReactiveCommand.Create(ExportSoundAsync);
		}

		decimal CalculateDuration()
			=> Data.Length / (decimal)Header.BytesPerSecond;

		public void PlaySound()
		{
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

				using (var ms = new MemoryStream(Data))
				using (var rs = new RawSourceWaveStream(ms, new WaveFormat((int)Header.SampleRate, Header.BitsPerSample, Header.NumberOfChannels)))
				using (CurrentWOEvent = new WaveOutEvent())
				{
					CurrentWOEvent.Init(rs);
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
			var fsi = await MainWindowViewModel.GetFileSystemItemFromUser(PlatformSpecific.WavFileTypes);
			if (fsi == null)
			{
				return;
			}

			var (header, pcmData) = SawyerStreamReader.LoadWavFile(fsi.Filename);
			Header = header;
			Data = pcmData;
			Duration = $"{CalculateDuration():0.##}s";
		}

		public async Task ExportSoundAsync()
		{
			var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.WavFileTypes);
			if (saveFile?.Path != null)
			{
				SawyerStreamWriter.ExportMusicAsWave(saveFile.Path.LocalPath, Header, Data);
			}
		}
		[Reactive]
		public string SoundName { get; init; }

		[Reactive, Editable(false)]
		public string Duration { get; set; }

		[Reactive, Editable(false)]
		public RiffWavHeader Header { get; set; }

		[Reactive, Editable(false)]
		public byte[] Data { get; set; }

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
}
