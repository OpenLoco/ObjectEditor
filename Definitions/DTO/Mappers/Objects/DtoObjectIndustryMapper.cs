using Definitions.Database;

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
		BuildingComponents = tblobjectindustry.BuildingComponents,
		AnimationSequences = tblobjectindustry.AnimationSequences,
		RandomAnimations = tblobjectindustry.RandomAnimations,
		InitialProductionRate = tblobjectindustry.InitialProductionRate,
		ProducedCargo = tblobjectindustry.ProducedCargo,
		RequiredCargo = tblobjectindustry.RequiredCargo,
		NumFarmTileImages = tblobjectindustry.NumFarmTileImages,
		WallTypes = tblobjectindustry.WallTypes,
		BuildingWall = tblobjectindustry.BuildingWall,
		BuildingWallEntrance = tblobjectindustry.BuildingWallEntrance,
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
		BuildingComponents = model.BuildingComponents,
		AnimationSequences = model.AnimationSequences,
		RandomAnimations = model.RandomAnimations,
		InitialProductionRate = model.InitialProductionRate,
		ProducedCargo = model.ProducedCargo,
		RequiredCargo = model.RequiredCargo,
		NumFarmTileImages = model.NumFarmTileImages,
		WallTypes = model.WallTypes,
		BuildingWall = model.BuildingWall,
		BuildingWallEntrance = model.BuildingWallEntrance,
		Buildings = model.Buildings,
		Id = model.Id,
	};

}

