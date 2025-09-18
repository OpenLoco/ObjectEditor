using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

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

	public ObjectModelHeader CliffEdgeHeader { get; set; }
	public ObjectModelHeader? UnkObjectHeader { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex > 32)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be in the range 0-32.", [nameof(CostIndex)]);
		}

		if (CostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(CostFactor)} must be positive.", [nameof(CostFactor)]);
		}

		if (NumGrowthStages < 1)
		{
			yield return new ValidationResult($"{nameof(NumGrowthStages)} must be at least 1.", [nameof(NumGrowthStages)]);
		}

		if (NumGrowthStages > 8)
		{
			yield return new ValidationResult($"{nameof(NumGrowthStages)} must be at most 8.", [nameof(NumGrowthStages)]);
		}

		if (NumImageAngles is not 1 or 2 or 4)
		{
			yield return new ValidationResult($"{nameof(NumImageAngles)} must be 1, 2, or 4.", [nameof(NumImageAngles)]);
		}
	}
}
