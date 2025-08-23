using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Sound;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SoundEffectWaveFormat
{
	public int16_t WaveFormatTag { get; set; }
	public int16_t Channels { get; set; }
	public int32_t SampleRate { get; set; }
	public int32_t AverageBytesPerSecond { get; set; }
	public int16_t BlockAlign { get; set; }
	public int16_t BitsPerSample { get; set; }
	public int16_t ExtraSize { get; set; }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SoundObjectData
{
	public int32_t var_00 { get; set; }
	public int32_t Offset { get; set; }
	public uint32_t Length { get; set; }
	public SoundEffectWaveFormat PcmHeader { get; set; } = new SoundEffectWaveFormat();
}

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
