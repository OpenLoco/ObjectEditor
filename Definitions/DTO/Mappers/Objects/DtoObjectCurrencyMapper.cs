using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectCurrencyMapper
	{
		public static DtoObjectCurrency ToDto(this TblObjectCurrency tblobjectcurrency)
		{
			return new DtoObjectCurrency
			{
				Separator = tblobjectcurrency.Separator,
				Factor = tblobjectcurrency.Factor,
				Id = tblobjectcurrency.Id,
			};
		}

		public static TblObjectCurrency ToTblObjectCurrencyEntity(this DtoObjectCurrency model)
		{
			return new TblObjectCurrency
			{
				Separator = model.Separator,
				Factor = model.Factor,
				Id = model.Id,
			};
		}

	}
}

