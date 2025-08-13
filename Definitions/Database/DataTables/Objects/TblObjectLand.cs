using Definitions.ObjectModels.Objects.Land;

namespace Definitions.Database;

public class TblObjectLand : DbSubObject, IConvertibleToTable<TblObjectLand, LandObject>
{
	public uint8_t CostIndex { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public uint8_t NumImageAngles { get; set; }
	public LandObjectFlags Flags { get; set; }
	public int16_t CostFactor { get; set; }
	public uint32_t NumImagesPerGrowthStage { get; set; }
	public uint8_t DistributionPattern { get; set; }
	public uint8_t NumVariations { get; set; }
	public uint8_t VariationLikelihood { get; set; }

	//public TblObjectCliffEdge CliffEdgeHeader1 { get; set; }
	//public TblObjectCliffEdge CliffEdgeHeader2 { get; set; }

	public static TblObjectLand FromObject(TblObject tbl, LandObject obj)
		=> new()
		{
			Parent = tbl,
			CostIndex = obj.CostIndex,
			NumGrowthStages = obj.NumGrowthStages,
			NumImageAngles = obj.NumImageAngles,
			Flags = obj.Flags,
			CostFactor = obj.CostFactor,
			NumImagesPerGrowthStage = obj.NumImagesPerGrowthStage,
			DistributionPattern = obj.DistributionPattern,
			NumVariations = obj.NumVariations,
			VariationLikelihood = obj.VariationLikelihood,
			//CliffEdgeHeader1 = obj.CliffEdgeHeader1,
			//CliffEdgeHeader2 = obj.CliffEdgeHeader2,
		};
}
