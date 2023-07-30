namespace OpenLocoTool.Objects
{
	[Flags]
	public enum BogieSpriteFlags : uint8_t
	{
		none = 0,
		hasSprites = 1 << 0,         // If not set then no bogie will be loaded
		rotationalSymmetry = 1 << 1, // requires 16 rather than 32 sprites
		hasGentleSprites = 1 << 2,   // for gentle slopes
		hasSteepSprites = 1 << 3,    // for steep slopes
		unk_4 = 1 << 4,              // Increases bounding box size
	};
}
