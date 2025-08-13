using Definitions.ObjectModels.Objects.Cliff;

namespace Definitions.ObjectModels.Objects.Land;

public class LandObject : ILocoStruct
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

	public CliffEdgeObject? CliffEdgeHeader1 { get; set; }
	public CliffEdgeObject? CliffEdgeHeader2 { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
