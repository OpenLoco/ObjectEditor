using Definitions.ObjectModels.Objects.Climate;

namespace Definitions.Database;

public class TblObjectClimate : DbSubObject, IConvertibleToTable<TblObjectClimate, ClimateObject>
{
	public Season FirstSeason { get; set; }
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
			SeasonLength1 = obj.SeasonLength1,
			SeasonLength2 = obj.SeasonLength2,
			SeasonLength3 = obj.SeasonLength3,
			SeasonLength4 = obj.SeasonLength4,
		};
}
