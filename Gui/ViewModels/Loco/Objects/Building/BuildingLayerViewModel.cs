using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Gui.ViewModels.Loco.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingLayerViewModel : ReactiveObject
{
	readonly ObservableAsPropertyHelper<double> x;
	readonly ObservableAsPropertyHelper<double> y;

	[Reactive] public Bitmap? DisplayedImage { get; set; }

	public double Width => DisplayedImage?.Size.Width ?? 0;
	public double Height => DisplayedImage?.Size.Height ?? 0;

	public double X => x.Value;
	public double Y => y.Value;

	[Reactive] public double XBase { get; set; }
	[Reactive] public double YBase { get; set; }

	[Reactive] public double XOffset { get; set; }
	[Reactive] public double YOffset { get; set; }

	public BuildingLayerViewModel()
	{
		x = this.WhenAnyValue(o => o.XBase, o => o.XOffset, (xBase, xOffset) => xBase + xOffset)
			.ToProperty(this, o => o.X);
		y = this.WhenAnyValue(o => o.YBase, o => o.YOffset, (yBase, yOffset) => yBase + yOffset)
			.ToProperty(this, o => o.Y);
	}
}

