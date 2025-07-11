using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectRoadMapper
{
	public static DtoObjectRoad ToDto(this TblObjectRoad tblobjectroad) => new()
	{
		RoadPieces = tblobjectroad.RoadPieces,
		BuildCostFactor = tblobjectroad.BuildCostFactor,
		SellCostFactor = tblobjectroad.SellCostFactor,
		TunnelCostFactor = tblobjectroad.TunnelCostFactor,
		CostIndex = tblobjectroad.CostIndex,
		MaxSpeed = tblobjectroad.MaxSpeed,
		Flags = tblobjectroad.Flags,
		PaintStyle = tblobjectroad.PaintStyle,
		DisplayOffset = tblobjectroad.DisplayOffset,
		TargetTownSize = tblobjectroad.TargetTownSize,
		Id = tblobjectroad.Id,
	};

	public static TblObjectRoad ToTblObjectRoadEntity(this DtoObjectRoad model, TblObject parent) => new()
	{
		Parent = parent,
		RoadPieces = model.RoadPieces,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		TunnelCostFactor = model.TunnelCostFactor,
		CostIndex = model.CostIndex,
		MaxSpeed = model.MaxSpeed,
		Flags = model.Flags,
		PaintStyle = model.PaintStyle,
		DisplayOffset = model.DisplayOffset,
		TargetTownSize = model.TargetTownSize,
		Id = model.Id,
	};

}

