using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectRegionMapper
{
	public static DtoObjectRegion ToDto(this TblObjectRegion tblobjectregion) => new()
	{
		VehicleDrivingSide = tblobjectregion.VehicleDrivingSide,
		CargoInfluenceObjects = tblobjectregion.CargoInfluenceObjects,
		DependentObjects = tblobjectregion.DependentObjects,
		CargoInfluenceTownFilter = tblobjectregion.CargoInfluenceTownFilter,
		Id = tblobjectregion.Id,
	};

	public static TblObjectRegion ToTblObjectRegionEntity(this DtoObjectRegion model, TblObject parent) => new()
	{
		Parent = parent,
		VehicleDrivingSide = model.VehicleDrivingSide,
		CargoInfluenceObjects = model.CargoInfluenceObjects,
		DependentObjects = model.DependentObjects,
		CargoInfluenceTownFilter = model.CargoInfluenceTownFilter,
		Id = model.Id,
	};

}

