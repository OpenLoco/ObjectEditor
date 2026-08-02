using Definitions.Database.Base;

namespace Definitions.DTO.Objects;

public class DtoObjectRoadExtra : IDtoSubObject
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public UniqueObjectId Id { get; set; }
}
