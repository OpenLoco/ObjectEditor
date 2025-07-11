using Definitions.Database;

namespace Definitions.DTO.Mappers
{
	public static class DtoObjectTownNamesMapper
	{
		public static DtoObjectTownNames ToDto(this TblObjectTownNames tblobjecttownnames) => new()
		{
			Id = tblobjecttownnames.Id,
		};

		public static TblObjectTownNames ToTblObjectTownNamesEntity(this DtoObjectTownNames model, TblObject parent) => new()
		{
			Parent = parent,
			Id = model.Id,
		};

	}
}

