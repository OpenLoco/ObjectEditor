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
