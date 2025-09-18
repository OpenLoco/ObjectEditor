using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.TrackStation;

public class TrackStationObject : ILocoStruct
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_0B { get; set; }
	public TrackStationObjectFlags Flags { get; set; }
	public uint8_t var_0D { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];

	//public uint8_t[][][] CargoOffsetBytes { get; set; }
	public CargoOffset[][][] CargoOffsets { get; set; }

	public uint8_t[][] var_6E { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= 32)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be between 0 and 31 inclusive.", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"The negative of {nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be positive.", [nameof(BuildCostFactor)]);
		}

		if (PaintStyle >= 1)
		{
			yield return new ValidationResult($"{nameof(PaintStyle)} must be 0.", [nameof(PaintStyle)]);
		}

		if (CompatibleTrackObjects.Count > 7 /*TrackStationObjectLoader.Constants.MaxNumCompatible*/)
		{
			yield return new ValidationResult($"{nameof(CompatibleTrackObjects)} must have at most 7 entries.", [nameof(CompatibleTrackObjects)]);
		}
	}
}
