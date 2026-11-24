using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectTreeMapper
{
	public static DtoObjectTree ToDto(this TblObjectTree tblobjecttree) => new()
	{
		InitialHeight = tblobjecttree.InitialHeight,
		Height = tblobjecttree.Height,
		NumRotations = tblobjecttree.NumRotations,
		NumGrowthStages = tblobjecttree.NumGrowthStages,
		Flags = tblobjecttree.Flags,
		ShadowImageOffset = tblobjecttree.ShadowImageOffset,
		SeasonState = tblobjecttree.SeasonState,
		CurrentSeason = tblobjecttree.CurrentSeason,
		CostIndex = tblobjecttree.CostIndex,
		BuildCostFactor = tblobjecttree.BuildCostFactor,
		ClearCostFactor = tblobjecttree.ClearCostFactor,
		Colours = tblobjecttree.Colours,
		Rating = tblobjecttree.Rating,
		DemolishRatingReduction = tblobjecttree.DemolishRatingReduction,
		MinHeight = tblobjecttree.MinHeight,
		MaxHeight = tblobjecttree.MaxHeight,
		VariantFlags = tblobjecttree.VariantFlags,
		Id = tblobjecttree.Id,
	};

	public static TblObjectTree ToTblObjectTreeEntity(this DtoObjectTree model, TblObject parent) => new()
	{
		Parent = parent,
		InitialHeight = model.InitialHeight,
		Height = model.Height,
		NumRotations = model.NumRotations,
		NumGrowthStages = model.NumGrowthStages,
		Flags = model.Flags,
		ShadowImageOffset = model.ShadowImageOffset,
		SeasonState = model.SeasonState,
		CurrentSeason = model.CurrentSeason,
		CostIndex = model.CostIndex,
		BuildCostFactor = model.BuildCostFactor,
		ClearCostFactor = model.ClearCostFactor,
		Colours = model.Colours,
		Rating = model.Rating,
		DemolishRatingReduction = model.DemolishRatingReduction,
		MinHeight = model.MinHeight,
		MaxHeight = model.MaxHeight,
		VariantFlags = model.VariantFlags,
		Id = model.Id,
	};

}

