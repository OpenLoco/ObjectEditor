using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectStreetLightMapper
	{
		public static DtoObjectStreetLight ToDto(this TblObjectStreetLight tblobjectstreetlight) => new()
		{
			Id = tblobjectstreetlight.Id,
		};

		public static TblObjectStreetLight ToTblObjectStreetLightEntity(this DtoObjectStreetLight model, TblObject parent) => new()
		{
			Parent = parent,
			Id = model.Id,
		};

	}
}

