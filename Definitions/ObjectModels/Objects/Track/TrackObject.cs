using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Track;

public class TrackObject : ILocoStruct
{
	public TrackTraitFlags TrackPieces { get; set; }
	public TrackTraitFlags StationTrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 MaxCurveSpeed { get; set; }
	public TrackObjectFlags Flags { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public uint8_t var_06 { get; set; }

	public List<ObjectModelHeader> CompatibleTracksAndRoads { get; set; } = [];
	public List<ObjectModelHeader> TrackMods { get; set; } = []; // aka TrackExtraObject
	public List<ObjectModelHeader> Signals { get; set; } = [];
	public ObjectModelHeader Tunnel { get; set; }
	public List<ObjectModelHeader> Bridges { get; set; } = [];
	public List<ObjectModelHeader> Stations { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (var_06 >= 3)
		{
			yield return new ValidationResult($"{nameof(var_06)} must be 0, 1, or 2.", [nameof(var_06)]);
		}

		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"The negative of {nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be positive.", [nameof(BuildCostFactor)]);
		}

		if (TunnelCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(TunnelCostFactor)} must be positive.", [nameof(TunnelCostFactor)]);
		}

		if (TrackPieces.HasFlag(TrackTraitFlags.Diagonal | TrackTraitFlags.LargeCurve) && TrackPieces.HasFlag(TrackTraitFlags.OneSided | TrackTraitFlags.VerySmallCurve))
		{
			yield return new ValidationResult($"{nameof(TrackPieces)} cannot include both {TrackTraitFlags.Diagonal} or {TrackTraitFlags.LargeCurve} and {TrackTraitFlags.OneSided} or {TrackTraitFlags.VerySmallCurve}.", [nameof(TrackPieces)]);
		}

		if (Bridges.Count > 7)
		{
			yield return new ValidationResult($"{nameof(Bridges)} can contain at most 7 entries.", [nameof(Bridges)]);
		}

		if (Stations.Count > 7)
		{
			yield return new ValidationResult($"{nameof(Stations)} can contain at most 7 entries.", [nameof(Stations)]);
		}
	}
}
