using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectDock : DbSubObject, IConvertibleToTable<TblObjectDock, DockObject>
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public DockObjectFlags Flags { get; set; }
	public BuildingComponents BuildingComponents { get; set; } = new();
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public Pos2 BoatPosition { get; set; } = new();

	public static TblObjectDock FromObject(TblObject tbl, DockObject obj)
		=> new()
		{
			Parent = tbl,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			Flags = obj.Flags,
			BuildingComponents = obj.BuildingComponents,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			BoatPosition = obj.BoatPosition,
		};
}
