using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectSnowMapper
{
	public static DtoObjectSnow ToDto(this TblObjectSnow tblobjectsnow) => new()
	{
		Id = tblobjectsnow.Id,
	};

	public static TblObjectSnow ToTblObjectSnowEntity(this DtoObjectSnow model, TblObject parent) => new()
	{
		Parent = parent,
		Id = model.Id,
	};

}

