using ReactiveUI;
using OpenLoco.ObjectEditor.DatFileParsing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor;
using SixLabors.ImageSharp.PixelFormats;
using Avalonia;
using Avalonia.Platform;
using System.Reactive.Linq;
using System;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using OpenLoco.ObjectEditor.Types;
using System.ComponentModel;
using Core.Objects.Sound;
using System.Collections.ObjectModel;
using OpenLoco.ObjectEditor.Data;
using NAudio.Wave;
using System.Threading;

namespace AvaGui.ViewModels
{
	public interface IExtraContentViewModel
	{ }

	public record UiSoundObject(string SoundName)
	{
		public UiSoundObject(string soundName, RiffWavHeader header, byte[] data) : this(soundName)
		{
			Header = header;
			Data = data;
			Duration = $"{data.Length / (decimal)header.ByteRate:0.#}s";
		}

		public string Duration { get; init; }

		public RiffWavHeader Header { get; set; }
		public byte[] Data { get; set; }
	}

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

	public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel
	{
		ILocoObject Parent;

		public ImageTableViewModel(ILocoObject parent, PaletteMap paletteMap)
		{
			Parent = parent;
			PaletteMap = paletteMap;

			_ = this.WhenAnyValue(o => o.Parent)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.PaletteMap)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.Zoom)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));

			ImportImagesCommand = ReactiveCommand.Create(ImportImages);
			ExportImagesCommand = ReactiveCommand.Create(ExportImages);
		}

		public async Task ImportImages()
		{
			var folders = await PlatformSpecific.OpenFolderPicker();
			var dir = folders.FirstOrDefault();
			if (dir == null)
			{
				return;
			}

			var dirPath = dir.Path.LocalPath;
			if (Directory.Exists(dirPath) && Directory.EnumerateFiles(dirPath).Any())
			{
				var files = Directory.GetFiles(dirPath);
				var sorted = files.OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('-')[0]));

				var g1Elements = new List<G1Element32>();
				var i = 0;
				foreach (var file in sorted)
				{
					var img = SixLabors.ImageSharp.Image.Load<Rgb24>(file);
					var data = PaletteMap.ConvertRgb24ImageToG1Data(img);
					var hasTransparency = data.Any(b => b == 0);
					var oldImage = Parent.G1Elements[i++];
					oldImage.ImageData = PaletteMap.ConvertRgb24ImageToG1Data(img); // simply overwrite existing pixel data
				}
			}

			this.RaisePropertyChanged(nameof(Images));
		}

		public async Task ExportImages()
		{
			var folders = await PlatformSpecific.OpenFolderPicker();
			var dir = folders.FirstOrDefault();
			if (dir == null)
			{
				return;
			}

			var dirPath = dir.Path.LocalPath;
			if (Directory.Exists(dirPath))
			{
				var counter = 0;
				foreach (var image in Images)
				{
					var imageName = counter++.ToString(); // todo: use GetImageName from winforms project
					var path = Path.Combine(dir.Path.LocalPath, $"{imageName}.png");
					//logger.Debug($"Saving image to {path}");
					image.Save(path);
				}
			}
		}

		[Reactive]
		public PaletteMap PaletteMap { get; set; }

		[Reactive]
		public ICommand ImportImagesCommand { get; set; }

		[Reactive]
		public ICommand ExportImagesCommand { get; set; }

		[Reactive]
		public int Zoom { get; set; } = 1;

		// public Bitmap FirstImage => Images.FirstOrDefault();

		public List<Bitmap> Images
		{
			get
			{
				images = CreateImages(Parent.G1Elements, PaletteMap, Zoom).ToList();
				return images;
			}
			set
			{
				images = value;
				_ = this.RaiseAndSetIfChanged(ref images, value);
			}
		}
		List<Bitmap> images;

		public static IEnumerable<Bitmap> CreateImages(IEnumerable<G1Element32> g1Elements, PaletteMap paletteMap, int zoom)
		{
			foreach (var g1Element in g1Elements)
			{
				if (g1Element.ImageData.Length == 0)
				{
					//logger?.Info($"skipped loading g1 element {i} with 0 length");
					continue;
				}

				if (g1Element.Flags.HasFlag(G1ElementFlags.IsR8G8B8Palette))
				{
					yield return G1RGBToBitmap(g1Element, zoom);
				}
				else
				{
					yield return G1IndexedToBitmap(g1Element, paletteMap, true, zoom);
				}
			}
		}

		static Bitmap G1RGBToBitmap(G1Element32 g1Element, int zoom = 1)
		{
			var imageData = g1Element.ImageData;
			var writeableBitmap = new WriteableBitmap(
				new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom),
				new Vector(96, 96),  // DPI
				PixelFormat.Rgba8888); // Or a suitable pixel format

			using (var lockedBitmap = writeableBitmap.Lock())
			{
				unsafe
				{
					var pointer = (uint*)lockedBitmap.Address; // Access pixel data directly

					for (var y = 0; y < g1Element.Height; y++)
					{
						for (var x = 0; x < g1Element.Width; x++)
						{
							// Calculate pixel index
							var index = x + (y * g1Element.Width);

							// Set pixel color (example: red)
							pointer[index] = 0xFFFF0000;
						}
					}
				}
			}

			return writeableBitmap;
		}

		static Bitmap G1IndexedToBitmap(G1Element32 g1Element, PaletteMap paletteMap, bool useTransparency = false, int zoom = 1)
		{
			var writeableBitmap = new WriteableBitmap(
				new PixelSize(g1Element.Width, g1Element.Height),
				new Vector(96, 96),  // DPI
				PixelFormat.Rgba8888); // Or a suitable pixel format

			using (var lockedBitmap = writeableBitmap.Lock())
			{
				unsafe
				{
					var ptr = (byte*)lockedBitmap.Address; // Access pixel data directly

					for (var y = 0; y < g1Element.Height; y++)
					{
						for (var x = 0; x < g1Element.Width; x++)
						{
							var index = x + (y * g1Element.Width);
							var paletteIndex = g1Element.ImageData[index];

							if (paletteIndex == 0 && useTransparency)
							{
								ptr += 4;
							}
							else
							{
								var colour = paletteMap.Palette[paletteIndex].Color;
								var pixel = colour.ToPixel<Rgb24>();

								//var ptr = (byte*)pointer;
								*ptr++ = pixel.R;
								*ptr++ = pixel.G;
								*ptr++ = pixel.B;
								*ptr++ = 255;
							}
						}
					}
				}
			}

			// bug in avalonia/skiasharp/skia: https://github.com/AvaloniaUI/Avalonia/issues/8444
			//return writeableBitmap.CreateScaledBitmap(new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom), BitmapInterpolationMode.None);

			return writeableBitmap;
		}

		static Bitmap G1IndexedToBitmapScaled(G1Element32 g1Element, PaletteMap paletteMap, bool useTransparency = false, int zoom = 1)
		{
			var info = new SKImageInfo(g1Element.Width, g1Element.Height, SKColorType.Rgba8888, SKAlphaType.Opaque);
			var img = SKImage.Create(info);
			var bmp = SKBitmap.FromImage(img);

			unsafe
			{
				var ptr = (uint*)bmp.GetPixels();
				for (var y = 0; y < g1Element.Height; y++)
				{
					for (var x = 0; x < g1Element.Width; x++)
					{
						var index = x + (y * g1Element.Width);
						var paletteIndex = g1Element.ImageData[index];

						if (paletteIndex == 0 && useTransparency)
						{
							ptr += 4;
						}
						else
						{
							var colour = paletteMap.Palette[paletteIndex].Color;
							var pixel = colour.ToPixel<Rgb24>();

							//var ptr = (byte*)pointer;
							*ptr++ = pixel.R;
							*ptr++ = pixel.G;
							*ptr++ = pixel.B;
							*ptr++ = 255;
						}
					}
				}
			}

			var scaledImage = new SKBitmap(g1Element.Width * zoom, g1Element.Height * zoom, SKColorType.Rgba8888, SKAlphaType.Opaque);
			_ = bmp.ScalePixels(scaledImage, SKFilterQuality.None);

			// Encode the SKBitmap into a memory stream (using PNG format for best compatibility)
			using (var memoryStream = new MemoryStream())
			{
				scaledImage.Encode(memoryStream, SKEncodedImageFormat.Png, 100); // 100 is the quality (0-100)

				// Create an Avalonia Bitmap from the memory stream
				memoryStream.Position = 0;
				var avaloniaBitmap = new Bitmap(memoryStream);
				return avaloniaBitmap;
			}
			// bug in avalonia/skiasharp/skia: https://github.com/AvaloniaUI/Avalonia/issues/8444
			//return writeableBitmap.CreateScaledBitmap(new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom), BitmapInterpolationMode.None);

			//return writeableBitmap;
		}
	}
}
