using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BogieSprite : ILocoStruct
{
	public uint8_t RollStates { get; set; }
	public BogieSpriteFlags Flags { get; set; }
	public uint8_t Width { get; set; }
	public uint8_t HeightNegative { get; set; }
	public uint8_t HeightPositive { get; set; }
	public uint8_t NumRollSprites { get; set; }

	public Dictionary<BogieSpriteSlopeType, List<int>> ImageIds { get; set; } = new();
	public int NumImages { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (Flags.HasFlag(BogieSpriteFlags.HasSprites))
		{
			if (RollStates is not 1 or 2 or 4)
			{
				yield return new ValidationResult($"{nameof(RollStates)} must be either 1, 2, or 4 when {nameof(Flags)} includes {nameof(BogieSpriteFlags.HasSprites)}.", [nameof(RollStates), nameof(Flags)]);
			}
		}
	}
}
