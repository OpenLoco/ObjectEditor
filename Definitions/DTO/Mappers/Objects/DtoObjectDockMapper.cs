using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectDockMapper
	{
		public static DtoObjectDock ToDto(this TblObjectDock tblobjectdock)
		{
			return new DtoObjectDock
			{
				BuildCostFactor = tblobjectdock.BuildCostFactor,
				SellCostFactor = tblobjectdock.SellCostFactor,
				CostIndex = tblobjectdock.CostIndex,
				Flags = tblobjectdock.Flags,
				NumBuildingPartAnimations = tblobjectdock.NumBuildingPartAnimations,
				NumBuildingVariationParts = tblobjectdock.NumBuildingVariationParts,
				DesignedYear = tblobjectdock.DesignedYear,
				ObsoleteYear = tblobjectdock.ObsoleteYear,
				BoatPositionX = tblobjectdock.BoatPositionX,
				BoatPositionY = tblobjectdock.BoatPositionY,
				Id = tblobjectdock.Id,
			};
		}

		public static TblObjectDock ToTblObjectDockEntity(this DtoObjectDock model)
		{
			return new TblObjectDock
			{
				BuildCostFactor = model.BuildCostFactor,
				SellCostFactor = model.SellCostFactor,
				CostIndex = model.CostIndex,
				Flags = model.Flags,
				NumBuildingPartAnimations = model.NumBuildingPartAnimations,
				NumBuildingVariationParts = model.NumBuildingVariationParts,
				DesignedYear = model.DesignedYear,
				ObsoleteYear = model.ObsoleteYear,
				BoatPositionX = model.BoatPositionX,
				BoatPositionY = model.BoatPositionY,
				Id = model.Id,
			};
		}

	}
}

