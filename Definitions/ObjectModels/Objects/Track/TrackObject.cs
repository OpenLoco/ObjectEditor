using Definitions.ObjectModels.Types;

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

	public bool Validate()
	{
		if (var_06 >= 3)
		{
			return false;
		}

		// vanilla missed this check
		if (CostIndex > 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		if (BuildCostFactor <= 0)
		{
			return false;
		}

		if (TunnelCostFactor <= 0)
		{
			return false;
		}

		if (TrackPieces.HasFlag(TrackTraitFlags.Diagonal | TrackTraitFlags.LargeCurve)
			&& TrackPieces.HasFlag(TrackTraitFlags.OneSided | TrackTraitFlags.VerySmallCurve))
		{
			return false;
		}

		if (Bridges.Count > 7)
		{
			return false;
		}

		return Stations.Count <= 7;
	}
}
