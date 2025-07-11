using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectTrackStationMapper
{
	public static DtoObjectTrackStation ToDto(this TblObjectTrackStation tblobjecttrackstation) => new()
	{
		PaintStyle = tblobjecttrackstation.PaintStyle,
		Height = tblobjecttrackstation.Height,
		BuildCostFactor = tblobjecttrackstation.BuildCostFactor,
		SellCostFactor = tblobjecttrackstation.SellCostFactor,
		CostIndex = tblobjecttrackstation.CostIndex,
		Flags = tblobjecttrackstation.Flags,
		DesignedYear = tblobjecttrackstation.DesignedYear,
		ObsoleteYear = tblobjecttrackstation.ObsoleteYear,
		Id = tblobjecttrackstation.Id,
	};

	public static TblObjectTrackStation ToTblObjectTrackStationEntity(this DtoObjectTrackStation model, TblObject parent) => new()
	{
		Parent = parent,
		PaintStyle = model.PaintStyle,
		Height = model.Height,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		Flags = model.Flags,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		Id = model.Id,
	};

}

