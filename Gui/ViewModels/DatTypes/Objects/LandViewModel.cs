using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;

namespace Gui.ViewModels;

public class LandViewModel : LocoObjectViewModel<LandObject>
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
	public ObjectModelHeader CliffEdgeHeader { get; set; }
	public ObjectModelHeader? UnkObjectHeader { get; set; }

	public LandViewModel(LandObject obj)
	{
		CostIndex = obj.CostIndex;
		NumGrowthStages = obj.NumGrowthStages;
		NumImageAngles = obj.NumImageAngles;
		Flags = obj.Flags;
		CostFactor = obj.CostFactor;
		NumImagesPerGrowthStage = obj.NumImagesPerGrowthStage;
		DistributionPattern = obj.DistributionPattern;
		NumVariations = obj.NumVariations;
		VariationLikelihood = obj.VariationLikelihood;
		CliffEdgeHeader = obj.CliffEdgeHeader;
		UnkObjectHeader = obj.UnkObjectHeader;
	}

	public override LandObject GetAsModel()
		=> new()
		{
			CostIndex = CostIndex,
			NumGrowthStages = NumGrowthStages,
			NumImageAngles = NumImageAngles,
			Flags = Flags,
			CostFactor = CostFactor,
			NumImagesPerGrowthStage = NumImagesPerGrowthStage,
			DistributionPattern = DistributionPattern,
			NumVariations = NumVariations,
			VariationLikelihood = VariationLikelihood,
			CliffEdgeHeader = CliffEdgeHeader,
			UnkObjectHeader = UnkObjectHeader,
		};
}
