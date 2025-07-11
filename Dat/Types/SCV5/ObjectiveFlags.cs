namespace Dat.Types.SCV5;

[Flags]
public enum ObjectiveFlags : uint8_t
{
	None = (byte)0U,
	BeTopCompany = (byte)(1U << 0),
	BeWithinTopThreeCompanies = (byte)(1U << 1),
	WithinTimeLimit = (byte)(1U << 2),
	unk_03 = (byte)(1U << 3),
	unk_04 = (byte)(1U << 4),
}
