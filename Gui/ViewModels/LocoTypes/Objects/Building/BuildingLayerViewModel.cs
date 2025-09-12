using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gui.ViewModels.LocoTypes.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingLayerViewModel : ReactiveObject
{
	[Reactive] public Bitmap DisplayedImage { get; set; }

	public double Width => DisplayedImage.Size.Width;
	public double Height => DisplayedImage.Size.Height;

	public double X => XBase + XOffset;
	public double Y => YBase + YOffset;

	[Reactive] public double XBase { get; set; }
	[Reactive] public double YBase { get; set; }

	[Reactive] public double XOffset { get; set; }
	[Reactive] public double YOffset { get; set; }

	public BuildingLayerViewModel()
	{
		_ = this.WhenAnyValue(o => o.XBase)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(X)));
		_ = this.WhenAnyValue(o => o.XOffset)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(X)));

		_ = this.WhenAnyValue(o => o.YBase)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));
		_ = this.WhenAnyValue(o => o.YOffset)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));
	}
}

