using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Competitor;

public class CompetitorObject : ILocoStruct
{
	public NamePrefixFlags AvailableNamePrefixes { get; set; } // bitset
	public PlaystyleFlags AvailablePlaystyles { get; set; } // bitset
	public EmotionFlags Emotions { get; set; } // bitset
	public uint8_t Intelligence { get; set; }
	public uint8_t Aggressiveness { get; set; }
	public uint8_t Competitiveness { get; set; }
	public uint8_t var_37 { get; set; }

	public bool Validate()
	{
		if (!Emotions.HasFlag(EmotionFlags.Neutral))
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
}
