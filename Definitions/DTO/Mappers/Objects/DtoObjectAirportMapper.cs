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
		BuildingComponents = tblobjectairport.BuildingComponents,
		BuildingPositions = tblobjectairport.BuildingPositions,
		MovementNodes = tblobjectairport.MovementNodes,
		MovementEdges = tblobjectairport.MovementEdges,
		RequiredClearEdges = tblobjectairport.RequiredClearEdges,
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
		BuildingComponents = model.BuildingComponents,
		BuildingPositions = model.BuildingPositions,
		MovementNodes = model.MovementNodes,
		MovementEdges = model.MovementEdges,
		RequiredClearEdges = model.RequiredClearEdges,
		Id = model.Id,
	};
}
