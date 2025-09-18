using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.RoadStation;

public class RoadStationObject : ILocoStruct
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public RoadStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public List<ObjectModelHeader> CompatibleRoadObjects { get; set; } = [];

	public ObjectModelHeader? CargoType { get; set; }

	//public uint8_t[][][] CargoOffsetBytes { get; set; }
	public CargoOffset[][][] CargoOffsets { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (SellCostFactor >= 0)
		{
			yield return new ValidationResult($"{nameof(SellCostFactor)} must be less than 0 {nameof(SellCostFactor)}", [nameof(SellCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0", [nameof(BuildCostFactor)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"-{nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (PaintStyle >= 1)
		{
			yield return new ValidationResult($"{nameof(PaintStyle)} must be 0.", [nameof(PaintStyle)]);
		}

		if (CompatibleRoadObjects.Count > 7)
		{
			yield return new ValidationResult($"{nameof(CompatibleRoadObjects)} must have at most 7 entries.", [nameof(CompatibleRoadObjects)]);
		}

		if (Flags.HasFlag(RoadStationObjectFlags.Passenger) && Flags.HasFlag(RoadStationObjectFlags.Freight))
		{
			yield return new ValidationResult($"Only one of {nameof(RoadStationObjectFlags.Passenger)} or {nameof(RoadStationObjectFlags.Freight)} can be set.", [nameof(Flags)]);
		}
	}
}
