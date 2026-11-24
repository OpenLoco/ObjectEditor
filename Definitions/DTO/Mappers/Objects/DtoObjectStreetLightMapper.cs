using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectStreetLightMapper
{
	public static DtoObjectStreetLight ToDto(this TblObjectStreetLight tblobjectstreetlight) => new()
	{
		DesignedYears = tblobjectstreetlight.DesignedYears,
		Id = tblobjectstreetlight.Id,
	};

	public static TblObjectStreetLight ToTblObjectStreetLightEntity(this DtoObjectStreetLight model, TblObject parent) => new()
	{
		Parent = parent,
		DesignedYears = model.DesignedYears,
		Id = model.Id,
	};

}

