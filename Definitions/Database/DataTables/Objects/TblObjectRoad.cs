using Definitions.ObjectModels.Objects.Road;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectRoad : DbSubObject, IConvertibleToTable<TblObjectRoad, RoadObject>
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t MaxCurveSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t VehicleDisplayListVerticalOffset { get; set; }
	public TownSize TargetTownSize { get; set; }
	public uint8_t pad_2F { get; set; }
	public string Tunnel { get; set; } = "null";
	public string Bridges { get; set; } = "[]";
	public string Stations { get; set; } = "[]";
	public string RoadMods { get; set; } = "[]";
	public string TracksAndRoads { get; set; } = "[]";

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
			Tunnel = JsonSerializer.Serialize(obj.Tunnel),
			Bridges = JsonSerializer.Serialize(obj.Bridges),
			Stations = JsonSerializer.Serialize(obj.Stations),
			RoadMods = JsonSerializer.Serialize(obj.RoadMods),
			TracksAndRoads = JsonSerializer.Serialize(obj.TracksAndRoads),
		};
}