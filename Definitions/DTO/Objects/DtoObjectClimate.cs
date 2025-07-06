using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectClimate : IDtoSubObject
	{
		public uint8_t FirstSeason { get; set; }
		public uint8_t WinterSnowLine { get; set; }
		public uint8_t SummerSnowLine { get; set; }
		public uint8_t SeasonLength1 { get; set; }
		public uint8_t SeasonLength2 { get; set; }
		public uint8_t SeasonLength3 { get; set; }
		public uint8_t SeasonLength4 { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
