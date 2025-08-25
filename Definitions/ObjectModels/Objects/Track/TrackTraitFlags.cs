namespace Definitions.ObjectModels.Objects.Track;

[Flags]
public enum TrackTraitFlags : uint16_t
{
	None = 0,
	Diagonal = 1 << 0,
	LargeCurve = 1 << 1,
	NormalCurve = 1 << 2,
	SmallCurve = 1 << 3,
	VerySmallCurve = 1 << 4,
	Slope = 1 << 5,
	SteepSlope = 1 << 6,
	OneSided = 1 << 7,
	SlopedCurve = 1 << 8,
	SBend = 1 << 9,
	Junction = 1 << 10,
}
