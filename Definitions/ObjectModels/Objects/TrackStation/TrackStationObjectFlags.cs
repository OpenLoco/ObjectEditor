namespace Definitions.ObjectModels.Objects.TrackStation;

[Flags]
public enum TrackStationObjectFlags : uint8_t
{
	None = 0,
	Recolourable = 1 << 0,
	NoGlass = 1 << 1,
}
