using Definitions.Database;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers;

public static class DtoObjectDockMapper
{
	public static DtoObjectDock ToDto(this TblObjectDock tblobjectdock) => new()
	{
		BuildCostFactor = tblobjectdock.BuildCostFactor,
		SellCostFactor = tblobjectdock.SellCostFactor,
		CostIndex = tblobjectdock.CostIndex,
		Flags = tblobjectdock.Flags,
		DesignedYear = tblobjectdock.DesignedYear,
		ObsoleteYear = tblobjectdock.ObsoleteYear,
		BoatPositionX = tblobjectdock.BoatPositionX,
		BoatPositionY = tblobjectdock.BoatPositionY,
		BuildingComponents = tblobjectdock.BuildingComponents,
		Id = tblobjectdock.Id,
	};

	public static TblObjectDock ToTblObjectDockEntity(this DtoObjectDock model, TblObject parent) => new()
	{
		Parent = parent,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		BoatPositionX = model.BoatPositionX,
		BoatPositionY = model.BoatPositionY,
		BuildingComponents = model.BuildingComponents,
		Id = model.Id,
	};
}