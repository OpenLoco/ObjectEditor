using Definitions.Database;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers;

public static class DtoObjectIndustryMapper
{
	public static DtoObjectIndustry ToDto(this TblObjectIndustry tblobjectindustry) => new()
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
		FarmNumFields = tblobjectindustry.FarmNumFields,
		FarmNumStagesOfGrowth = tblobjectindustry.FarmNumStagesOfGrowth,
		MonthlyClosureChance = tblobjectindustry.MonthlyClosureChance,
		var_E8 = tblobjectindustry.var_E8,
		BuildingComponents = tblobjectindustry.BuildingComponents,
		AnimationSequences = tblobjectindustry.AnimationSequences,
		var_38 = tblobjectindustry.var_38,
		InitialProductionRate = tblobjectindustry.InitialProductionRate,
		Buildings = tblobjectindustry.Buildings,
		Id = tblobjectindustry.Id,
	};

	public static TblObjectIndustry ToTblObjectIndustryEntity(this DtoObjectIndustry model, TblObject parent) => new()
	{
		Parent = parent,
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
		FarmNumFields = model.FarmNumFields,
		FarmNumStagesOfGrowth = model.FarmNumStagesOfGrowth,
		MonthlyClosureChance = model.MonthlyClosureChance,
		var_E8 = model.var_E8,
		BuildingComponents = model.BuildingComponents,
		AnimationSequences = model.AnimationSequences,
		var_38 = model.var_38,
		InitialProductionRate = model.InitialProductionRate,
		Buildings = model.Buildings,
		Id = model.Id,
	};
}