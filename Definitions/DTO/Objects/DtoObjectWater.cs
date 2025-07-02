namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectWater : IHasId
	{
		public uint8_t CostIndex { get; set; }
		public int16_t CostFactor { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
