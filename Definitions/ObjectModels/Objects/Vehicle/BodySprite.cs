using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BodySprite : ILocoStruct
{
	public uint8_t NumFlatRotationFrames { get; set; }
	public uint8_t NumSlopedRotationFrames { get; set; }
	public uint8_t NumAnimationFrames { get; set; }
	public uint8_t NumCargoLoadFrames { get; set; }
	public uint8_t NumCargoFrames { get; set; }
	public uint8_t NumRollFrames { get; set; }
	public uint8_t HalfLength { get; set; }
	public BodySpriteFlags Flags { get; set; }
	[Browsable(false)] public uint8_t _Width { get; set; }
	[Browsable(false)] public uint8_t _HeightNegative { get; set; }
	[Browsable(false)] public uint8_t _HeightPositive { get; set; }
	[Browsable(false)] public uint8_t _FlatYawAccuracy { get; set; }
	[Browsable(false)] public uint8_t _SlopedYawAccuracy { get; set; }
	[Browsable(false)] public uint8_t _NumFramesPerRotation { get; set; }
	[Browsable(false)] public image_id _FlatImageId { get; set; }
	[Browsable(false)] public image_id _UnkImageId { get; set; }
	[Browsable(false)] public image_id _GentleImageId { get; set; }
	[Browsable(false)] public image_id _SteepImageId { get; set; }

	//public Dictionary<BodySpriteSlopeType, List<int>> ImageIds { get; set; } = [];
	//public int NumImages { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (Flags.HasFlag(BodySpriteFlags.HasSprites))
		{
			if (NumFlatRotationFrames is not 8 or 16 or 32 or 64 or 128)
			{
				yield return new ValidationResult($"{nameof(NumFlatRotationFrames)} must be one of the following values: 8, 16, 32, 64, 128.", [nameof(NumFlatRotationFrames)]);
			}

			if (NumSlopedRotationFrames is not 4 or 8 or 16 or 32)
			{
				yield return new ValidationResult($"{nameof(NumSlopedRotationFrames)} must be one of the following values: 8, 16, 32, 64, 128.", [nameof(NumSlopedRotationFrames)]);
			}

			if (NumAnimationFrames is not 1 or 2 or 4)
			{
				yield return new ValidationResult($"{nameof(NumAnimationFrames)} must be one of the following values: 1, 2, 4.", [nameof(NumAnimationFrames)]);
			}

			if (NumCargoLoadFrames is < 1 or > 5)
			{
				yield return new ValidationResult($"{nameof(NumCargoLoadFrames)} must be between 1 and 5 inclusive.", [nameof(NumCargoLoadFrames)]);
			}

			if (NumRollFrames is not 1 or 3)
			{
				yield return new ValidationResult($"{nameof(NumRollFrames)} must be one of the following values: 1, 3.", [nameof(NumRollFrames)]);
			}
		}
	}
}
