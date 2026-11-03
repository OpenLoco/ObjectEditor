using Definitions.ObjectModels.Graphics;
using AvaColour = Avalonia.Media.Color;

namespace Gui.ViewModels.Graphics;

public class ColourRemapSwatchViewModel
{
	public ColourSwatch Swatch { get; init; }
	public AvaColour Colour { get; init; }

	public AvaColour[] GradientColours { get; init; } = [];
}
