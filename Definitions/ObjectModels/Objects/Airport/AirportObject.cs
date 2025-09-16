using Definitions.ObjectModels.Objects.Common;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Airport;

public class AirportObject : ILocoStruct, IHasBuildingComponents
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_07 { get; set; }
	public uint16_t AllowedPlaneTypes { get; set; }
	public uint32_t LargeTiles { get; set; }
	public int8_t MinX { get; set; }
	public int8_t MinY { get; set; }
	public int8_t MaxX { get; set; }
	public int8_t MaxY { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public BuildingComponentsModel BuildingComponents { get; set; } = new();

	public List<AirportBuilding> BuildingPositions { get; set; } = [];
	public List<MovementNode> MovementNodes { get; set; } = [];
	public List<MovementEdge> MovementEdges { get; set; } = [];
	public uint8_t[] var_B6 { get; set; } = [];

	public bool Validate()
	{
		if (!BuildingComponents.Validate())
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
