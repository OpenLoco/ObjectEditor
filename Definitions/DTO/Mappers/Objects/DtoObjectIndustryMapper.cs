using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectIndustryMapper
	{
		public static DtoObjectIndustry ToDto(this TblObjectIndustry tblobjectindustry)
		{
			return new DtoObjectIndustry
			{
				FarmImagesPerGrowthStage = tblobjectindustry.FarmImagesPerGrowthStage,
				MinNumBuildings = tblobjectindustry.MinNumBuildings,
				MaxNumBuildings = tblobjectindustry.MaxNumBuildings,
				Colours = tblobjectindustry.Colours,
				BuildingSizeFlags = tblobjectindustry.BuildingSizeFlags,
				DesignedYear = tblobjectindustry.DesignedYear,
				ObsoleteYear = tblobjectindustry.ObsoleteYear,
				TotalOfTypeInScenario = tblobjectindustry.TotalOfTypeInScenario,
				CostIndex = tblobjectindustry.CostIndex,
				BuildCostFactor = tblobjectindustry.BuildCostFactor,
				SellCostFactor = tblobjectindustry.SellCostFactor,
				ScaffoldingSegmentType = tblobjectindustry.ScaffoldingSegmentType,
				ScaffoldingColour = tblobjectindustry.ScaffoldingColour,
				MapColour = tblobjectindustry.MapColour,
				Flags = tblobjectindustry.Flags,
				FarmTileNumImageAngles = tblobjectindustry.FarmTileNumImageAngles,
				FarmGrowthStageWithNoProduction = tblobjectindustry.FarmGrowthStageWithNoProduction,
				FarmIdealSize = tblobjectindustry.FarmIdealSize,
				FarmNumStagesOfGrowth = tblobjectindustry.FarmNumStagesOfGrowth,
				MonthlyClosureChance = tblobjectindustry.MonthlyClosureChance,
				Id = tblobjectindustry.Id,
			};
		}

		public static TblObjectIndustry ToTblObjectIndustryEntity(this DtoObjectIndustry model)
		{
			return new TblObjectIndustry
			{
				FarmImagesPerGrowthStage = model.FarmImagesPerGrowthStage,
				MinNumBuildings = model.MinNumBuildings,
				MaxNumBuildings = model.MaxNumBuildings,
				Colours = model.Colours,
				BuildingSizeFlags = model.BuildingSizeFlags,
				DesignedYear = model.DesignedYear,
				ObsoleteYear = model.ObsoleteYear,
				TotalOfTypeInScenario = model.TotalOfTypeInScenario,
				CostIndex = model.CostIndex,
				BuildCostFactor = model.BuildCostFactor,
				SellCostFactor = model.SellCostFactor,
				ScaffoldingSegmentType = model.ScaffoldingSegmentType,
				ScaffoldingColour = model.ScaffoldingColour,
				MapColour = model.MapColour,
				Flags = model.Flags,
				FarmTileNumImageAngles = model.FarmTileNumImageAngles,
				FarmGrowthStageWithNoProduction = model.FarmGrowthStageWithNoProduction,
				FarmIdealSize = model.FarmIdealSize,
				FarmNumStagesOfGrowth = model.FarmNumStagesOfGrowth,
				MonthlyClosureChance = model.MonthlyClosureChance,
				Id = model.Id,
			};
		}

	}
}

