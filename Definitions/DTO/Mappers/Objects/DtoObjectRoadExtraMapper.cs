using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectRoadExtraMapper
{
	public static DtoObjectRoadExtra ToDto(this TblObjectRoadExtra tblobjectroadextra) => new()
	{
		PaintStyle = tblobjectroadextra.PaintStyle,
		CostIndex = tblobjectroadextra.CostIndex,
		BuildCostFactor = tblobjectroadextra.BuildCostFactor,
		SellCostFactor = tblobjectroadextra.SellCostFactor,
		RoadPieces = tblobjectroadextra.RoadPieces,
		Id = tblobjectroadextra.Id,
	};

	public static TblObjectRoadExtra ToTblObjectRoadExtraEntity(this DtoObjectRoadExtra model, TblObject parent) => new()
	{
		Parent = parent,
		PaintStyle = model.PaintStyle,
		CostIndex = model.CostIndex,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		RoadPieces = model.RoadPieces,
		Id = model.Id,
	};

}

