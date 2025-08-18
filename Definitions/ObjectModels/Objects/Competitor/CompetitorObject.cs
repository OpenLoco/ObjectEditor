namespace Definitions.ObjectModels.Objects.Competitor;

public class CompetitorObject : ILocoStruct, IImageTableNameProvider
{
	public CompetitorNamePrefix AvailableNamePrefixes { get; set; } // bitset
	public CompetitorPlaystyle AvailablePlaystyles { get; set; } // bitset
	public uint32_t Emotions { get; set; } // bitset
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }

	public bool Validate()
	{
		if ((Emotions & (1 << 0)) == 0)
		{
			return false;
		}

		if (Intelligence is < 1 or > 9)
		{
			return false;
		}

		if (Aggressiveness is < 1 or > 9)
		{
			return false;
		}

		return Competitiveness is >= 1 and <= 9;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "smallNeutral" },
		{ 1, "largeNeutral" },
		{ 2, "smallHappy" },
		{ 3, "largeHappy" },
		{ 4, "smallWorried" },
		{ 5, "largeWorried" },
		{ 6, "smallThinking" },
		{ 7, "largeThinking" },
		{ 8, "smallDejected" },
		{ 9, "largeDejected" },
		{ 10, "smallSurprised" },
		{ 11, "largeSurprised" },
		{ 12, "smallScared" },
		{ 13, "largeScared" },
		{ 14, "smallAngry" },
		{ 15, "largeAngry" },
		{ 16, "smallDisgusted" },
		{ 17, "largeDisgusted" },
	};
}
