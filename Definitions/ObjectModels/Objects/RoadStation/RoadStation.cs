using Definitions.ObjectModels.Objects.Road;

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
	public uint8_t CompatibleRoadObjectCount { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
