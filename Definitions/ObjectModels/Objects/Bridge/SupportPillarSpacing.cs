namespace Definitions.ObjectModels.Objects.Bridge;

[Flags]
public enum SupportPillarSpacing : uint8_t
{
	Tile0A = 1 << 0,
	Tile0B = 1 << 1,
	Tile1A = 1 << 2,
	Tile1B = 1 << 3,
	Tile2A = 1 << 4,
	Tile2B = 1 << 5,
	Tile3A = 1 << 6,
	Tile3B = 1 << 7,
}
