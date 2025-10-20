namespace Definitions.ObjectModels.Objects.Land;

[Flags]
public enum LandObjectFlags : uint8_t
{
	None = 0,
	HasGrowthStages = 1 << 0,
	HasReplacementLandHeader = 1 << 1,
	IsDesert = 1 << 2,
	NoTrees = 1 << 3,
	HasSharpSlopeTransition = 1 << 4,
	DisableSmoothTileTransition = 1 << 5,
}
