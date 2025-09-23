using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectAirportMapper
{
	public static DtoObjectAirport ToDto(this TblObjectAirport tblobjectairport) => new()
	{
		BuildCostFactor = tblobjectairport.BuildCostFactor,
		SellCostFactor = tblobjectairport.SellCostFactor,
		CostIndex = tblobjectairport.CostIndex,
		Flags = tblobjectairport.Flags,
		LargeTiles = tblobjectairport.LargeTiles,
		MinX = tblobjectairport.MinX,
		MinY = tblobjectairport.MinY,
		MaxX = tblobjectairport.MaxX,
		MaxY = tblobjectairport.MaxY,
		DesignedYear = tblobjectairport.DesignedYear,
		ObsoleteYear = tblobjectairport.ObsoleteYear,
		Id = tblobjectairport.Id,
	};

	public static TblObjectAirport ToTblObjectAirportEntity(this DtoObjectAirport model, TblObject parent) => new()
	{
		Parent = parent,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
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
