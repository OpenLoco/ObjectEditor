namespace Dat.Loaders;

[Flags]
public enum DatBogieSpriteFlags : uint8_t
{
	None = 0,
	HasSprites = 1 << 0,         // If not set then no bogie will be loaded
	RotationalSymmetry = 1 << 1, // requires 16 rather than 32 sprites
	unk_02 = 1 << 2,
	HasGentleSprites = 1 << 3,   // for gentle slopes
	HasSteepSprites = 1 << 4,    // for steep slopes
	HasBrakingLights = 1 << 5,
	HasSpeedAnimation = 1 << 6, // Speed based animation (such as hydrofoil)
}
