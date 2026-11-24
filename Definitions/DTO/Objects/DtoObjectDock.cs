using Definitions.Database;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectDock : IDtoSubObject
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public DockObjectFlags Flags { get; set; }
	public BuildingComponents BuildingComponents { get; set; } = new();
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public Pos2 BoatPosition { get; set; } = new();
	public UniqueObjectId Id { get; set; }
}
