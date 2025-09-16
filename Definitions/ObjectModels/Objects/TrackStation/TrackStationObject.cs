using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;

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

	public bool Validate()
	{
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

		if (PaintStyle >= 1)
		{
			return false;
		}

		return true; // CompatibleTrackObjects.Count <= TrackStationObjectLoader.Constants.MaxNumCompatible;
	}
}
