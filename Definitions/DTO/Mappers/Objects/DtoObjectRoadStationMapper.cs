using Definitions.Database;
using Definitions.DTO.Objects;

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
		DesignedYear = tblobjectroadstation.DesignedYear,
		ObsoleteYear = tblobjectroadstation.ObsoleteYear,
		pad_2D = tblobjectroadstation.pad_2D,
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
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		pad_2D = model.pad_2D,
		CargoOffsets = model.CargoOffsets,
		Id = model.Id,
	};
}