using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

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

