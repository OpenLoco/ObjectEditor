using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;

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

		if (CompatibleRoadObjects.Count > 7)
		{
			return false;
		}

		if (Flags.HasFlag(RoadStationObjectFlags.Passenger) && Flags.HasFlag(RoadStationObjectFlags.Freight))
		{
			return false;
		}

		return true;
	}
}
