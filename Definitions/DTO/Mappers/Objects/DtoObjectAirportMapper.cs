using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectAirportMapper
	{
		public static DtoObjectAirport ToDto(this TblObjectAirport tblobjectairport)
		{
			return new DtoObjectAirport
			{
				BuildCostFactor = tblobjectairport.BuildCostFactor,
				SellCostFactor = tblobjectairport.SellCostFactor,
				CostIndex = tblobjectairport.CostIndex,
				AllowedPlaneTypes = tblobjectairport.AllowedPlaneTypes,
				LargeTiles = tblobjectairport.LargeTiles,
				MinX = tblobjectairport.MinX,
				MinY = tblobjectairport.MinY,
				MaxX = tblobjectairport.MaxX,
				MaxY = tblobjectairport.MaxY,
				DesignedYear = tblobjectairport.DesignedYear,
				ObsoleteYear = tblobjectairport.ObsoleteYear,
				Id = tblobjectairport.Id,
			};
		}

		public static TblObjectAirport ToTblObjectAirportEntity(this DtoObjectAirport model)
		{
			return new TblObjectAirport
			{
				BuildCostFactor = model.BuildCostFactor,
				SellCostFactor = model.SellCostFactor,
				CostIndex = model.CostIndex,
				AllowedPlaneTypes = model.AllowedPlaneTypes,
				LargeTiles = model.LargeTiles,
				MinX = model.MinX,
				MinY = model.MinY,
				MaxX = model.MaxX,
				MaxY = model.MaxY,
				DesignedYear = model.DesignedYear,
				ObsoleteYear = model.ObsoleteYear,
				Id = model.Id,
			};
		}
	}
}
