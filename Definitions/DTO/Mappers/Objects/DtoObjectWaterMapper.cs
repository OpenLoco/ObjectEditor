using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectWaterMapper
{
	public static DtoObjectWater ToDto(this TblObjectWater tblobjectwater) => new()
	{
		CostIndex = tblobjectwater.CostIndex,
		CostFactor = tblobjectwater.CostFactor,
		Id = tblobjectwater.Id,
	};

	public static TblObjectWater ToTblObjectWaterEntity(this DtoObjectWater model, TblObject parent) => new()
	{
		Parent = parent,
		CostIndex = model.CostIndex,
		CostFactor = model.CostFactor,
		Id = model.Id,
	};

}

