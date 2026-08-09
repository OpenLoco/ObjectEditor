using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectLevelCrossingMapper
{
	public static DtoObjectLevelCrossing ToDto(this TblObjectLevelCrossing tblobjectlevelcrossing) => new()
	{
		CostFactor = tblobjectlevelcrossing.CostFactor,
		SellCostFactor = tblobjectlevelcrossing.SellCostFactor,
		CostIndex = tblobjectlevelcrossing.CostIndex,
		AnimationSpeed = tblobjectlevelcrossing.ClosedAnimationFrameInterval,
		ClosingFrames = tblobjectlevelcrossing.ClosedAnimationFrameCount,
		ClosedFrames = tblobjectlevelcrossing.TransitionAnimationFrameCount,
		DesignedYear = tblobjectlevelcrossing.DesignedYear,
		Id = tblobjectlevelcrossing.Id,
	};

	public static TblObjectLevelCrossing ToTblObjectLevelCrossingEntity(this DtoObjectLevelCrossing model, TblObject parent) => new()
	{
		Parent = parent,
		CostFactor = model.CostFactor,
		SellCostFactor = model.SellCostFactor,
		CostIndex = model.CostIndex,
		ClosedAnimationFrameInterval = model.AnimationSpeed,
		ClosedAnimationFrameCount = model.ClosingFrames,
		TransitionAnimationFrameCount = model.ClosedFrames,
		DesignedYear = model.DesignedYear,
		Id = model.Id,
	};

}

