using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectRoadExtraMapper
	{
		public static DtoObjectRoadExtra ToDto(this TblObjectRoadExtra tblobjectroadextra) => new()
		{
			PaintStyle = tblobjectroadextra.PaintStyle,
			CostIndex = tblobjectroadextra.CostIndex,
			BuildCostFactor = tblobjectroadextra.BuildCostFactor,
			SellCostFactor = tblobjectroadextra.SellCostFactor,
			Id = tblobjectroadextra.Id,
		};

		public static TblObjectRoadExtra ToTblObjectRoadExtraEntity(this DtoObjectRoadExtra model, TblObject parent) => new()
		{
			Parent = parent,
			PaintStyle = model.PaintStyle,
			CostIndex = model.CostIndex,
			BuildCostFactor = model.BuildCostFactor,
			SellCostFactor = model.SellCostFactor,
			Id = model.Id,
		};

	}
}

