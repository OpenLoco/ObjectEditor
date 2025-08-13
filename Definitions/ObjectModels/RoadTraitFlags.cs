namespace Definitions.ObjectModels;

[Flags]
public enum RoadTraitFlags : uint16_t
{
	None = 0,
	SmallCurve = 1 << 0,
	VerySmallCurve = 1 << 1,
	Slope = 1 << 2,
	SteepSlope = 1 << 3,
	unk_04 = 1 << 4, // intersection?
	Turnaround = 1 << 5,
	unk_06 = 1 << 6, // overtake?
	unk_07 = 1 << 7,
	unk_08 = 1 << 8, // streetlight?
}
