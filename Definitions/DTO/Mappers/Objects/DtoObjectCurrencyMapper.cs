using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectCurrencyMapper
{
	public static DtoObjectCurrency ToDto(this TblObjectCurrency tblobjectcurrency) => new()
	{
		Separator = tblobjectcurrency.Separator,
		Factor = tblobjectcurrency.Factor,
		Id = tblobjectcurrency.Id,
	};

	public static TblObjectCurrency ToTblObjectCurrencyEntity(this DtoObjectCurrency model, TblObject parent) => new()
	{
		Parent = parent,
		Separator = model.Separator,
		Factor = model.Factor,
		Id = model.Id,
	};

}

