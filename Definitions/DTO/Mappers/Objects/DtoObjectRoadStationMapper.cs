using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectRoadStationMapper
{
	public static DtoObjectRoadStation ToDto(this TblObjectRoadStation tblobjectroadstation) => new()
	{
		PaintStyle = tblobjectroadstation.PaintStyle,
		Height = tblobjectroadstation.Height,
		RoadPieces = tblobjectroadstation.RoadPieces,
		BuildCostFactor = tblobjectroadstation.BuildCostFactor,
		SellCostFactor = tblobjectroadstation.SellCostFactor,
		CostIndex = tblobjectroadstation.CostIndex,
		Flags = tblobjectroadstation.Flags,
		CompatibleRoadObjects = tblobjectroadstation.CompatibleRoadObjects,
		DesignedYear = tblobjectroadstation.DesignedYear,
		ObsoleteYear = tblobjectroadstation.ObsoleteYear,
		CargoType = tblobjectroadstation.CargoType,
		CargoOffsets = tblobjectroadstation.CargoOffsets,
		Id = tblobjectroadstation.Id,
	};

	public static TblObjectRoadStation ToTblObjectRoadStationEntity(this DtoObjectRoadStation model, TblObject parent) => new()
	{
		Parent = parent,
		PaintStyle = model.PaintStyle,
		Height = model.Height,
		RoadPieces = model.RoadPieces,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
		CompatibleRoadObjects = model.CompatibleRoadObjects,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		CargoType = model.CargoType,
		CargoOffsets = model.CargoOffsets,
		Id = model.Id,
	};

}

