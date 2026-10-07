using Definitions.Database;

namespace Definitions.DTO.Mappers;

public static class DtoObjectTrackMapper
{
	public static DtoObjectTrack ToDto(this TblObjectTrack tblobjecttrack) => new()
	{
		TrackPieces = tblobjecttrack.TrackPieces,
		StationTrackPieces = tblobjecttrack.StationTrackPieces,
		BuildCostFactor = tblobjecttrack.BuildCostFactor,
		SellCostFactor = tblobjecttrack.SellCostFactor,
		TunnelCostFactor = tblobjecttrack.TunnelCostFactor,
		CostIndex = tblobjecttrack.CostIndex,
		MaxCurveSpeed = tblobjecttrack.MaxCurveSpeed,
		Flags = tblobjecttrack.Flags,
		VehicleDisplayListVerticalOffset = tblobjecttrack.VehicleDisplayListVerticalOffset,
		var_06 = tblobjecttrack.var_06,
		Tunnel = tblobjecttrack.Tunnel,
		TrackMods = tblobjecttrack.TrackMods,
		Signals = tblobjecttrack.Signals,
		TracksAndRoads = tblobjecttrack.TracksAndRoads,
		Bridges = tblobjecttrack.Bridges,
		Stations = tblobjecttrack.Stations,
		Id = tblobjecttrack.Id,
	};

	public static TblObjectTrack ToTblObjectTrackEntity(this DtoObjectTrack model, TblObject parent) => new()
	{
		Parent = parent,
		TrackPieces = model.TrackPieces,
		StationTrackPieces = model.StationTrackPieces,
		BuildCostFactor = model.BuildCostFactor,
		SellCostFactor = model.SellCostFactor,
		TunnelCostFactor = model.TunnelCostFactor,
		CostIndex = model.CostIndex,
		MaxCurveSpeed = model.MaxCurveSpeed,
		Flags = model.Flags,
		VehicleDisplayListVerticalOffset = model.VehicleDisplayListVerticalOffset,
		var_06 = model.var_06,
		Tunnel = model.Tunnel,
		TrackMods = model.TrackMods,
		Signals = model.Signals,
		TracksAndRoads = model.TracksAndRoads,
		Bridges = model.Bridges,
		Stations = model.Stations,
		Id = model.Id,
	};

}

