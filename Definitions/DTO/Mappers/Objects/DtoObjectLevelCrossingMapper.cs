using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectLevelCrossingMapper
	{
		public static DtoObjectLevelCrossing ToDto(this TblObjectLevelCrossing tblobjectlevelcrossing) => new()
		{
			CostFactor = tblobjectlevelcrossing.CostFactor,
			SellCostFactor = tblobjectlevelcrossing.SellCostFactor,
			CostIndex = tblobjectlevelcrossing.CostIndex,
			AnimationSpeed = tblobjectlevelcrossing.AnimationSpeed,
			ClosingFrames = tblobjectlevelcrossing.ClosingFrames,
			ClosedFrames = tblobjectlevelcrossing.ClosedFrames,
			DesignedYear = tblobjectlevelcrossing.DesignedYear,
			Id = tblobjectlevelcrossing.Id,
		};

		public static TblObjectLevelCrossing ToTblObjectLevelCrossingEntity(this DtoObjectLevelCrossing model, TblObject parent) => new()
		{
			Parent = parent,
			CostFactor = model.CostFactor,
			SellCostFactor = model.SellCostFactor,
			CostIndex = model.CostIndex,
			AnimationSpeed = model.AnimationSpeed,
			ClosingFrames = model.ClosingFrames,
			ClosedFrames = model.ClosedFrames,
			DesignedYear = model.DesignedYear,
			Id = model.Id,
		};

	}
}

