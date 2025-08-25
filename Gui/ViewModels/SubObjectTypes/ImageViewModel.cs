using Avalonia.Media.Imaging;
using Definitions.ObjectModels.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System;

namespace Gui.ViewModels;

public class ImageViewModel : ReactiveObject
{
	[ReadOnly(true)] public int ImageIndex { get; init; }
	[ReadOnly(true)] public string ImageName { get; init; }
	[ReadOnly(true)] public short Width { get; init; }
	[ReadOnly(true)] public short Height { get; init; }

	[Reactive] public short XOffset { get; set; }
	[Reactive] public short YOffset { get; set; }

	public GraphicsElementFlags Flags { get; set; }
	public short ZoomOffset { get; set; }

	[Reactive, Browsable(false)]
	public Bitmap Image { get; set; }

	[Browsable(false)]
	public Avalonia.Size SelectedBitmapPreviewBorder
		=> Image == null
			? new Avalonia.Size()
			: new Avalonia.Size(Image.Size.Width + 2, Image.Size.Height + 2);

	ImageViewModel() { }

	public ImageViewModel(int imageIndex, string imageName, GraphicsElement graphicsElement, Bitmap image)
		: this()
	{
		ImageIndex = imageIndex;
		ImageName = imageName;
		Width = graphicsElement.Width;
		Height = graphicsElement.Height;
		XOffset = graphicsElement.XOffset;
		YOffset = graphicsElement.YOffset;
		Flags = graphicsElement.Flags;
		ZoomOffset = graphicsElement.ZoomOffset;
		Image = image;

		_ = this.WhenAnyValue(o => o.Image)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedBitmapPreviewBorder)));
	}
}
