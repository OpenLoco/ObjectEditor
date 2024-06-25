namespace Core.Types.SCV5
{
	[Flags]
	public enum ObjectiveFlags : uint8_t
	{
		None = (byte)0U,
		BeTopCompany = (byte)(1U << 0),
		BeWithinTopThreeCompanies = (byte)(1U << 1),
		WithinTimeLimit = (byte)(1U << 2),
		Flag_3 = (byte)(1U << 3),
		Flag_4 = (byte)(1U << 4),
	}
}
