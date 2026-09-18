using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectBuildingMapper
{
	public static DtoObjectBuilding ToDto(this TblObjectBuilding tblobjectbuilding) => new()
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
		BuildingComponents = tblobjectbuilding.BuildingComponents,
		ProducedQuantity = tblobjectbuilding.ProducedQuantity,
		ProducedCargoType = tblobjectbuilding.ProducedCargoType,
		ConsumedCargoType = tblobjectbuilding.ConsumedCargoType,
		ProducedCargoQuantity = tblobjectbuilding.ProducedCargoQuantity,
		ConsumedCargoQuantity = tblobjectbuilding.ConsumedCargoQuantity,
		TownAmenityCategory = tblobjectbuilding.TownAmenityCategory,
		ElevatorHeightSequences = tblobjectbuilding.ElevatorHeightSequences,
		Id = tblobjectbuilding.Id,
	};

	public static TblObjectBuilding ToTblObjectBuildingEntity(this DtoObjectBuilding model, TblObject parent) => new()
	{
		Parent = parent,
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
		BuildingComponents = model.BuildingComponents,
		ProducedQuantity = model.ProducedQuantity,
		ProducedCargoType = model.ProducedCargoType,
		ConsumedCargoType = model.ConsumedCargoType,
		ProducedCargoQuantity = model.ProducedCargoQuantity,
		ConsumedCargoQuantity = model.ConsumedCargoQuantity,
		TownAmenityCategory = model.TownAmenityCategory,
		ElevatorHeightSequences = model.ElevatorHeightSequences,
		Id = model.Id,
	};

}

