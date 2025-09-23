using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Types;

namespace Gui.ViewModels;

public class InterfaceSkinViewModel(InterfaceSkinObject model) : LocoObjectViewModel<InterfaceSkinObject>(model)
{
	public Colour MapTooltipObjectColour
	{
		get => Model.MapTooltipObjectColour;
		set => Model.MapTooltipObjectColour = value;
	}

	public Colour MapTooltipCargoColour
	{
		get => Model.MapTooltipCargoColour;
		set => Model.MapTooltipCargoColour = value;
	}

	public Colour TooltipColour
	{
		get => Model.TooltipColour;
		set => Model.TooltipColour = value;
	}

	public Colour ErrorColour
	{
		get => Model.ErrorColour;
		set => Model.ErrorColour = value;
	}

	public Colour WindowPlayerColour
	{
		get => Model.WindowPlayerColour;
		set => Model.WindowPlayerColour = value;
	}

	public Colour WindowTitlebarColour
	{
		get => Model.WindowTitlebarColour;
		set => Model.WindowTitlebarColour = value;
	}

	public Colour WindowColour
	{
		get => Model.WindowColour;
		set => Model.WindowColour = value;
	}

	public Colour WindowConstructionColour
	{
		get => Model.WindowConstructionColour;
		set => Model.WindowConstructionColour = value;
	}

	public Colour WindowTerraFormColour
	{
		get => Model.WindowTerraFormColour;
		set => Model.WindowTerraFormColour = value;
	}

	public Colour WindowMapColour
	{
		get => Model.WindowMapColour;
		set => Model.WindowMapColour = value;
	}

	public Colour WindowOptionsColour
	{
		get => Model.WindowOptionsColour;
		set => Model.WindowOptionsColour = value;
	}

	public Colour Colour_11
	{
		get => Model.Colour_11;
		set => Model.Colour_11 = value;
	}

	public Colour TopToolbarPrimaryColour
	{
		get => Model.TopToolbarPrimaryColour;
		set => Model.TopToolbarPrimaryColour = value;
	}

	public Colour TopToolbarSecondaryColour
	{
		get => Model.TopToolbarSecondaryColour;
		set => Model.TopToolbarSecondaryColour = value;
	}

	public Colour TopToolbarTertiaryColour
	{
		get => Model.TopToolbarTertiaryColour;
		set => Model.TopToolbarTertiaryColour = value;
	}

	public Colour TopToolbarQuaternaryColour
	{
		get => Model.TopToolbarQuaternaryColour;
		set => Model.TopToolbarQuaternaryColour = value;
	}

	public Colour PlayerInfoToolbarColour
	{
		get => Model.PlayerInfoToolbarColour;
		set => Model.PlayerInfoToolbarColour = value;
	}

	public Colour TimeToolbarColour
	{
		get => Model.TimeToolbarColour;
		set => Model.TimeToolbarColour = value;
	}
}
