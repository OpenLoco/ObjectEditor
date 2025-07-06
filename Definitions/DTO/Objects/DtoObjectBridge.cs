using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectBridge : IDtoSubObject
	{
		public BridgeObjectFlags Flags { get; set; }
		public uint16_t ClearHeight { get; set; }
		public int16_t DeckDepth { get; set; }
		public uint8_t SpanLength { get; set; }
		public uint8_t PillarSpacing { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public MicroZ MaxHeight { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BaseCostFactor { get; set; }
		public int16_t HeightCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint16_t DesignedYear { get; set; }
		public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
