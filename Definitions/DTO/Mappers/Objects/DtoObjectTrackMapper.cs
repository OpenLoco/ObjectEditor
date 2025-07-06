using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
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
			CurveSpeed = tblobjecttrack.CurveSpeed,
			Flags = tblobjecttrack.Flags,
			DisplayOffset = tblobjecttrack.DisplayOffset,
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
			CurveSpeed = model.CurveSpeed,
			Flags = model.Flags,
			DisplayOffset = model.DisplayOffset,
			Id = model.Id,
		};

	}
}

