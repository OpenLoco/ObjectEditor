using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectBuildingMapper
	{
		public static DtoObjectBuilding ToDto(this TblObjectBuilding tblobjectbuilding)
		{
			return new DtoObjectBuilding
			{
				DesignedYear = tblobjectbuilding.DesignedYear,
				ObsoleteYear = tblobjectbuilding.ObsoleteYear,
				Flags = tblobjectbuilding.Flags,
				CostIndex = tblobjectbuilding.CostIndex,
				SellCostFactor = tblobjectbuilding.SellCostFactor,
				DemolishRatingReduction = tblobjectbuilding.DemolishRatingReduction,
				ScaffoldingSegmentType = tblobjectbuilding.ScaffoldingSegmentType,
				ScaffoldingColour = tblobjectbuilding.ScaffoldingColour,
				Colours = tblobjectbuilding.Colours,
				GeneratorFunction = tblobjectbuilding.GeneratorFunction,
				AverageNumberOnMap = tblobjectbuilding.AverageNumberOnMap,
				Id = tblobjectbuilding.Id,
			};
		}

		public static TblObjectBuilding ToTblObjectBuildingEntity(this DtoObjectBuilding model)
		{
			return new TblObjectBuilding
			{
				DesignedYear = model.DesignedYear,
				ObsoleteYear = model.ObsoleteYear,
				Flags = model.Flags,
				CostIndex = model.CostIndex,
				SellCostFactor = model.SellCostFactor,
				DemolishRatingReduction = model.DemolishRatingReduction,
				ScaffoldingSegmentType = model.ScaffoldingSegmentType,
				ScaffoldingColour = model.ScaffoldingColour,
				Colours = model.Colours,
				GeneratorFunction = model.GeneratorFunction,
				AverageNumberOnMap = model.AverageNumberOnMap,
				Id = model.Id,
			};
		}

	}
}

