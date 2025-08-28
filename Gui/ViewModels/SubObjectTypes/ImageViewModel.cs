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
	public Bitmap DisplayedImage { get; private set; }

	[Reactive, Browsable(false)]
	public Image<Rgba32> UnderlyingImage { get; set; }

	[Browsable(false)]
	public Avalonia.Rect SelectedBitmapPreviewBorder
		=> DisplayedImage == null
		? new Avalonia.Rect()
			: new Avalonia.Rect(
				XOffset - 1,
				YOffset - 1,
				DisplayedImage.Size.Width + 2,
				DisplayedImage.Size.Height + 2);

	readonly PaletteMap PaletteMap;

	public ImageViewModel(int imageIndex, string imageName, GraphicsElement graphicsElement, PaletteMap paletteMap)
	{
		ImageIndex = imageIndex;
		ImageName = imageName;
		XOffset = graphicsElement.XOffset;
		YOffset = graphicsElement.YOffset;
		Flags = graphicsElement.Flags;
		ZoomOffset = graphicsElement.ZoomOffset;
		PaletteMap = paletteMap;

		_ = this.WhenAnyValue(o => o.UnderlyingImage)
			.Where(x => x != null)
			.Subscribe(_ => UnderlyingImageChanged());

		_ = this.WhenAnyValue(o => o.DisplayedImage)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)));

		if (!PaletteMap.TryConvertG1ToRgba32Bitmap(graphicsElement, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
		{
			throw new Exception("Failed to convert image");
		}

		UnderlyingImage = image!;
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
			throw new Exception("Failed to recolour image");
		}

		// only update the UI image - don't update the underlying image as we want to keep the original
		DisplayedImage = image!.ToAvaloniaBitmap();
	}

	void UnderlyingImageChanged()
		=> DisplayedImage = UnderlyingImage!.ToAvaloniaBitmap();

	public void CropImage()
	{
		var cropRegion = FindCropRegion(UnderlyingImage);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			UnderlyingImage.Mutate(i => i.Crop(new Rectangle(0, 0, 1, 1)));
			UnderlyingImageChanged();
			XOffset = 0;
			YOffset = 0;
		}
		else
		{
			UnderlyingImage.Mutate(i => i.Crop(cropRegion));
			UnderlyingImageChanged();
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
