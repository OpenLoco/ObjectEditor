using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectLandMapper
	{
		public static DtoObjectLand ToDto(this TblObjectLand tblobjectland) => new()
		{
			CostIndex = tblobjectland.CostIndex,
			NumGrowthStages = tblobjectland.NumGrowthStages,
			NumImageAngles = tblobjectland.NumImageAngles,
			Flags = tblobjectland.Flags,
			CostFactor = tblobjectland.CostFactor,
			NumImagesPerGrowthStage = tblobjectland.NumImagesPerGrowthStage,
			DistributionPattern = tblobjectland.DistributionPattern,
			NumVariations = tblobjectland.NumVariations,
			VariationLikelihood = tblobjectland.VariationLikelihood,
			Id = tblobjectland.Id,
		};

		public static TblObjectLand ToTblObjectLandEntity(this DtoObjectLand model, TblObject parent) => new()
		{
			Parent = parent,
			CostIndex = model.CostIndex,
			NumGrowthStages = model.NumGrowthStages,
			NumImageAngles = model.NumImageAngles,
			Flags = model.Flags,
			CostFactor = model.CostFactor,
			NumImagesPerGrowthStage = model.NumImagesPerGrowthStage,
			DistributionPattern = model.DistributionPattern,
			NumVariations = model.NumVariations,
			VariationLikelihood = model.VariationLikelihood,
			Id = model.Id,
		};

	}
}

