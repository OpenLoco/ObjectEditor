namespace Dat.Types.SCV5
{
	[Flags]
	public enum HeaderFlags : uint8_t
	{
		None = (byte)0U,
		IsRaw = (byte)1U << 0,
		IsDump = (byte)1U << 1,
		IsTitleSequence = (byte)1U << 2,
		HasSaveDetails = (byte)1U << 3,
	}
}
