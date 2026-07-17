using Definitions.ObjectModels.Objects.Dock;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectDock : DbSubObject, IConvertibleToTable<TblObjectDock, DockObject>
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public DockObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public coord_t BoatPositionX { get; set; }
	public coord_t BoatPositionY { get; set; }

	public string BuildingComponents { get; set; } = "null";

	public static TblObjectDock FromObject(TblObject tbl, DockObject obj)
		=> new()
		{
			Parent = tbl,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			Flags = obj.Flags,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			BoatPositionX = obj.BoatPosition.X,
			BoatPositionY = obj.BoatPosition.Y,
			BuildingComponents = JsonSerializer.Serialize(obj.BuildingComponents),
		};
}