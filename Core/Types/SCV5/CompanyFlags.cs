namespace Core.Types.SCV5
{
	[Flags]
	public enum CompanyFlags : uint32_t
	{
		None = 0U,
		Unk0 = 1U << 0,                      // 0x01
		Unk1 = 1U << 1,                      // 0x02
		Unk2 = 1U << 2,                      // 0x04
		Sorted = 1U << 3,                    // 0x08
		IncreasedPerformance = 1U << 4,      // 0x10
		DecreasedPerformance = 1U << 5,      // 0x20
		ChallengeCompleted = 1U << 6,        // 0x40
		ChallengeFailed = 1U << 7,           // 0x80
		ChallengeBeatenByOpponent = 1U << 8, // 0x100
		Bankrupt = 1U << 9,                  // 0x200
		AutopayLoan = 1U << 31,              // 0x80000000 new for OpenLoco
	}
}
