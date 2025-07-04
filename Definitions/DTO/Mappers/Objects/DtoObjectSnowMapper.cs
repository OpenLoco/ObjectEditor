using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
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
}

