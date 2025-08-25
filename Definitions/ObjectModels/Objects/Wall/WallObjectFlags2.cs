namespace Definitions.ObjectModels.Objects.Wall;

// this is copied from OpenRCT2: https://github.com/OpenRCT2/OpenRCT2/blob/develop/src/openrct2/object/WallSceneryEntry.h
[Flags]
public enum WallObjectFlags2 : uint8_t
{
	None = 0,
	NoSelectPrimaryColour = 1 << 0,
	DoorSoundMask = 1 << 1,  // unused? only for rct2?
	DoorSoundShift = 1 << 2, // unused? only for rct2?
	Opaque = 1 << 3, // unused? only for rct2?
	Animated = 1 << 4, // unused? only for rct2?
}
