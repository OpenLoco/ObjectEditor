using ReactiveUI;
using OpenLoco.ObjectEditor.DatFileParsing;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Objects.Sound;
using OpenLoco.ObjectEditor.Data;
using NAudio.Wave;
using System.Threading;
using System.Runtime.InteropServices;
using AvaGui.Models;
using System;

namespace AvaGui.ViewModels
{
	public class SoundViewModel : ReactiveObject, IExtraContentViewModel, IDisposable
	{
		ILocoObject parent; // currently not needed

		WaveOutEvent? CurrentWOEvent { get; set; }

		public SoundViewModel(ILocoObject parent)
		{
			if (parent.Object is not SoundObject soundObject)
			{
				return;
			}

			this.parent = parent;

			var hdr = soundObject.SoundObjectData.PcmHeader;
			var text = parent.StringTable.Table["Name"][LanguageId.English_UK] ?? "<null>";
			Sound = new UiSoundObject(text, SawyerStreamWriter.WaveFormatExToRiff(hdr, soundObject.PcmData.Length), soundObject.PcmData);

			PlaySoundCommand = ReactiveCommand.Create(PlaySound);
			PauseSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Pause());
			StopSoundCommand = ReactiveCommand.Create(() => CurrentWOEvent?.Stop());

			//ImportSoundCommand = ReactiveCommand.Create(ImportSound);
			//ExportSoundCommand = ReactiveCommand.Create(ExportSound);
		}

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

				using (var ms = new MemoryStream(Sound.Data))
				using (var rs = new RawSourceWaveStream(ms, new WaveFormat((int)Sound.Header.SampleRate, Sound.Header.BitsPerSample, Sound.Header.NumberOfChannels)))
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

		[Reactive]
		public UiSoundObject Sound { get; set; }

		[Reactive]
		public ICommand PlaySoundCommand { get; set; }

		[Reactive]
		public ICommand PauseSoundCommand { get; set; }

		[Reactive]
		public ICommand StopSoundCommand { get; set; }

		public void Dispose()
		{
			CurrentWOEvent?.Stop();
		}
	}
}
