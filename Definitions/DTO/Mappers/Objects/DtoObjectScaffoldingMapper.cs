using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectScaffoldingMapper
	{
		public static DtoObjectScaffolding ToDto(this TblObjectScaffolding tblobjectscaffolding)
		{
			return new DtoObjectScaffolding
			{
				Id = tblobjectscaffolding.Id,
			};
		}

		public static TblObjectScaffolding ToTblObjectScaffoldingEntity(this DtoObjectScaffolding model)
		{
			return new TblObjectScaffolding
			{
				Id = model.Id,
			};
		}

	}
}

