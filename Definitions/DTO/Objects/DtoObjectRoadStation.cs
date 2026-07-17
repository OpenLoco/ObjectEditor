using Definitions.Database;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;

namespace Definitions.DTO.Objects;

public class DtoObjectRoadStation : IDtoSubObject
{
	public UniqueObjectId Id { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public RoadStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint8_t pad_2D { get; set; }
	public string CargoOffsets { get; set; } = "[]";
}