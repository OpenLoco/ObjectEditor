namespace Definitions.ObjectModels.Objects.Wall;

[Flags]
public enum WallObjectFlags1 : uint8_t
{
	None = 0,
	HasPrimaryColour = 1 << 0,
	HasGlass = 1 << 1,  // unused? only for rct2?
	OnlyOnLevelLand = 1 << 2,
	DoubleSided = 1 << 3, // unused? only for rct2?
	Door_UNUSED = 1 << 4, // unused, setting it in loco does nothing
	LongDoor_UNUSED = 1 << 5, // unused, setting it in loco does nothing
	HasSecondaryColour = 1 << 6,
	HasTertiaryColour_UNUSED = 1 << 7, // unused, setting it in loco does nothing
}
