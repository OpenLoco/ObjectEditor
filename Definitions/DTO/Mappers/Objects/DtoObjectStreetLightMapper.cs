using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectStreetLightMapper
	{
		public static DtoObjectStreetLight ToDto(this TblObjectStreetLight tblobjectstreetlight)
		{
			return new DtoObjectStreetLight
			{
				Id = tblobjectstreetlight.Id,
			};
		}

		public static TblObjectStreetLight ToTblObjectStreetLightEntity(this DtoObjectStreetLight model)
		{
			return new TblObjectStreetLight
			{
				Id = model.Id,
			};
		}

	}
}

