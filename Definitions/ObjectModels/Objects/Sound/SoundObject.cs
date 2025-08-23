using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Sound;

//[LocoStructSize(0x12)]
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

//[LocoStructSize(0x1E)]
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
	[Browsable(false)]public byte[] PcmData { get; set; } = [];

	// unk
	public uint32_t NumUnkStructs { get; set; }
	[Browsable(false)] public byte[] UnkData { get; set; }

	public bool Validate()
		=> SoundObjectData?.Offset >= 0;
}
