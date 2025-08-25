using Definitions.ObjectModels.Types;

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

	public bool Validate()
	{
		// check missing in vanilla
		if (CostIndex >= 32)
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

		if (Bridges.Count > 7)
		{
			return false;
		}

		if (RoadMods.Count > 2)
		{
			return false;
		}

		if (Flags.HasFlag(RoadObjectFlags.unk_03))
		{
			return RoadMods.Count == 0;
		}

		return true;
	}
}
