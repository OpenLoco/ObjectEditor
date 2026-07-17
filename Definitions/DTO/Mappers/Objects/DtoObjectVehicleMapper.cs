using Definitions.Database;
using Definitions.DTO.Objects;

namespace Definitions.DTO.Mappers;

public static class DtoObjectVehicleMapper
{
	public static DtoObjectVehicle ToDto(this TblObjectVehicle tblobjectvehicle) => new()
	{
		Mode = tblobjectvehicle.Mode,
		Type = tblobjectvehicle.Type,
		NumCarComponents = tblobjectvehicle.NumCarComponents,
		TrackTypeId = tblobjectvehicle.TrackTypeId,
		CostIndex = tblobjectvehicle.CostIndex,
		CostFactor = tblobjectvehicle.CostFactor,
		Reliability = tblobjectvehicle.Reliability,
		RunCostIndex = tblobjectvehicle.RunCostIndex,
		RunCostFactor = tblobjectvehicle.RunCostFactor,
		CompanyColourSchemeIndex = tblobjectvehicle.CompanyColourSchemeIndex,
		Power = tblobjectvehicle.Power,
		Speed = tblobjectvehicle.Speed,
		RackSpeed = tblobjectvehicle.RackSpeed,
		Weight = tblobjectvehicle.Weight,
		Flags = tblobjectvehicle.Flags,
		ShipWakeSpacing = tblobjectvehicle.ShipWakeSpacing,
		DesignedYear = tblobjectvehicle.DesignedYear,
		ObsoleteYear = tblobjectvehicle.ObsoleteYear,
		DrivingSoundType = tblobjectvehicle.DrivingSoundType,
		CarComponents = tblobjectvehicle.CarComponents,
		BodySprites = tblobjectvehicle.BodySprites,
		BogieSprites = tblobjectvehicle.BogieSprites,
		MaxCargo = tblobjectvehicle.MaxCargo,
		CompatibleCargoCategories = tblobjectvehicle.CompatibleCargoCategories,
		CargoTypeSpriteOffsets = tblobjectvehicle.CargoTypeSpriteOffsets,
		Id = tblobjectvehicle.Id,
	};

	public static TblObjectVehicle ToTblObjectVehicleEntity(this DtoObjectVehicle model, TblObject parent) => new()
	{
		Parent = parent,
		Mode = model.Mode,
		Type = model.Type,
		NumCarComponents = model.NumCarComponents,
		TrackTypeId = model.TrackTypeId,
		CostIndex = model.CostIndex,
		CostFactor = model.CostFactor,
		Reliability = model.Reliability,
		RunCostIndex = model.RunCostIndex,
		RunCostFactor = model.RunCostFactor,
		CompanyColourSchemeIndex = model.CompanyColourSchemeIndex,
		Power = model.Power,
		Speed = model.Speed,
		RackSpeed = model.RackSpeed,
		Weight = model.Weight,
		Flags = model.Flags,
		ShipWakeSpacing = model.ShipWakeSpacing,
		DesignedYear = model.DesignedYear,
		ObsoleteYear = model.ObsoleteYear,
		DrivingSoundType = model.DrivingSoundType,
		CarComponents = model.CarComponents,
		BodySprites = model.BodySprites,
		BogieSprites = model.BogieSprites,
		MaxCargo = model.MaxCargo,
		CompatibleCargoCategories = model.CompatibleCargoCategories,
		CargoTypeSpriteOffsets = model.CargoTypeSpriteOffsets,
		Id = model.Id,
	};
}