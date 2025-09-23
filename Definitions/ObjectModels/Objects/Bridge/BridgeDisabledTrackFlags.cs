namespace Definitions.ObjectModels.Objects.Bridge;

[Flags]
public enum BridgeDisabledTrackFlags : uint16_t
{
	None = 0,
	Slope = 1 << 0,
	SteepSlope = 1 << 1,
	CurveSlope = 1 << 2,
	Diagonal = 1 << 3,
	VerySmallCurve = 1 << 4,
	SmallCurve = 1 << 5,
	Curve = 1 << 6,
	LargeCurve = 1 << 7,
	SBendCurve = 1 << 8,
	OneSided = 1 << 9,
	StartsAtHalfHeight = 1 << 10, // Not used. From RCT2
	Junction = 1 << 11,
	Unk = 1 << 12,
}
