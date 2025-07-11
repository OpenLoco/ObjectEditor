using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectRoad : IDtoSubObject
	{
		public RoadTraitFlags RoadPieces { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public int16_t TunnelCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public RoadObjectFlags Flags { get; set; }
		public uint8_t PaintStyle { get; set; }
		public uint8_t DisplayOffset { get; set; }
		public TownSize TargetTownSize { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
