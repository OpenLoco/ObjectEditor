using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO.Mappers
{
	public static class DtoObjectBridgeMapper
	{
		public static DtoObjectBridge ToDto(this TblObjectBridge tblobjectbridge) => new()
		{
			Flags = tblobjectbridge.Flags,
			ClearHeight = tblobjectbridge.ClearHeight,
			DeckDepth = tblobjectbridge.DeckDepth,
			SpanLength = tblobjectbridge.SpanLength,
			PillarSpacing = tblobjectbridge.PillarSpacing,
			MaxSpeed = tblobjectbridge.MaxSpeed,
			MaxHeight = tblobjectbridge.MaxHeight,
			CostIndex = tblobjectbridge.CostIndex,
			BaseCostFactor = tblobjectbridge.BaseCostFactor,
			HeightCostFactor = tblobjectbridge.HeightCostFactor,
			SellCostFactor = tblobjectbridge.SellCostFactor,
			DesignedYear = tblobjectbridge.DesignedYear,
			DisabledTrackFlags = tblobjectbridge.DisabledTrackFlags,
			Id = tblobjectbridge.Id,
		};

		public static TblObjectBridge ToTblObjectBridgeEntity(this DtoObjectBridge model, TblObject parent) => new()
		{
			Parent = parent,
			Flags = model.Flags,
			ClearHeight = model.ClearHeight,
			DeckDepth = model.DeckDepth,
			SpanLength = model.SpanLength,
			PillarSpacing = model.PillarSpacing,
			MaxSpeed = model.MaxSpeed,
			MaxHeight = model.MaxHeight,
			CostIndex = model.CostIndex,
			BaseCostFactor = model.BaseCostFactor,
			HeightCostFactor = model.HeightCostFactor,
			SellCostFactor = model.SellCostFactor,
			DesignedYear = model.DesignedYear,
			DisabledTrackFlags = model.DisabledTrackFlags,
			Id = model.Id,
		};
	}
}
