using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectRegionMapper
{
	public static DtoObjectRegion ToDto(this TblObjectRegion tblobjectregion) => new()
	{
		Id = tblobjectregion.Id,
	};

	public static TblObjectRegion ToTblObjectRegionEntity(this DtoObjectRegion model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

