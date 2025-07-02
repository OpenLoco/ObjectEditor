using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectRegionMapper
	{
		public static DtoObjectRegion ToDto(this TblObjectRegion tblobjectregion)
		{
			return new DtoObjectRegion
			{
				Id = tblobjectregion.Id,
			};
		}

		public static TblObjectRegion ToTblObjectRegionEntity(this DtoObjectRegion model)
		{
			return new TblObjectRegion
			{
				Id = model.Id,
			};
		}

	}
}

