using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectRegionMapper
	{
		public static DtoObjectRegion ToDto(this TblObjectRegion tblobjectregion) => new()
		{
			Id = tblobjectregion.Id,
		};

		public static TblObjectRegion ToTblObjectRegionEntity(this DtoObjectRegion model, TblObject parent) => new()
		{
			Parent = parent,
			Id = model.Id,
		};

	}
}

