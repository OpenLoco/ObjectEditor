using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectSteamMapper
	{
		public static DtoObjectSteam ToDto(this TblObjectSteam tblobjectsteam)
		{
			return new DtoObjectSteam
			{
				NumStationaryTicks = tblobjectsteam.NumStationaryTicks,
				SpriteWidth = tblobjectsteam.SpriteWidth,
				SpriteHeightNegative = tblobjectsteam.SpriteHeightNegative,
				SpriteHeightPositive = tblobjectsteam.SpriteHeightPositive,
				Flags = tblobjectsteam.Flags,
				Id = tblobjectsteam.Id,
			};
		}

		public static TblObjectSteam ToTblObjectSteamEntity(this DtoObjectSteam model)
		{
			return new TblObjectSteam
			{
				NumStationaryTicks = model.NumStationaryTicks,
				SpriteWidth = model.SpriteWidth,
				SpriteHeightNegative = model.SpriteHeightNegative,
				SpriteHeightPositive = model.SpriteHeightPositive,
				Flags = model.Flags,
				Id = model.Id,
			};
		}

	}
}

