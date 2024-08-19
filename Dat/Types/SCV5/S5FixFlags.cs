namespace OpenLoco.Dat.Types.SCV5
{
	[Flags]
	public enum S5FixFlags : uint16_t
	{
		None = (uint16_t)0U,
		FixFlag0 = (uint16_t)1U << 0,
		FixFlag1 = (uint16_t)1U << 1,
	};
}
