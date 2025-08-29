using System.ComponentModel;

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

	public bool Validate()
		=> SoundObjectData?.Offset >= 0;
}
