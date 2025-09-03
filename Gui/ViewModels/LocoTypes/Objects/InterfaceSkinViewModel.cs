using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Types;

namespace Gui.ViewModels;

public class InterfaceSkinViewModel : LocoObjectViewModel<InterfaceSkinObject>
{
	public Colour MapTooltipObjectColour { get; set; }
	public Colour MapTooltipCargoColour { get; set; }
	public Colour TooltipColour { get; set; }
	public Colour ErrorColour { get; set; }
	public Colour WindowPlayerColor { get; set; }
	public Colour WindowTitlebarColour { get; set; }
	public Colour WindowColour { get; set; }
	public Colour WindowConstructionColour { get; set; }
	public Colour WindowTerraFormColour { get; set; }
	public Colour WindowMapColour { get; set; }
	public Colour WindowOptionsColour { get; set; }
	public Colour Colour_11 { get; set; }
	public Colour TopToolbarPrimaryColour { get; set; }
	public Colour TopToolbarSecondaryColour { get; set; }
	public Colour TopToolbarTertiaryColour { get; set; }
	public Colour TopToolbarQuaternaryColour { get; set; }
	public Colour PlayerInfoToolbarColour { get; set; }
	public Colour TimeToolbarColour { get; set; }

	public InterfaceSkinViewModel(InterfaceSkinObject obj)
	{
		MapTooltipObjectColour = obj.MapTooltipObjectColour;
		MapTooltipCargoColour = obj.MapTooltipCargoColour;
		TooltipColour = obj.TooltipColour;
		ErrorColour = obj.ErrorColour;
		WindowPlayerColor = obj.WindowPlayerColor;
		WindowTitlebarColour = obj.WindowTitlebarColour;
		WindowColour = obj.WindowColour;
		WindowConstructionColour = obj.WindowConstructionColour;
		WindowTerraFormColour = obj.WindowTerraFormColour;
		WindowMapColour = obj.WindowMapColour;
		WindowOptionsColour = obj.WindowOptionsColour;
		Colour_11 = obj.Colour_11;
		TopToolbarPrimaryColour = obj.TopToolbarPrimaryColour;
		TopToolbarSecondaryColour = obj.TopToolbarSecondaryColour;
		TopToolbarTertiaryColour = obj.TopToolbarTertiaryColour;
		TopToolbarQuaternaryColour = obj.TopToolbarQuaternaryColour;
		PlayerInfoToolbarColour = obj.PlayerInfoToolbarColour;
		TimeToolbarColour = obj.TimeToolbarColour;
	}

	public override InterfaceSkinObject GetAsModel()
		=> new()
		{
			MapTooltipObjectColour = MapTooltipObjectColour,
			MapTooltipCargoColour = MapTooltipCargoColour,
			TooltipColour = TooltipColour,
			ErrorColour = ErrorColour,
			WindowPlayerColor = WindowPlayerColor,
			WindowTitlebarColour = WindowTitlebarColour,
			WindowColour = WindowColour,
			WindowConstructionColour = WindowConstructionColour,
			WindowTerraFormColour = WindowTerraFormColour,
			WindowMapColour = WindowMapColour,
			WindowOptionsColour = WindowOptionsColour,
			Colour_11 = Colour_11,
			TopToolbarPrimaryColour = TopToolbarPrimaryColour,
			TopToolbarSecondaryColour = TopToolbarSecondaryColour,
			TopToolbarTertiaryColour = TopToolbarTertiaryColour,
			TopToolbarQuaternaryColour = TopToolbarQuaternaryColour,
			PlayerInfoToolbarColour = PlayerInfoToolbarColour,
			TimeToolbarColour = TimeToolbarColour,
		};
}
