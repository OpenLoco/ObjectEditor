using Definitions.ObjectModels.Objects.Airport;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectAirport : DbSubObject, IConvertibleToTable<TblObjectAirport, AirportObject>
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public AirportObjectFlags Flags { get; set; }
	public uint32_t LargeTiles { get; set; }
	public int8_t MinX { get; set; }
	public int8_t MinY { get; set; }
	public int8_t MaxX { get; set; }
	public int8_t MaxY { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint32_t var_B6 { get; set; }
	public string BuildingComponents { get; set; } = "null";
	public string BuildingPositions { get; set; } = "[]";
	public string MovementNodes { get; set; } = "[]";
	public string MovementEdges { get; set; } = "[]";

	public static TblObjectAirport FromObject(TblObject tbl, AirportObject obj)
		=> new()
		{
			Parent = tbl,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			Flags = obj.Flags,
			LargeTiles = obj.LargeTiles,
			MinX = obj.MinX,
			MinY = obj.MinY,
			MaxX = obj.MaxX,
			MaxY = obj.MaxY,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			var_B6 = obj.var_B6,
			BuildingComponents = JsonSerializer.Serialize(obj.BuildingComponents),
			BuildingPositions = JsonSerializer.Serialize(obj.BuildingPositions),
			MovementNodes = JsonSerializer.Serialize(obj.MovementNodes),
			MovementEdges = JsonSerializer.Serialize(obj.MovementEdges),
		};
}