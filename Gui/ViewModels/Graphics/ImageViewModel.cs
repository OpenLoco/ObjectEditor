using Avalonia.Media.Imaging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Gui.Models;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gui.ViewModels.Graphics;

public class ImageViewModel : ReactiveUI.ReactiveObject
{
	public string Name => Model.Name;
	public int ImageTableIndex => Model.ImageTableIndex;

	[Unit("px")]
	public int Width => UnderlyingImage.Width;

	[Unit("px")]
	public int Height => UnderlyingImage.Height;

	[Unit("px")]
	public short XOffset
	{
		get => Model.XOffset;
		set
		{
			Model.XOffset = value;
			this.RaisePropertyChanged(nameof(XOffset));
		}
	}

	[Unit("px")]
	public short YOffset
	{
		get => Model.YOffset;
		set
		{
			Model.YOffset = value;
			this.RaisePropertyChanged(nameof(YOffset));
		}
	}

	public short ZoomOffset
	{
		get => Model.ZoomOffset;
		set
		{
			Model.ZoomOffset = value;
			this.RaisePropertyChanged(nameof(ZoomOffset));
		}
	}

	[EnumProhibitValues<GraphicsElementFlags>(GraphicsElementFlags.None)]
	public GraphicsElementFlags Flags
	{
		get => Model.Flags;
		set
		{
			Model.Flags = value;
			this.RaisePropertyChanged(nameof(Flags));
		}
	}

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

	GraphicsElement Model { get; init; }

	public ImageViewModel(GraphicsElement graphicsElement)
	{
		Model = graphicsElement;

		_ = this.WhenAnyValue(o => o.UnderlyingImage)
			.Where(x => x != null)
			.Subscribe(_ => UnderlyingImageChanged());

		_ = this.WhenAnyValue(o => o.DisplayedImage)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)));

		UnderlyingImage = Model.Image!;
	}

	public void RecolourImage(ColourSwatch primary, ColourSwatch secondary, PaletteMap paletteMap)
	{
		// turn rgba32 into raw palette image
		var rawData = paletteMap.ConvertRgba32ImageToG1Data(UnderlyingImage, Flags);

		var dummyElement = new GraphicsElement
		{
			Name = Name, // not necessary for palette
			Width = (short)UnderlyingImage.Width,
			Height = (short)UnderlyingImage.Height,
			XOffset = XOffset,
			YOffset = YOffset,
			Flags = Flags,
			ZoomOffset = ZoomOffset,
			ImageData = rawData,
			ImageTableIndex = ImageTableIndex, // not necessary for palette
		};

		if (!paletteMap.TryConvertG1ToRgba32Bitmap(dummyElement, primary, secondary, out var image))
		{
			throw new InvalidOperationException("Failed to recolour image");
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
			XOffset = 0;
			YOffset = 0;
		}
		else
		{
			UnderlyingImage.Mutate(i => i.Crop(cropRegion));
			XOffset += (short)cropRegion.Left;
			YOffset += (short)cropRegion.Top;
		}

		UnderlyingImageChanged();

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

	public GraphicsElement ToGraphicsElement(PaletteMap paletteMap)
	{
		if (UnderlyingImage == null)
		{
			throw new InvalidOperationException("Cannot convert to GraphicsElement when UnderlyingImage is null");
		}

		// turn rgba32 into raw palette image
		var rawData = paletteMap.ConvertRgba32ImageToG1Data(UnderlyingImage, Flags);
		return new GraphicsElement
		{
			Width = (short)UnderlyingImage.Width,
			Height = (short)UnderlyingImage.Height,
			XOffset = XOffset,
			YOffset = YOffset,
			Flags = Flags,
			ZoomOffset = ZoomOffset,
			ImageData = rawData,
			ImageTableIndex = ImageTableIndex,
		};
	}
}
