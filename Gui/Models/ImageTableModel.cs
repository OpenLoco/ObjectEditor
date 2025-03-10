using Common.Json;
using OpenLoco.Common;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Types;
using OpenLoco.Gui.ViewModels;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenLoco.Gui.Models
{
	public class ImageTableModel(IList<Image<Rgba32>> images, IHasG1Elements g1ElementProvider, IImageTableNameProvider imageNameProvider, PaletteMap paletteMap, ILogger logger)
	{
		public readonly ILogger Logger = logger;
		public readonly IHasG1Elements G1Provider = g1ElementProvider;
		public readonly IImageTableNameProvider NameProvider = imageNameProvider;

		public PaletteMap PaletteMap { get; init; } = paletteMap;

		[Reactive]
		public IList<Image<Rgba32>> Images { get; set; } = images;

		public void RecalcImages(ColourRemapSwatch primary, ColourRemapSwatch secondary)
		{
			Logger.Info("Clearing current G1Element32s and existing object images");
			Images.Clear();

			foreach (var g1 in G1Provider.G1Elements)
			{
				if (PaletteMap.TryConvertG1ToRgba32Bitmap(g1, primary, secondary, out var img) && img != null)
				{
					Images.Add(img);
				}
				else
				{
					Logger.Error("Unable to convert G1 to image");
				}
			}
		}

		static Rectangle FindCropRegion(Image<Rgba32> image)
		{
			var minX = image.Width;
			var maxX = 0;
			var minY = image.Height;
			var maxY = 0;

			for (var y = 0; y < image.Height; y++)
			{
				for (var x = 0; x < image.Width; x++)
				{
					var pixel = image[x, y];

					if (pixel.A > 0)
					{
						minX = Math.Min(minX, x);
						maxX = Math.Max(maxX, x);
						minY = Math.Min(minY, y);
						maxY = Math.Max(maxY, y);
					}
				}
			}

			// Calculate the crop area. Ensure it is within image bounds.
			var width = Math.Max(0, Math.Min(maxX - minX + 1, image.Width - minX));
			var height = Math.Max(0, Math.Min(maxY - minY + 1, image.Height - minY));
			return new Rectangle(minX, minY, width, height);
		}

		public void CropAllImages(ColourRemapSwatch primary, ColourRemapSwatch secondary)
		{
			for (var i = 0; i < Images.Count; ++i)
			{
				var image = Images[i];

				var cropRegion = FindCropRegion(image);

				if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
				{
					image.Mutate(i => i.Crop(new Rectangle(0, 0, 1, 1)));
					UpdateImage(image, primary, secondary, i, 0, 0);
				}
				else
				{
					image.Mutate(i => i.Crop(cropRegion));
					var currG1 = G1Provider.G1Elements[i];

					// set to bitmaps
					UpdateImage(image, primary, secondary, i, xOffset: (short)(currG1.XOffset + cropRegion.Left), yOffset: (short)(currG1.YOffset + cropRegion.Top));
				}
			}
		}

		public string GetImageName(int index)
			=> NameProvider.TryGetImageName(index, out var value) && !string.IsNullOrEmpty(value)
				? value
				: index.ToString();

		public async Task ImportImages(string directory, ColourRemapSwatch primary, ColourRemapSwatch secondary)
		{
			if (string.IsNullOrEmpty(directory))
			{
				Logger.Error($"Directory is invalid: \"{directory}\"");
				return;
			}

			if (!Directory.Exists(directory))
			{
				Logger.Error($"Directory does not exist: \"{directory}\"");
				return;
			}

			Logger.Info($"Importing images from {directory}");

			try
			{
				// count files in dir and check naming
				var files = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
				IEnumerable<G1Element32Json> offsets;

				// check for offsets file
				var offsetsFile = Path.Combine(directory, "sprites.json");
				if (File.Exists(offsetsFile))
				{
					offsets = JsonSerializer.Deserialize<ICollection<G1Element32Json>>(File.ReadAllText(offsetsFile)); // sprites.json is an unnamed array so we need ICollection here, not IEnumerable
					ArgumentNullException.ThrowIfNull(offsets);
					Logger.Debug("Found sprites.json file; using that");
				}
				else
				{
					offsets = G1Provider.G1Elements.Select((x, i) => new G1Element32Json($"{i}.png", x.XOffset, x.YOffset));
					Logger.Debug("Didn't find sprites.json; using existing G1Element32 offsets");
				}

				offsets = offsets.Fill(files.Length, G1Element32Json.Zero);

				// clear existing images
				Logger.Info("Clearing current G1Element32s and existing object images");
				G1Provider.G1Elements.Clear();
				Images.Clear();

				// load files
				var offsetList = offsets.ToList();
				for (var i = 0; i < files.Length; ++i)
				{
					var offsetPath = offsetList[i].Path;
					var validPath = string.IsNullOrEmpty(offsetPath) ? $"{i}.png" : offsetPath;
					var filename = Path.Combine(directory, validPath);
					AddImage(filename, offsetList[i], primary, secondary);
				}

				Logger.Debug($"Imported {G1Provider.G1Elements.Count} images successfully");
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		//public void UpdateImage(string filename, ColourRemapSwatch primary, ColourRemapSwatch secondary, int index, G1Element32Json ele)
		//	=> UpdateImage(filename, primary, secondary, index, ele.Flags, ele.XOffset, ele.YOffset, ele.ZoomOffset);

		public void UpdateImage(string filename, ColourRemapSwatch primary, ColourRemapSwatch secondary, int index, G1ElementFlags? flags = null, int16_t? xOffset = null, int16_t? yOffset = null, int16_t? zoomOffset = null)
		{
			if (string.IsNullOrEmpty(filename))
			{
				Logger.Error($"Filename is invalid: \"{filename}\"");
				return;
			}

			if (!File.Exists(filename))
			{
				Logger.Error($"File doesn't exist: \"{filename}\"");
				return;
			}

			var img = Image.Load<Rgba32>(filename);
			UpdateImage(img, primary, secondary, index, flags, xOffset, yOffset, zoomOffset);
		}

		public void UpdateImage(Image<Rgba32> img, ColourRemapSwatch primary, ColourRemapSwatch secondary, int index, G1ElementFlags? flags = null, int16_t? xOffset = null, int16_t? yOffset = null, int16_t? zoomOffset = null)
		{
			if (index == -1)
			{
				return;
			}

			Images[index] = img;

			var currG1 = G1Provider.G1Elements[index];
			G1Provider.G1Elements[index] = (currG1 with
			{
				ImageData = PaletteMap.ConvertRgba32ImageToG1Data(img, flags ?? currG1.Flags, primary, secondary),
				Width = (int16_t)img.Width,
				Height = (int16_t)img.Height,
				Flags = flags ?? currG1.Flags,
				XOffset = xOffset ?? currG1.XOffset,
				YOffset = yOffset ?? currG1.YOffset,
				ZoomOffset = zoomOffset ?? currG1.ZoomOffset,
			});
		}

		void AddImage(string filename, G1Element32Json ele, ColourRemapSwatch primary, ColourRemapSwatch secondary)
		{
			if (string.IsNullOrEmpty(filename))
			{
				Logger.Error($"Filename is invalid: \"{filename}\"");
				return;
			}

			if (!File.Exists(filename))
			{
				Logger.Error($"File doesn't exist: \"{filename}\"");
				return;
			}

			var img = Image.Load<Rgba32>(filename);

			var flags = ele.Flags ?? G1ElementFlags.None;
			var newElement = new G1Element32(0, (int16_t)img.Width, (int16_t)img.Height, ele.XOffset, ele.YOffset, flags, ele.ZoomOffset ?? 0)
			{
				ImageData = PaletteMap.ConvertRgba32ImageToG1Data(img, flags, primary, secondary)
			};

			G1Provider.G1Elements.Add(newElement);
			Images.Add(img);
		}

		public async Task ExportImages(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				Logger.Error($"Directory is invalid: \"{directory}\"");
				return;
			}

			if (!Directory.Exists(directory))
			{
				Logger.Error($"Directory does not exist: \"{directory}\"");
				return;
			}

			Logger.Info($"Exporting images to {directory}");

			var counter = 0;
			var offsets = new List<G1Element32Json>();

			foreach (var image in Images)
			{
				var g1Element = G1Provider.G1Elements[counter];
				var imageName = counter.ToString(); // todo: maybe use image name provider below (but number must still exist)
				counter++;

				var fileName = $"{imageName}.png";
				var path = Path.Combine(directory, fileName);
				await image.SaveAsPngAsync(path);

				offsets.Add(new G1Element32Json(fileName, g1Element));
			}

			var offsetsFile = Path.Combine(directory, "sprites.json");
			Logger.Info($"Saving sprite offsets to {offsetsFile}");
			await JsonFile.SerializeToFileAsync(offsets, offsetsFile);
		}
	}
}
