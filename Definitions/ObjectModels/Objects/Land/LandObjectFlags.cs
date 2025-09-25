namespace Definitions.ObjectModels.Objects.Land;

[Flags]
public enum LandObjectFlags : uint8_t
{
	None = 0,
	unk_00 = 1 << 0,
	HasReplacementLandHeader = 1 << 1,
	IsDesert = 1 << 2,
	NoTrees = 1 << 3,
	unk_04 = 1 << 4,
	unk_05 = 1 << 5,
}
