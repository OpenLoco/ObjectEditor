using AvaGui.Models;
using Avalonia.Media.Imaging;
using OpenLoco.Dat;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaGui.ViewModels
{
	public record UIG1Element32(
		[Category("Image")] int ImageIndex,
		[Category("Image")] string ImageName,
		[Category("G1Element32")] uint32_t Offset,
		[Category("G1Element32")] int16_t Width,
		[Category("G1Element32")] int16_t Height,
		[Category("G1Element32")] int16_t XOffset,
		[Category("G1Element32")] int16_t YOffset,
		[Category("G1Element32")] G1ElementFlags Flags,
		[Category("G1Element32")] int16_t ZoomOffset
	)
	{
		public UIG1Element32(int imageIndex, string imageName, G1Element32 g1Element)
			: this(
				imageIndex,
				imageName,
				g1Element.Offset,
				g1Element.Width,
				g1Element.Height,
				g1Element.XOffset,
				g1Element.YOffset,
				g1Element.Flags,
				g1Element.ZoomOffset)
		{ }
	}

	public class ImageTableViewModel : ReactiveObject, IExtraContentViewModel
	{
		readonly ILocoObject Parent;

		public ImageTableViewModel(ILocoObject parent, PaletteMap paletteMap, IList<Image<Rgba32>> images)
		{
			Parent = parent;
			PaletteMap = paletteMap;
			Images = images;

			_ = this.WhenAnyValue(o => o.Parent)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.PaletteMap)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.Zoom)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Images)));
			_ = this.WhenAnyValue(o => o.SelectedImageIndex)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedG1Element)));

			_ = this.WhenAnyValue(o => o.Images)
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
					var img = Image.Load<Rgba32>(file);
					Images[i] = img;
					Parent.G1Elements[i++].ImageData = PaletteMap.ConvertRgb32ImageToG1Data(img); // simply overwrite existing pixel data
				}
			}

			this.RaisePropertyChanged(nameof(Bitmaps));
			//this.RaisePropertyChanged(nameof(Images));
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
					await image.SaveAsPngAsync(path);
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

		[Reactive]
		public IList<Image<Rgba32>> Images { get; set; }

		public IList<Bitmap?> Bitmaps
		{
			get
			{
				// this shenanigans is to handle the DuplicatePrevious flag. we store it as null
				// and here we just reuse the previous image if the flag is set (ie null image)
				var list = G1ImageConversion.CreateAvaloniaImages(Images).ToList();
				Bitmap? prevValue = null;

				for (var i = 0; i < list.Count; i++)
				{
					if (list[i] == null)
					{
						list[i] = prevValue;
					}
					else
					{
						prevValue = list[i];
					}
				}
				return list;
			}
		}

		[Reactive]
		public int SelectedImageIndex { get; set; } = -1;

		public UIG1Element32? SelectedG1Element
			=> SelectedImageIndex == -1 || Parent.G1Elements.Count == 0 ? null : new UIG1Element32(SelectedImageIndex, GetImageName(Parent, SelectedImageIndex), Parent.G1Elements[SelectedImageIndex]);

		public static string GetImageName(ILocoObject locoObj, int counter)
		{
			ILocoImageTableNames? its = null;
			//var objectName = string.Empty;

			if (locoObj.Object is ILocoImageTableNames itss)
			{
				its = itss;
				//objectName = locoObj.DatFileInfo.S5Header.Name;
			}
			//else if (uiObj is UiG1 uiG1 && uiG1.G1 is ILocoImageTableNames itsg)
			//{
			//	its = itsg;
			//	objectName = "g1.dat";
			//}

			if (its != null && its.TryGetImageName(counter, out var value) && value != null)
			{
				return $"{value}";
			}

			return "<unk>";
		}
	}
}
