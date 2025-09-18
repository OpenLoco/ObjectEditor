using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Road;

public class RoadObject : ILocoStruct
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t MaxCurveSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public TownSize TargetTownSize { get; set; }

	public List<ObjectModelHeader> CompatibleTracksAndRoads { get; set; } = [];
	public List<ObjectModelHeader> RoadMods { get; set; } = [];
	public ObjectModelHeader Tunnel { get; set; }
	public List<ObjectModelHeader> Bridges { get; set; } = [];
	public List<ObjectModelHeader> Stations { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= 32)
		{
			yield return new ValidationResult("CostIndex must be less than 32", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult("SellCostFactor must not be less than negative BuildCostFactor", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult("BuildCostFactor must be greater than 0", [nameof(BuildCostFactor)]);
		}

		if (TunnelCostFactor <= 0)
		{
			yield return new ValidationResult("TunnelCostFactor must be greater than 0", [nameof(TunnelCostFactor)]);
		}

		if (Bridges.Count > 7)
		{
			yield return new ValidationResult("Bridges.Count must be 7 or less", [nameof(Bridges)]);
		}

		if (RoadMods.Count > 2)
		{
			yield return new ValidationResult("RoadMods.Count must be 2 or less", [nameof(RoadMods)]);
		}

		if (Flags.HasFlag(RoadObjectFlags.unk_03) && RoadMods.Count != 0)
		{
			yield return new ValidationResult("If unk_03 flag is set, RoadMods.Count must be 0", [nameof(Flags), nameof(RoadMods)]);
		}
	}
}
