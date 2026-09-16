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
		MaxCurveSpeed = tblobjectroad.MaxCurveSpeed,
		Flags = tblobjectroad.Flags,
		PaintStyle = tblobjectroad.PaintStyle,
		VehicleDisplayListVerticalOffset = tblobjectroad.VehicleDisplayListVerticalOffset,
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
		MaxCurveSpeed = model.MaxCurveSpeed,
		Flags = model.Flags,
		PaintStyle = model.PaintStyle,
		VehicleDisplayListVerticalOffset = model.VehicleDisplayListVerticalOffset,
		TargetTownSize = model.TargetTownSize,
		Id = model.Id,
	};

}

