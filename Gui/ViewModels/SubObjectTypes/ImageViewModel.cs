using Avalonia.Media.Imaging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public class ImageViewModel : ReactiveObject
{
	[ReadOnly(true)] public int ImageIndex { get; }
	[ReadOnly(true)] public string ImageName { get; }
	public int Width => UnderlyingImage.Width;
	public int Height => UnderlyingImage.Height;
	[Reactive] public int XOffset { get; set; }
	[Reactive] public int YOffset { get; set; }

	public GraphicsElementFlags Flags { get; set; }
	public short ZoomOffset { get; set; }

	[Reactive, Browsable(false)]
	public Bitmap Image { get; set; }

	[Reactive, Browsable(false)]
	public Image<Rgba32> UnderlyingImage { get; set; }

	[Browsable(false)]
	public Avalonia.Size SelectedBitmapPreviewBorder
		=> Image == null
			? new Avalonia.Size()
			: new Avalonia.Size(Image.Size.Width + 2, Image.Size.Height + 2);

	PaletteMap PaletteMap;

	ImageViewModel() { }

	public ImageViewModel(int imageIndex, string imageName, GraphicsElement graphicsElement, PaletteMap paletteMap)
		: this()
	{
		ImageIndex = imageIndex;
		ImageName = imageName;
		XOffset = graphicsElement.XOffset;
		YOffset = graphicsElement.YOffset;
		Flags = graphicsElement.Flags;
		ZoomOffset = graphicsElement.ZoomOffset;

		_ = this.WhenAnyValue(o => o.UnderlyingImage)
			.Where(x => x != null)
			.Subscribe(_ =>
			{
				Image = UnderlyingImage!.ToAvaloniaBitmap();
			});

		_ = this.WhenAnyValue(o => o.Image)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)));

		if (paletteMap.TryConvertG1ToRgba32Bitmap(graphicsElement, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
		{
			UnderlyingImage = image;
			//Image = image.ToAvaloniaBitmap();
		}

		PaletteMap = paletteMap;
	}

	public void RecolourImage(ColourRemapSwatch primary, ColourRemapSwatch secondary)
	{
		// turn rgba32 into raw palette image
		var rawData = PaletteMap.ConvertRgba32ImageToG1Data(UnderlyingImage, Flags);

		var dummyElement = new GraphicsElement
		{
			Width = (short)UnderlyingImage.Width,
			Height = (short)UnderlyingImage.Height,
			XOffset = (short)XOffset,
			YOffset = (short)YOffset,
			Flags = Flags,
			ZoomOffset = ZoomOffset,
			ImageData = rawData
		};

		if (!PaletteMap.TryConvertG1ToRgba32Bitmap(dummyElement, primary, secondary, out var image))
		{
		}
		Image = image.ToAvaloniaBitmap();
	}

	public void CropImage()
	{
		var cropRegion = FindCropRegion(UnderlyingImage);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			UnderlyingImage.Mutate(i => i.Crop(new Rectangle(0, 0, 1, 1)));

			Image = UnderlyingImage.ToAvaloniaBitmap();
			XOffset = 0;
			YOffset = 0;
		}
		else
		{
			UnderlyingImage.Mutate(i => i.Crop(cropRegion));
			Image = UnderlyingImage.ToAvaloniaBitmap();
			XOffset += cropRegion.Left;
			YOffset += cropRegion.Top;
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
	}

	public GraphicsElement ToGraphicsElement()
	{
		if (ImageIndex < 0 || UnderlyingImage == null)
		{
			throw new InvalidOperationException("Cannot convert to GraphicsElement when ImageIndex is less than 0 or UnderlyingImage is null");
		}
		// turn rgba32 into raw palette image
		var rawData = PaletteMap.ConvertRgba32ImageToG1Data(UnderlyingImage, Flags);
		return new GraphicsElement
		{
			Width = (short)UnderlyingImage.Width,
			Height = (short)UnderlyingImage.Height,
			XOffset = (short)XOffset,
			YOffset = (short)YOffset,
			Flags = Flags,
			ZoomOffset = ZoomOffset,
			ImageData = rawData
		};
	}
}
