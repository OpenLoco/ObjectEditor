using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Dock;

public class DockObject : ILocoStruct, IHasBuildingComponents
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_07 { get; set; } // probably padding, not used in the game
	public DockObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public Pos2 BoatPosition { get; set; }

	public BuildingComponentsModel BuildingComponents { get; set; } = new();

	public bool Validate()
	{
		if (BuildingComponents.Validate())
		{
			return false;
		}

		if (CostIndex > 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		return BuildCostFactor > 0;
	}
}
