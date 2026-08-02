using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectScenarioTextMapper
{
	public static DtoObjectScenarioText ToDto(this TblObjectScenarioText tblobjectscenariotext) => new()
	{
		Id = tblobjectscenariotext.Id,
	};

	public static TblObjectScenarioText ToTblObjectScenarioTextEntity(this DtoObjectScenarioText model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

