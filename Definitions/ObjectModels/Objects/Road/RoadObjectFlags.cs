namespace Definitions.ObjectModels.Objects.Road;

[Flags]
public enum RoadObjectFlags : uint16_t
{
	None = 0,
	IsOneWay = 1 << 0,
	UseTracksMenu = 1 << 1,
	unk_02 = 1 << 2,
	unk_03 = 1 << 3, // Likely isTram
	NoWheelSlipping = 1 << 4,
	unk_05 = 1 << 5,
	IsRoad = 1 << 6, // If not set this is tram track
	unk_07 = 1 << 7,
	CanHaveStreetLights = 1 << 8,
}
