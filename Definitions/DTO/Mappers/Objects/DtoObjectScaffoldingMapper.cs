using Definitions.Database;

namespace Definitions.DTO.Mappers
{
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
}

