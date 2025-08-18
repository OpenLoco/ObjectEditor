namespace Definitions.ObjectModels.Objects.Airport;

public enum AirportMovementNodeFlags : uint16_t
{
	None = 0,
	Terminal = 1 << 0,
	TakeoffEnd = 1 << 1,
	Flag2 = 1 << 2,
	Taxiing = 1 << 3,
	InFlight = 1 << 4,
	HeliTakeoffBegin = 1 << 5,
	TakeoffBegin = 1 << 6,
	HeliTakeoffEnd = 1 << 7,
	Touchdown = 1 << 8,
}

public record MovementEdge(
	uint8_t var_00,
	uint8_t CurrNode,
	uint8_t NextNode,
	uint8_t var_03,
	uint32_t MustBeClearEdges,    // Which edges must be clear to use the transition edge. should probably be some kind of flags?
	uint32_t AtLeastOneClearEdges // Which edges must have at least one clear to use transition edge. should probably be some kind of flags?
	) : ILocoStruct
{
	public MovementEdge() : this(0, 0, 0, 0, 0, 0)
	{ }

	public bool Validate()
		=> true;
}

public record MovementNode(
	int16_t X,
	int16_t Y,
	int16_t Z,
	AirportMovementNodeFlags Flags
	) : ILocoStruct
{
	public MovementNode() : this(0, 0, 0, AirportMovementNodeFlags.None)
	{ }

	public bool Validate()
		=> true;
}

public record AirportBuilding(
	uint8_t Index,
	uint8_t Rotation,
	int8_t X,
	int8_t Y
	) : ILocoStruct
{
	public AirportBuilding() : this(0, 0, 0, 0)
	{ }

	public bool Validate()
		=> true;
}

public record BuildingPartAnimation(
	uint8_t NumFrames,     // Must be a power of 2 (0 = no part animation, could still have animation sequence)
	uint8_t AnimationSpeed // Also encodes in bit 7 if the animation is position modified
	) : ILocoStruct
{
	public BuildingPartAnimation() : this(0, 0)
	{ }

	public bool Validate()
		=> IsPowerOfTwo(NumFrames);

	static bool IsPowerOfTwo(uint8_t x)
		=> (x & (x - 1)) == 0 && x > 0;
}

public class AirportObject : ILocoStruct
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
	public List<uint8_t> BuildingHeights { get; set; } = [];
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];
	public List<AirportBuilding> BuildingPositions { get; set; } = [];
	public List<MovementNode> MovementNodes { get; set; } = [];
	public List<MovementEdge> MovementEdges { get; set; } = [];
	public uint8_t[] var_B6 { get; set; } = [];

	public bool Validate()
	{
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
