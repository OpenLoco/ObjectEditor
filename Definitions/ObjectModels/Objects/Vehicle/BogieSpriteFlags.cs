namespace Definitions.ObjectModels.Objects.Vehicle;

[Flags]
public enum BogieSpriteFlags : uint8_t
{
	None = 0,
	HasSprites = 1 << 0,         // If not set then no bogie will be loaded
	RotationalSymmetry = 1 << 1, // Requires 16 rather than 32 sprites
	HasGentleSprites = 1 << 2,   // For gentle slopes
	HasSteepSprites = 1 << 3,    // For steep slopes
	LargerBoundingBox = 1 << 4,  // Increases bounding box size
}
