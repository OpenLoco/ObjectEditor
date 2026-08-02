using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectWallMapper
{
	public static DtoObjectWall ToDto(this TblObjectWall tblobjectwall) => new()
	{
		Height = tblobjectwall.Height,
		Flags1 = tblobjectwall.Flags1,
		Id = tblobjectwall.Id,
	};

	public static TblObjectWall ToTblObjectWallEntity(this DtoObjectWall model, TblObject parent) => new()
	{
		Parent = parent,
		Height = model.Height,
		Flags1 = model.Flags1,
		Id = model.Id,
	};

}

