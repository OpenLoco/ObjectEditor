using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectTrackExtra : DtoSubObject
	{
		public uint8_t PaintStyle { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
	}
}
