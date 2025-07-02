using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectTrackExtraMapper
	{
		public static DtoObjectTrackExtra ToDto(this TblObjectTrackExtra tblobjecttrackextra)
		{
			return new DtoObjectTrackExtra
			{
				PaintStyle = tblobjecttrackextra.PaintStyle,
				CostIndex = tblobjecttrackextra.CostIndex,
				BuildCostFactor = tblobjecttrackextra.BuildCostFactor,
				SellCostFactor = tblobjecttrackextra.SellCostFactor,
				Id = tblobjecttrackextra.Id,
			};
		}

		public static TblObjectTrackExtra ToTblObjectTrackExtraEntity(this DtoObjectTrackExtra model)
		{
			return new TblObjectTrackExtra
			{
				PaintStyle = model.PaintStyle,
				CostIndex = model.CostIndex,
				BuildCostFactor = model.BuildCostFactor,
				SellCostFactor = model.SellCostFactor,
				Id = model.Id,
			};
		}

	}
}

