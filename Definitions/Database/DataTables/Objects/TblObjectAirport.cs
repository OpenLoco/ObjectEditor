using Definitions.ObjectModels.Objects.Airport;

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

	//public uint8_t var_07 { get; set; }
	//public List<uint8_t> BuildingHeights { get; set; }
	//public uint8_t NumBuildingParts { get; set; }
	//public List<BuildingPartAnimation> BuildingAnimations { get; set; }
	//public uint8_t NumBuildingVariations { get; set; }
	//public List<List<uint8_t>> BuildingVariations { get; set; }
	//public List<AirportBuilding> BuildingPositions { get; set; }
	//public uint8_t NumMovementNodes { get; set; }
	//public List<MovementNode> MovementNodes { get; set; }
	//public uint8_t NumMovementEdges { get; set; }
	//public List<MovementEdge> MovementEdges { get; set; }
	//public uint8_t[] var_B6 { get; set; }

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
		};
}
