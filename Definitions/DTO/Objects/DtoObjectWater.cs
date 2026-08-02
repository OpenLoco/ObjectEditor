using Definitions.Database.Base;

namespace Definitions.DTO.Objects;

public class DtoObjectWater : IDtoSubObject
{
	public uint8_t CostIndex { get; set; }
	public int16_t CostFactor { get; set; }
	public UniqueObjectId Id { get; set; }
}
