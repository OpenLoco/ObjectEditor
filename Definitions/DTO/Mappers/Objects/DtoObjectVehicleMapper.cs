using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectVehicleMapper
{
	public static DtoObjectVehicle ToDto(this TblObjectVehicle tblobjectvehicle) => new()
	{
		Mode = tblobjectvehicle.Mode,
		Type = tblobjectvehicle.Type,
		NumCarComponents = tblobjectvehicle.NumCarComponents,
		TrackTypeId = tblobjectvehicle.TrackTypeId,
		NumRequiredTrackExtras = tblobjectvehicle.NumRequiredTrackExtras,
		CostIndex = tblobjectvehicle.CostIndex,
		CostFactor = tblobjectvehicle.CostFactor,
		Reliability = tblobjectvehicle.Reliability,
		RunCostIndex = tblobjectvehicle.RunCostIndex,
		RunCostFactor = tblobjectvehicle.RunCostFactor,
		NumCompatibleVehicles = tblobjectvehicle.NumCompatibleVehicles,
		Power = tblobjectvehicle.Power,
		Speed = tblobjectvehicle.Speed,
		RackSpeed = tblobjectvehicle.RackSpeed,
		Weight = tblobjectvehicle.Weight,
		Flags = tblobjectvehicle.Flags,
		ShipWakeSpacing = tblobjectvehicle.ShipWakeSpacing,
		DesignedYear = tblobjectvehicle.DesignedYear,
		ObsoleteYear = tblobjectvehicle.ObsoleteYear,
		DrivingSoundType = tblobjectvehicle.DrivingSoundType,
		Id = tblobjectvehicle.Id,
	};

	public static TblObjectVehicle ToTblObjectVehicleEntity(this DtoObjectVehicle model, TblObject parent) => new()
	{
		Parent = parent,
		Mode = model.Mode,
		Type = model.Type,
		NumCarComponents = model.NumCarComponents,
		TrackTypeId = model.TrackTypeId,
		NumRequiredTrackExtras = model.NumRequiredTrackExtras,
		CostIndex = model.CostIndex,
		CostFactor = model.CostFactor,
		Reliability = model.Reliability,
		RunCostIndex = model.RunCostIndex,
		RunCostFactor = model.RunCostFactor,
		NumCompatibleVehicles = model.NumCompatibleVehicles,
		Power = model.Power,
		Speed = model.Speed,
		RackSpeed = model.RackSpeed,
		Weight = model.Weight,
		Flags = model.Flags,
		ShipWakeSpacing = model.ShipWakeSpacing,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		DrivingSoundType = model.DrivingSoundType,
		Id = model.Id,
	};

}

