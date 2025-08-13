using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Types.Audio;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x12)]
public record SoundEffectWaveFormat(
	[property: LocoStructOffset(0x00)] int16_t WaveFormatTag,
	[property: LocoStructOffset(0x02)] int16_t Channels,
	[property: LocoStructOffset(0x04)] int32_t SampleRate,
	[property: LocoStructOffset(0x08)] int32_t AverageBytesPerSecond,
	[property: LocoStructOffset(0x0B)] int16_t BlockAlign,
	[property: LocoStructOffset(0x0D)] int16_t BitsPerSample,
	[property: LocoStructOffset(0x010)] int16_t ExtraSize
	) : ILocoStruct
{
	public SoundEffectWaveFormat() : this(0, 0, 0, 0, 0, 0, 0)
	{ }

	public ReadOnlySpan<byte> Write()
	{
		using var bs = new BinaryWriter(new MemoryStream());

		bs.Write(BitConverter.GetBytes(WaveFormatTag));
		bs.Write(BitConverter.GetBytes(Channels));
		bs.Write(BitConverter.GetBytes(SampleRate));
		bs.Write(BitConverter.GetBytes(AverageBytesPerSecond));
		bs.Write(BitConverter.GetBytes(BlockAlign));
		bs.Write(BitConverter.GetBytes(BitsPerSample));
		bs.Write(BitConverter.GetBytes(ExtraSize));

		bs.Flush();
		return ((MemoryStream)bs.BaseStream).ToArray();
	}

	public bool Validate()
		=> true;
}
