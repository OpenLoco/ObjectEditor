namespace Definitions.ObjectModels.Objects.TrackSignal;

[Flags]
public enum TrackSignalObjectFlags : uint16_t
{
	None = 0 << 0,
	IsLeft = 1 << 0,
	HasLights = 1 << 1,
	unk_02 = 1 << 2,
}
