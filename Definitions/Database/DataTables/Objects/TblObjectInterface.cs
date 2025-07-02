using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectInterface : DbSubObject, IConvertibleToTable<TblObjectInterface, InterfaceSkinObject>
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

		public static TblObjectInterface FromObject(TblObject tbl, InterfaceSkinObject obj)
			=> new()
			{
				Parent = tbl,
				MapTooltipObjectColour = obj.MapTooltipObjectColour,
				MapTooltipCargoColour = obj.MapTooltipCargoColour,
				TooltipColour = obj.TooltipColour,
				ErrorColour = obj.ErrorColour,
				WindowPlayerColor = obj.WindowPlayerColor,
				WindowTitlebarColour = obj.WindowTitlebarColour,
				WindowColour = obj.WindowColour,
				WindowConstructionColour = obj.WindowConstructionColour,
				WindowTerraFormColour = obj.WindowTerraFormColour,
				WindowMapColour = obj.WindowMapColour,
				WindowOptionsColour = obj.WindowOptionsColour,
				Colour_11 = obj.Colour_11,
				TopToolbarPrimaryColour = obj.TopToolbarPrimaryColour,
				TopToolbarSecondaryColour = obj.TopToolbarSecondaryColour,
				TopToolbarTertiaryColour = obj.TopToolbarTertiaryColour,
				TopToolbarQuaternaryColour = obj.TopToolbarQuaternaryColour,
				PlayerInfoToolbarColour = obj.PlayerInfoToolbarColour,
				TimeToolbarColour = obj.TimeToolbarColour,
			};
	}
}
