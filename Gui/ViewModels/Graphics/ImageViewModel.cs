using Avalonia.Media.Imaging;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Graphics.Dithering;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace Gui.ViewModels.Graphics;

public class DesignImageViewModel : ImageViewModel
{
	public DesignImageViewModel()
	{
		Model = GraphicsElement.FromIndexed(GraphicsElementFlags.None, PaletteMapLoader.Current.CreateIndexedImageFrame(16, 16), 1, 2, 3, "NewImage", 0);
		UnderlyingImage = Model;
	}
}

public class ImageViewModel : ReactiveUI.ReactiveObject, IDisposable
{
	public string Name
		=> Model.Name;

	public int ImageTableIndex
		=> Model.ImageTableIndex;

	[Unit("px")]
	public int Width
		=> UnderlyingImage!.Width;

	[Unit("px")]
	public int Height
		=> UnderlyingImage!.Height;

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
	public Bitmap? DisplayedImage { get; private set; }

	/// <summary>If set, this dithering method is used when converting the RGBA image to palette data on the next image change.</summary>
	[Reactive, Browsable(false)]
	public DitheringMethod? DitheringMethod { get; set; }

	[Browsable(false)]
	public GraphicsElement? UnderlyingImage
	{
		get => Model;
		set
		{
			// The table owns the same GraphicsElement that this view model models, and Model is init-only,
			// so adopt the replacement's pixel data and metadata in-place rather than reassigning Model.
			if (Model != null && value != null && !ReferenceEquals(Model, value))
			{
				// AdoptDecodedFrom transfers ownership of value's decoded frames into Model before we
				// dispose value, so the two never double-dispose the same Image/IndexedImageFrame.
				Model.AdoptDecodedFrom(value);
				Model.Width = value.Width;
				Model.Height = value.Height;
				Model.XOffset = value.XOffset;
				Model.YOffset = value.YOffset;
				Model.ZoomOffset = value.ZoomOffset;
				Model.Flags = value.Flags;
				Model.Name = value.Name;
				Model.ImageTableIndex = value.ImageTableIndex;
				value.Dispose();
			}

			this.RaisePropertyChanged(nameof(UnderlyingImage));
		}
	}

	[Browsable(false)]
	public Avalonia.Rect SelectedBitmapPreviewBorder
		=> DisplayedImage == null
		? new Avalonia.Rect()
			: new Avalonia.Rect(
				XOffset - 1,
				YOffset - 1,
				DisplayedImage.Size.Width + 2,
				DisplayedImage.Size.Height + 2);

	protected GraphicsElement Model { get; init; } = null!;

	readonly PaletteMap paletteMap;

	public ImageViewModel()
	{ }

	readonly CompositeDisposable subscriptions = [];
	bool disposed;

	public ImageViewModel(GraphicsElement image, PaletteMap paletteMap)
	{
		Model = image;
		this.paletteMap = paletteMap;
		UnderlyingImage = image;

		_ = this.WhenAnyValue(o => o.UnderlyingImage, o => o.DitheringMethod)
			.Where(x => x.Item1 != null)
			.Subscribe(_ => RefreshDisplayedImage())
			.DisposeWith(subscriptions);

		_ = this.WhenAnyValue(o => o.DisplayedImage, o => o.XOffset, o => o.YOffset, o => o.Width, o => o.Height)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)))
			.DisposeWith(subscriptions);
	}

	public void RecolourImage(ColourSwatch primary, ColourSwatch secondary, PaletteMap paletteMap)
	{
		// Decode the underlying image to RGBA, applying the chosen remap swatches to company colours.
		// The image table is not modified - this is a view-only preview of the remap effect.
		// DecodeToRgba returns a fresh, caller-owned image (never cached on the element) so the preview
		// always reflects the currently-selected swatches and can never feed back into the saved palette data.
		using var recoloured = UnderlyingImage!.DecodeToRgba(paletteMap, primary, secondary);

		// only update the UI image - don't update the underlying image as we want to keep the original
		SetDisplayedImage(recoloured.ToAvaloniaBitmap());
	}

	void SetDisplayedImage(Bitmap? bitmap)
	{
		if (ReferenceEquals(DisplayedImage, bitmap))
		{
			return;
		}

		DisplayedImage?.Dispose();
		DisplayedImage = bitmap;
		this.RaisePropertyChanged(nameof(DisplayedImage));
	}

	/// <summary>Refreshes the displayed preview from the underlying element's source of truth. Dithering is
	/// already baked into the indexed frame at import/replace time, so a palette image's preview shows the
	/// indexed (dithered) result naturally. Nothing is written back to the element - previews never mutate it.</summary>
	void RefreshDisplayedImage()
	{
		using var rgba = UnderlyingImage!.ToRgba(paletteMap);
		Model.Width = (short)rgba.Width;
		Model.Height = (short)rgba.Height;
		SetDisplayedImage(rgba.ToAvaloniaBitmap());

		this.RaisePropertyChanged(nameof(Width));
		this.RaisePropertyChanged(nameof(Height));
	}

	public void CropImage()
	{
		if (UnderlyingImage == null)
		{
			return;
		}

		// Crop mutates the GraphicsElement (Model) in-place, adjusting its offsets.
		UnderlyingImage.Crop(paletteMap);
		RefreshDisplayedImage();
		this.RaisePropertyChanged(nameof(XOffset));
		this.RaisePropertyChanged(nameof(YOffset));
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}

		if (disposing)
		{
			subscriptions.Dispose();
			DisplayedImage?.Dispose();
			// The underlying GraphicsElement is owned by the model and may be shared across view models.
			// Do not dispose it here during regrouping.
		}

		disposed = true;
	}
}
