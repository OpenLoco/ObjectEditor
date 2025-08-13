using Definitions.Database;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectInterface : IDtoSubObject
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
	public UniqueObjectId Id { get; set; }
}
