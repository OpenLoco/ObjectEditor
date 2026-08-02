using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectLevelCrossingMapper
{
	public static DtoObjectLevelCrossing ToDto(this TblObjectLevelCrossing tblobjectlevelcrossing) => new()
	{
		CostFactor = tblobjectlevelcrossing.CostFactor,
		SellCostFactor = tblobjectlevelcrossing.SellCostFactor,
		CostIndex = tblobjectlevelcrossing.CostIndex,
		AnimationSpeed = tblobjectlevelcrossing.ClosedAnimationDelay,
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
		ClosedAnimationDelay = model.AnimationSpeed,
		ClosedAnimationFrameCount = model.ClosingFrames,
		TransitionAnimationFrameCount = model.ClosedFrames,
		DesignedYear = model.DesignedYear,
		Id = model.Id,
	};

}

