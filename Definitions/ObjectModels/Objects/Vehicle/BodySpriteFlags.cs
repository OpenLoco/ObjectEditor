namespace Definitions.ObjectModels.Objects.Vehicle;

[Flags]
public enum BodySpriteFlags : uint8_t
{
	None = 0,
	HasSprites = 1 << 0,         // If not set then no body will be loaded
	RotationalSymmetry = 1 << 1, // Requires 32 rather than 64 sprites
	Flag02_Deprecated = 1 << 2, // Incomplete feature of vanilla. Do not repurpose until new object format
	HasGentleSprites = 1 << 3, // For gentle slopes
	HasSteepSprites = 1 << 4,  // For steep slopes
	HasBrakingLights = 1 << 5,
	HasSpeedAnimation = 1 << 6, // Speed based animation (such as hydrofoil)
}
