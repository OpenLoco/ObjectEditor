using Definitions.Database.DataTables;
using Definitions.Database.DataTables.Objects;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers.Objects;

public static class DtoObjectScaffoldingMapper
{
	public static DtoObjectScaffolding ToDto(this TblObjectScaffolding tblobjectscaffolding) => new()
	{
		Id = tblobjectscaffolding.Id,
	};

	public static TblObjectScaffolding ToTblObjectScaffoldingEntity(this DtoObjectScaffolding model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

