using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Sound;

public class SoundObject : ILocoStruct
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }
	public SoundObjectData SoundObjectData { get; set; }
	[Browsable(false)] public byte[] PcmData { get; set; } = [];

	// unk
	public uint32_t NumUnkStructs { get; set; }
	[Browsable(false)] public byte[] UnkData { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (SoundObjectData?.Offset < -1) // todo: move validation into SoundObjectData
		{
			yield return new ValidationResult($"{nameof(SoundObjectData.Offset)} must be -1 or non-negative.", [nameof(SoundObjectData), nameof(SoundObjectData.Offset)]);
		}
	}
}
