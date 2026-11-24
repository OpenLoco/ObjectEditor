using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Common;

namespace Definitions.Database;

public class TblObjectAirport : DbSubObject, IConvertibleToTable<TblObjectAirport, AirportObject>
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public AirportObjectFlags Flags { get; set; }
	public BuildingComponents BuildingComponents { get; set; } = new();
	public uint32_t LargeTiles { get; set; }
	public int8_t MinX { get; set; }
	public int8_t MinY { get; set; }
	public int8_t MaxX { get; set; }
	public int8_t MaxY { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<AirportBuilding> BuildingPositions { get; set; } = [];
	public List<MovementNode> MovementNodes { get; set; } = [];
	public List<MovementEdge> MovementEdges { get; set; } = [];
	public uint32_t RequiredClearEdges { get; set; } // bitfield of edges that must be clear for the airport to be built

	public static TblObjectAirport FromObject(TblObject tbl, AirportObject obj)
		=> new()
		{
			Parent = tbl,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			Flags = obj.Flags,
			BuildingComponents = obj.BuildingComponents,
			LargeTiles = obj.LargeTiles,
			MinX = obj.MinX,
			MinY = obj.MinY,
			MaxX = obj.MaxX,
			MaxY = obj.MaxY,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			BuildingPositions = obj.BuildingPositions,
			MovementNodes = obj.MovementNodes,
			MovementEdges = obj.MovementEdges,
			RequiredClearEdges = obj.RequiredClearEdges,
		};
}
