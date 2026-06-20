namespace Definitions.ObjectModels.Objects.Track;

[Flags]
public enum TrackObjectFlags : uint16_t
{
	None = 0,
	HasRackRail = 1 << 0,   // if set road can have rack rail added (not used)
	NoSlipSurface = 1 << 1, // if set vehicles can't start slipping
	IsRoad = 1 << 2,        // controls if the object appears in the roads menu instead of the track menu
}
