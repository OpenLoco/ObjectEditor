using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO;

public class DtoObjectTree : IDtoSubObject
{
	public uint8_t Clearance { get; set; }
	public uint8_t Height { get; set; }
	public uint8_t NumRotations { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public TreeObjectFlags Flags { get; set; }
	public uint16_t ShadowImageOffset { get; set; }
	public uint8_t SeasonState { get; set; }
	public uint8_t Season { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t ClearCostFactor { get; set; }
	public uint32_t Colours { get; set; }
	public int16_t Rating { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public UniqueObjectId Id { get; set; }
}
