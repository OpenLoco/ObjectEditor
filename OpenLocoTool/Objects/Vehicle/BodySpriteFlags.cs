namespace OpenLocoTool.Objects
{
	enum BodySpriteFlags : uint8_t
	{
		none = 0,
		hasSprites = 1 << 0,         // If not set then no body will be loaded
		rotationalSymmetry = 1 << 1, // requires 32 rather than 64 sprites
		hasUnkSprites = 1 << 2,
		hasGentleSprites = 1 << 3, // for gentle slopes
		hasSteepSprites = 1 << 4,  // for steep slopes
		hasBrakingLights = 1 << 5,
		hasSpeedAnimation = 1 << 6, // Speed based animation (such as hydrofoil)
	};
}
