using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectSnowMapper
	{
		public static DtoObjectSnow ToDto(this TblObjectSnow tblobjectsnow)
		{
			return new DtoObjectSnow
			{
				Id = tblobjectsnow.Id,
			};
		}

		public static TblObjectSnow ToTblObjectSnowEntity(this DtoObjectSnow model)
		{
			return new TblObjectSnow
			{
				Id = model.Id,
			};
		}

	}
}

