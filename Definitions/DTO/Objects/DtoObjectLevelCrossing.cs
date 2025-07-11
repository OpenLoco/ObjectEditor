using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectLevelCrossing : IDtoSubObject
	{
		public int16_t CostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint8_t AnimationSpeed { get; set; }
		public uint8_t ClosingFrames { get; set; }
		public uint8_t ClosedFrames { get; set; }
		public uint16_t DesignedYear { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
