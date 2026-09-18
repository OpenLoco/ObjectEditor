using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectDockMapper
{
	public static DtoObjectDock ToDto(this TblObjectDock tblobjectdock) => new()
	{
		BuildCostFactor = tblobjectdock.BuildCostFactor,
		SellCostFactor = tblobjectdock.SellCostFactor,
		CostIndex = tblobjectdock.CostIndex,
		Flags = tblobjectdock.Flags,
		BuildingComponents = tblobjectdock.BuildingComponents,
		DesignedYear = tblobjectdock.DesignedYear,
		ObsoleteYear = tblobjectdock.ObsoleteYear,
		BoatPosition = tblobjectdock.BoatPosition,
		Id = tblobjectdock.Id,
	};

	public static TblObjectDock ToTblObjectDockEntity(this DtoObjectDock model, TblObject parent) => new()
	{
		Parent = parent,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
		BuildingComponents = model.BuildingComponents,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		BoatPosition = model.BoatPosition,
		Id = model.Id,
	};

}

