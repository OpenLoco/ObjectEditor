using Definitions.Database;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectRoad : IDtoSubObject
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 MaxCurveSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t VehicleDisplayListVerticalOffset { get; set; }
	public TownSize TargetTownSize { get; set; }
	public ObjectModelHeader Tunnel { get; set; } = null!;
	public List<ObjectModelHeader> Bridges { get; set; } = [];
	public List<ObjectModelHeader> Stations { get; set; } = [];
	public List<ObjectModelHeader> RoadMods { get; set; } = [];
	public List<ObjectModelHeader> TracksAndRoads { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
