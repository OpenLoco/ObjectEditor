using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectRoadStationMapper
{
	public static DtoObjectRoadStation ToDto(this TblObjectRoadStation tblobjectroadstation) => new()
	{
		PaintStyle = tblobjectroadstation.PaintStyle,
		Height = tblobjectroadstation.Height,
		BuildCostFactor = tblobjectroadstation.BuildCostFactor,
		SellCostFactor = tblobjectroadstation.SellCostFactor,
		CostIndex = tblobjectroadstation.CostIndex,
		Flags = tblobjectroadstation.Flags,
		CompatibleRoadObjectCount = tblobjectroadstation.CompatibleRoadObjectCount,
		DesignedYear = tblobjectroadstation.DesignedYear,
		ObsoleteYear = tblobjectroadstation.ObsoleteYear,
		Id = tblobjectroadstation.Id,
	};

	public static TblObjectRoadStation ToTblObjectRoadStationEntity(this DtoObjectRoadStation model, TblObject parent) => new()
	{
		Parent = parent,
		PaintStyle = model.PaintStyle,
		Height = model.Height,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
		CompatibleRoadObjectCount = model.CompatibleRoadObjectCount,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		Id = model.Id,
	};

}

