using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectWallMapper
{
	public static DtoObjectWall ToDto(this TblObjectWall tblobjectwall) => new()
	{
		Height = tblobjectwall.Height,
		Flags1 = tblobjectwall.Flags1,
		ToolId = tblobjectwall.ToolId,
		Flags2 = tblobjectwall.Flags2,
		Id = tblobjectwall.Id,
	};

	public static TblObjectWall ToTblObjectWallEntity(this DtoObjectWall model, TblObject parent) => new()
	{
		Parent = parent,
		Height = model.Height,
		Flags1 = model.Flags1,
		ToolId = model.ToolId,
		Flags2 = model.Flags2,
		Id = model.Id,
	};

}

