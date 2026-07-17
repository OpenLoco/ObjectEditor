using Definitions.ObjectModels.Objects.Road;

namespace Definitions.Database;

public class TblObjectRoad : DbSubObject, IConvertibleToTable<TblObjectRoad, RoadObject>
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public Speed16 MaxCurveSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t VehicleDisplayListVerticalOffset { get; set; }
	public TownSize TargetTownSize { get; set; }
	public uint8_t pad_2F { get; set; }
	public UniqueObjectId TunnelId { get; set; }
	public TblObjectTunnel? Tunnel { get; set; }

	public static TblObjectRoad FromObject(TblObject tbl, RoadObject obj)
		=> new()
		{
			Parent = tbl,
			RoadPieces = obj.RoadPieces,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			TunnelCostFactor = obj.TunnelCostFactor,
			CostIndex = obj.CostIndex,
			MaxCurveSpeed = obj.MaxCurveSpeed,
			Flags = obj.Flags,
			PaintStyle = obj.PaintStyle,
			VehicleDisplayListVerticalOffset = obj.VehicleDisplayListVerticalOffset,
			TargetTownSize = obj.TargetTownSize,
			pad_2F = obj.pad_2F,
		};
}