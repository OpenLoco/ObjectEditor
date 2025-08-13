using Dat.Objects;
using Definitions.ObjectModels.Objects.TrackSignal;

namespace Dat.Converters;

public static class TrackSignalFlagsConverter
{
	public static DatTrackSignalObjectFlags Convert(this TrackSignalObjectFlags trackSignalFlags)
		=> trackSignalFlags switch
		{
			TrackSignalObjectFlags.None => DatTrackSignalObjectFlags.None,
			TrackSignalObjectFlags.IsLeft => DatTrackSignalObjectFlags.IsLeft,
			TrackSignalObjectFlags.HasLights => DatTrackSignalObjectFlags.HasLights,
			TrackSignalObjectFlags.unk_02 => DatTrackSignalObjectFlags.unk_02,
			_ => throw new ArgumentOutOfRangeException(nameof(trackSignalFlags), trackSignalFlags, "Unknown Track Signal Flags")
		};

	public static TrackSignalObjectFlags Convert(this DatTrackSignalObjectFlags datTrackSignalFlags)
		=> datTrackSignalFlags switch
		{
			DatTrackSignalObjectFlags.None => TrackSignalObjectFlags.None,
			DatTrackSignalObjectFlags.IsLeft => TrackSignalObjectFlags.IsLeft,
			DatTrackSignalObjectFlags.HasLights => TrackSignalObjectFlags.HasLights,
			DatTrackSignalObjectFlags.unk_02 => TrackSignalObjectFlags.unk_02,
			_ => throw new ArgumentOutOfRangeException(nameof(datTrackSignalFlags), datTrackSignalFlags, "Unknown Dat Track Signal Flags")
		};
}
