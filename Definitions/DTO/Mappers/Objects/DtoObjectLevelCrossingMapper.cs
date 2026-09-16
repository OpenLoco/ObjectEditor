using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectLevelCrossingMapper
{
	public static DtoObjectLevelCrossing ToDto(this TblObjectLevelCrossing tblobjectlevelcrossing) => new()
	{
		BuildCostFactor = tblobjectlevelcrossing.BuildCostFactor,
		SellCostFactor = tblobjectlevelcrossing.SellCostFactor,
		CostIndex = tblobjectlevelcrossing.CostIndex,
		ClosedAnimationFrameInterval = tblobjectlevelcrossing.ClosedAnimationFrameInterval,
		ClosedAnimationFrameCount = tblobjectlevelcrossing.ClosedAnimationFrameCount,
		TransitionAnimationFrameCount = tblobjectlevelcrossing.TransitionAnimationFrameCount,
		DesignedYear = tblobjectlevelcrossing.DesignedYear,
		Id = tblobjectlevelcrossing.Id,
	};

	public static TblObjectLevelCrossing ToTblObjectLevelCrossingEntity(this DtoObjectLevelCrossing model, TblObject parent) => new()
	{
		Parent = parent,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		ClosedAnimationFrameInterval = model.ClosedAnimationFrameInterval,
		ClosedAnimationFrameCount = model.ClosedAnimationFrameCount,
		TransitionAnimationFrameCount = model.TransitionAnimationFrameCount,
		DesignedYear = model.DesignedYear,
		Id = model.Id,
	};

}

