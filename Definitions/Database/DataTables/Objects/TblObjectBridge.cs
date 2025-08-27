using Definitions.ObjectModels.Objects.Bridge;

namespace Definitions.Database;

public class TblObjectBridge : DbSubObject, IConvertibleToTable<TblObjectBridge, BridgeObject>
{
	public BridgeObjectFlags Flags { get; set; }
	public uint16_t ClearHeight { get; set; }
	public int16_t DeckDepth { get; set; }
	public uint8_t SpanLength { get; set; }
	public SupportPillarSpacing PillarSpacing { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public MicroZ MaxHeight { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BaseCostFactor { get; set; }
	public int16_t HeightCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint16_t DesignedYear { get; set; }
	public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }

	//public uint8_t CompatibleTrackObjectCount { get; set; }
	//public uint8_t CompatibleRoadObjectCount { get; set; }
	// how to store in DB? just json? base64 encoded?
	//public ICollection<UniqueObjectId> CompatibleTrackObjects { get; set; }
	//public ICollection<UniqueObjectId> CompatibleRoadObjects { get; set; }

	public static TblObjectBridge FromObject(TblObject tbl, BridgeObject obj)
		=> new()
		{
			Parent = tbl,
			Flags = obj.Flags,
			ClearHeight = obj.ClearHeight,
			DeckDepth = obj.DeckDepth,
			SpanLength = obj.SpanLength,
			PillarSpacing = obj.PillarSpacing,
			MaxSpeed = obj.MaxSpeed,
			MaxHeight = obj.MaxHeight,
			CostIndex = obj.CostIndex,
			BaseCostFactor = obj.BaseCostFactor,
			HeightCostFactor = obj.HeightCostFactor,
			SellCostFactor = obj.SellCostFactor,
			DesignedYear = obj.DesignedYear,
			DisabledTrackFlags = obj.DisabledTrackFlags,
		};
}
