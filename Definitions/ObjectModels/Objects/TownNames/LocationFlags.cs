namespace Definitions.ObjectModels.Objects.TownNames;

[Flags]
public enum LocationFlags : uint8_t
{
	None = 0,
	AdjacentToLargeWaterBody = 1 << 0,
	Mountainous = 1 << 1,
	AdjacentToSmallWaterBody = 1 << 2,
};
