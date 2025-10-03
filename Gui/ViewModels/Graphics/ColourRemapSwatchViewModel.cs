using Definitions.ObjectModels.Graphics;
using ReactiveUI.Fody.Helpers;
using AvaColour = Avalonia.Media.Color;

namespace Gui.ViewModels.Graphics;

public class ColourRemapSwatchViewModel
{
	[Reactive] public ColourSwatch Swatch { get; init; }
	[Reactive] public AvaColour Colour { get; init; }

	[Reactive] public AvaColour[] GradientColours { get; init; }
}
