using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectInterfaceMapper
	{
		public static DtoObjectInterface ToDto(this TblObjectInterface tblobjectinterface)
		{
			return new DtoObjectInterface
			{
				MapTooltipObjectColour = tblobjectinterface.MapTooltipObjectColour,
				MapTooltipCargoColour = tblobjectinterface.MapTooltipCargoColour,
				TooltipColour = tblobjectinterface.TooltipColour,
				ErrorColour = tblobjectinterface.ErrorColour,
				WindowPlayerColor = tblobjectinterface.WindowPlayerColor,
				WindowTitlebarColour = tblobjectinterface.WindowTitlebarColour,
				WindowColour = tblobjectinterface.WindowColour,
				WindowConstructionColour = tblobjectinterface.WindowConstructionColour,
				WindowTerraFormColour = tblobjectinterface.WindowTerraFormColour,
				WindowMapColour = tblobjectinterface.WindowMapColour,
				WindowOptionsColour = tblobjectinterface.WindowOptionsColour,
				Colour_11 = tblobjectinterface.Colour_11,
				TopToolbarPrimaryColour = tblobjectinterface.TopToolbarPrimaryColour,
				TopToolbarSecondaryColour = tblobjectinterface.TopToolbarSecondaryColour,
				TopToolbarTertiaryColour = tblobjectinterface.TopToolbarTertiaryColour,
				TopToolbarQuaternaryColour = tblobjectinterface.TopToolbarQuaternaryColour,
				PlayerInfoToolbarColour = tblobjectinterface.PlayerInfoToolbarColour,
				TimeToolbarColour = tblobjectinterface.TimeToolbarColour,
				Id = tblobjectinterface.Id,
			};
		}

		public static TblObjectInterface ToTblObjectInterfaceEntity(this DtoObjectInterface model)
		{
			return new TblObjectInterface
			{
				MapTooltipObjectColour = model.MapTooltipObjectColour,
				MapTooltipCargoColour = model.MapTooltipCargoColour,
				TooltipColour = model.TooltipColour,
				ErrorColour = model.ErrorColour,
				WindowPlayerColor = model.WindowPlayerColor,
				WindowTitlebarColour = model.WindowTitlebarColour,
				WindowColour = model.WindowColour,
				WindowConstructionColour = model.WindowConstructionColour,
				WindowTerraFormColour = model.WindowTerraFormColour,
				WindowMapColour = model.WindowMapColour,
				WindowOptionsColour = model.WindowOptionsColour,
				Colour_11 = model.Colour_11,
				TopToolbarPrimaryColour = model.TopToolbarPrimaryColour,
				TopToolbarSecondaryColour = model.TopToolbarSecondaryColour,
				TopToolbarTertiaryColour = model.TopToolbarTertiaryColour,
				TopToolbarQuaternaryColour = model.TopToolbarQuaternaryColour,
				PlayerInfoToolbarColour = model.PlayerInfoToolbarColour,
				TimeToolbarColour = model.TimeToolbarColour,
				Id = model.Id,
			};
		}

	}
}

