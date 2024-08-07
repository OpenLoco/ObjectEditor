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

namespace AvaGui.ViewModels
{
	public class SoundViewModel : ReactiveObject, IExtraContentViewModel
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

		//public async Task ImportSound()
		//{
		//	var folders = await PlatformSpecific.OpenFolderPicker();
		//	var dir = folders.FirstOrDefault();
		//	if (dir == null)
		//	{
		//		return;
		//	}

		//	var dirPath = dir.Path.LocalPath;
		//	if (Directory.Exists(dirPath) && Directory.EnumerateFiles(dirPath).Any())
		//	{
		//		var files = Directory.GetFiles(dirPath);
		//		var sorted = files.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('-')[0]));

		//		var g1Elements = new List<G1Element32>();
		//		var i = 0;
		//		//foreach (var file in sorted)
		//		//{
		//		//	var img = SixLabors.ImageSharp.Image.Load<Rgb24>(file);
		//		//	var data = PaletteMap.ConvertRgb24ImageToG1Data(img);
		//		//	var hasTransparency = data.Any(b => b == 0);
		//		//	var oldImage = Parent.G1Elements[i++];
		//		//	oldImage.ImageData = PaletteMap.ConvertRgb24ImageToG1Data(img); // simply overwrite existing pixel data
		//		//}
		//	}

		//	//this.RaisePropertyChanged(nameof(Images));
		//}

		//public async Task ExportSound()
		//{
		//	var folders = await PlatformSpecific.OpenFolderPicker();
		//	var dir = folders.FirstOrDefault();
		//	if (dir == null)
		//	{
		//		return;
		//	}

		//	var dirPath = dir.Path.LocalPath;
		//	if (Directory.Exists(dirPath))
		//	{
		//		var counter = 0;
		//		//foreach (var image in Images)
		//		//{
		//		//	var imageName = counter++.ToString(); // todo: use GetImageName from winforms project
		//		//	var path = Path.Combine(dir.Path.LocalPath, $"{imageName}.png");
		//		//	//logger.Debug($"Saving image to {path}");
		//		//	image.Save(path);
		//		//}
		//	}
		//}

		//[Reactive]
		//public ICommand ImportSoundCommand { get; set; }

		//[Reactive]
		//public ICommand ExportSoundCommand { get; set; }
	}
}
