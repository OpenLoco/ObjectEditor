using System.ComponentModel.DataAnnotations;
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

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (!Emotions.HasFlag(EmotionFlags.Neutral))
		{
			yield return new ValidationResult($"{nameof(Emotions)} must include {nameof(EmotionFlags.Neutral)}.", [nameof(Emotions)]);
		}

		if (Intelligence is < 1 or > 9)
		{
			yield return new ValidationResult($"{nameof(Intelligence)} must be between 1 and 9 inclusive.", [nameof(Intelligence)]);
		}

		if (Aggressiveness is < 1 or > 9)
		{
			yield return new ValidationResult($"{nameof(Aggressiveness)} must be between 1 and 9 inclusive.", [nameof(Aggressiveness)]);
		}

		if (Competitiveness is < 1 or > 9)
		{
			yield return new ValidationResult($"{nameof(Competitiveness)} must be between 1 and 9 inclusive.", [nameof(Competitiveness)]);
		}
	}
}
