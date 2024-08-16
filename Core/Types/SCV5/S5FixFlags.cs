namespace OpenLoco.Dat.Types.SCV5
{
	[Flags]
	public enum S5FixFlags : uint16_t
	{
		none = (uint16_t)0U,
		fixFlag0 = (uint16_t)1U << 0,
		fixFlag1 = (uint16_t)1U << 1,
	};
}
