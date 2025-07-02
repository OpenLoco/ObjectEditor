using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectSoundMapper
	{
		public static DtoObjectSound ToDto(this TblObjectSound tblobjectsound)
		{
			return new DtoObjectSound
			{
				ShouldLoop = tblobjectsound.ShouldLoop,
				Volume = tblobjectsound.Volume,
				Id = tblobjectsound.Id,
			};
		}

		public static TblObjectSound ToTblObjectSoundEntity(this DtoObjectSound model)
		{
			return new TblObjectSound
			{
				ShouldLoop = model.ShouldLoop,
				Volume = model.Volume,
				Id = model.Id,
			};
		}

	}
}

