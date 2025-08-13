using Dat.Objects;
using Definitions.ObjectModels.Objects.Road;

namespace Dat.Converters;

public static class RoadTraitFlagsConverter
{
	public static DatRoadTraitFlags Convert(this RoadTraitFlags roadTraitFlags)
		=> roadTraitFlags switch
		{
			RoadTraitFlags.None => DatRoadTraitFlags.None,
			RoadTraitFlags.SmallCurve => DatRoadTraitFlags.SmallCurve,
			RoadTraitFlags.VerySmallCurve => DatRoadTraitFlags.VerySmallCurve,
			RoadTraitFlags.Slope => DatRoadTraitFlags.Slope,
			RoadTraitFlags.SteepSlope => DatRoadTraitFlags.SteepSlope,
			RoadTraitFlags.unk_04 => DatRoadTraitFlags.unk_04,
			RoadTraitFlags.Turnaround => DatRoadTraitFlags.Turnaround,
			RoadTraitFlags.unk_06 => DatRoadTraitFlags.unk_06,
			RoadTraitFlags.unk_07 => DatRoadTraitFlags.unk_07,
			RoadTraitFlags.unk_08 => DatRoadTraitFlags.unk_08,
			_ => throw new ArgumentOutOfRangeException(nameof(roadTraitFlags), roadTraitFlags, "Unknown Road Trait Flags")
		};

	public static RoadTraitFlags Convert(this DatRoadTraitFlags datRoadTraitFlags)
		=> datRoadTraitFlags switch
		{
			DatRoadTraitFlags.None => RoadTraitFlags.None,
			DatRoadTraitFlags.SmallCurve => RoadTraitFlags.SmallCurve,
			DatRoadTraitFlags.VerySmallCurve => RoadTraitFlags.VerySmallCurve,
			DatRoadTraitFlags.Slope => RoadTraitFlags.Slope,
			DatRoadTraitFlags.SteepSlope => RoadTraitFlags.SteepSlope,
			DatRoadTraitFlags.unk_04 => RoadTraitFlags.unk_04,
			DatRoadTraitFlags.Turnaround => RoadTraitFlags.Turnaround,
			DatRoadTraitFlags.unk_06 => RoadTraitFlags.unk_06,
			DatRoadTraitFlags.unk_07 => RoadTraitFlags.unk_07,
			DatRoadTraitFlags.unk_08 => RoadTraitFlags.unk_08,
			_ => throw new ArgumentOutOfRangeException(nameof(datRoadTraitFlags), datRoadTraitFlags, "Unknown Road Trait Flags")
		};
}
