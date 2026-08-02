using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectCliffEdgeMapper
{
	public static DtoObjectCliffEdge ToDto(this TblObjectCliffEdge tblobjectcliffedge) => new()
	{
		Id = tblobjectcliffedge.Id,
	};

	public static TblObjectCliffEdge ToTblObjectCliffEdgeEntity(this DtoObjectCliffEdge model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

