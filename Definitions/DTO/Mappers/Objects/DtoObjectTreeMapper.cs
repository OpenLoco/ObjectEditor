using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectTreeMapper
	{
		public static DtoObjectTree ToDto(this TblObjectTree tblobjecttree) => new()
		{
			Clearance = tblobjecttree.Clearance,
			Height = tblobjecttree.Height,
			NumRotations = tblobjecttree.NumRotations,
			NumGrowthStages = tblobjecttree.NumGrowthStages,
			Flags = tblobjecttree.Flags,
			ShadowImageOffset = tblobjecttree.ShadowImageOffset,
			SeasonState = tblobjecttree.SeasonState,
			Season = tblobjecttree.Season,
			CostIndex = tblobjecttree.CostIndex,
			BuildCostFactor = tblobjecttree.BuildCostFactor,
			ClearCostFactor = tblobjecttree.ClearCostFactor,
			Colours = tblobjecttree.Colours,
			Rating = tblobjecttree.Rating,
			DemolishRatingReduction = tblobjecttree.DemolishRatingReduction,
			Id = tblobjecttree.Id,
		};

		public static TblObjectTree ToTblObjectTreeEntity(this DtoObjectTree model, TblObject parent) => new()
		{
			Parent = parent,
			Clearance = model.Clearance,
			Height = model.Height,
			NumRotations = model.NumRotations,
			NumGrowthStages = model.NumGrowthStages,
			Flags = model.Flags,
			ShadowImageOffset = model.ShadowImageOffset,
			SeasonState = model.SeasonState,
			Season = model.Season,
			CostIndex = model.CostIndex,
			BuildCostFactor = model.BuildCostFactor,
			ClearCostFactor = model.ClearCostFactor,
			Colours = model.Colours,
			Rating = model.Rating,
			DemolishRatingReduction = model.DemolishRatingReduction,
			Id = model.Id,
		};

	}
}

