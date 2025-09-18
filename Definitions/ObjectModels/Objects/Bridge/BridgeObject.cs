using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Bridge;

public class BridgeObject : ILocoStruct
{
	public BridgeObjectFlags Flags { get; set; }
	public uint16_t ClearHeight { get; set; }
	public int16_t DeckDepth { get; set; }
	public uint8_t SpanLength { get; set; }
	public SupportPillarSpacing PillarSpacing { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public MicroZ MaxHeight { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BaseCostFactor { get; set; }
	public int16_t HeightCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint16_t DesignedYear { get; set; }
	public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
	public uint8_t var_03 { get; set; } // unknown

	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];
	public List<ObjectModelHeader> CompatibleRoadObjects { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BaseCostFactor)
		{
			yield return new ValidationResult($"The negative of {nameof(SellCostFactor)} must be less than or equal to {nameof(BaseCostFactor)}.", [nameof(SellCostFactor), nameof(BaseCostFactor)]);
		}

		if (BaseCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BaseCostFactor)} must be positive.", [nameof(BaseCostFactor)]);
		}

		if (HeightCostFactor < 0)
		{
			yield return new ValidationResult($"{nameof(HeightCostFactor)} must be non-negative.", [nameof(HeightCostFactor)]);
		}

		if (DeckDepth is not 16 and not 32)
		{
			yield return new ValidationResult($"{nameof(DeckDepth)} must be either 16 or 32.", [nameof(DeckDepth)]);
		}

		if (SpanLength is not 1 and not 2 and not 4)
		{
			yield return new ValidationResult($"{nameof(SpanLength)} must be either 1, 2, or 4.", [nameof(SpanLength)]);
		}

		if (CompatibleTrackObjects.Count > 7)
		{
			yield return new ValidationResult($"{nameof(CompatibleTrackObjects)} must contain at most 7 entries.", [nameof(CompatibleTrackObjects)]);
		}

		if (CompatibleRoadObjects.Count > 7)
		{
			yield return new ValidationResult($"{nameof(CompatibleRoadObjects)} must contain at most 7 entries.", [nameof(CompatibleRoadObjects)]);
		}
	}
}
