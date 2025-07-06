using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectClimateMapper
	{
		public static DtoObjectClimate ToDto(this TblObjectClimate tblobjectclimate) => new()
		{
			FirstSeason = tblobjectclimate.FirstSeason,
			WinterSnowLine = tblobjectclimate.WinterSnowLine,
			SummerSnowLine = tblobjectclimate.SummerSnowLine,
			SeasonLength1 = tblobjectclimate.SeasonLength1,
			SeasonLength2 = tblobjectclimate.SeasonLength2,
			SeasonLength3 = tblobjectclimate.SeasonLength3,
			SeasonLength4 = tblobjectclimate.SeasonLength4,
			Id = tblobjectclimate.Id,
		};

		public static TblObjectClimate ToTblObjectClimateEntity(this DtoObjectClimate model, TblObject parent) => new()
		{
			Parent = parent,
			FirstSeason = model.FirstSeason,
			WinterSnowLine = model.WinterSnowLine,
			SummerSnowLine = model.SummerSnowLine,
			SeasonLength1 = model.SeasonLength1,
			SeasonLength2 = model.SeasonLength2,
			SeasonLength3 = model.SeasonLength3,
			SeasonLength4 = model.SeasonLength4,
			Id = model.Id,
		};

	}
}

