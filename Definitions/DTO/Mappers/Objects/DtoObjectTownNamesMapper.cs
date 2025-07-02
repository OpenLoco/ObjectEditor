using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectTownNamesMapper
	{
		public static DtoObjectTownNames ToDto(this TblObjectTownNames tblobjecttownnames)
		{
			return new DtoObjectTownNames
			{
				Id = tblobjecttownnames.Id,
			};
		}

		public static TblObjectTownNames ToTblObjectTownNamesEntity(this DtoObjectTownNames model)
		{
			return new TblObjectTownNames
			{
				Id = model.Id,
			};
		}

	}
}

