using Dat.Objects;

namespace Definitions.Database;

public class TblObjectClimate : DbSubObject, IConvertibleToTable<TblObjectClimate, ClimateObject>
{
	public uint8_t FirstSeason { get; set; }
	public uint8_t WinterSnowLine { get; set; }
	public uint8_t SummerSnowLine { get; set; }
	public uint8_t SeasonLength1 { get; set; }
	public uint8_t SeasonLength2 { get; set; }
	public uint8_t SeasonLength3 { get; set; }
	public uint8_t SeasonLength4 { get; set; }

	public static TblObjectClimate FromObject(TblObject tbl, ClimateObject obj)
		=> new()
		{
			Parent = tbl,
			FirstSeason = obj.FirstSeason,
			WinterSnowLine = obj.WinterSnowLine,
			SummerSnowLine = obj.SummerSnowLine,
			SeasonLength1 = obj.SeasonLengths[0],
			SeasonLength2 = obj.SeasonLengths[1],
			SeasonLength3 = obj.SeasonLengths[2],
			SeasonLength4 = obj.SeasonLengths[3]
		};
}
