using Dat.FileParsing;

namespace Dat.Types.Audio;

// this is essentially just a RIFF WAV format header
[LocoStructSize(0x2C)]
public record MusicWaveFormat(
	[property: LocoStructOffset(0x00)] uint32_t Signature, // ASCII, "RIFF"
	[property: LocoStructOffset(0x04)] uint32_t Size,
	[property: LocoStructOffset(0x08)] uint32_t RiffType, // ASCII, e.g. "WAVE"
	[property: LocoStructOffset(0x0C)] uint32_t FormatMarker, // either "fmt" or "fmt\0"
	[property: LocoStructOffset(0x10)] uint32_t FormatSize,
	[property: LocoStructOffset(0x14)] uint16_t FormatType, // 1 is PCM
	[property: LocoStructOffset(0x16)] uint16_t Channels,
	[property: LocoStructOffset(0x18)] uint32_t SampleRate,
	[property: LocoStructOffset(0x1C)] uint32_t ByteRate, // SampleRate * BitsPerSample * Channels / 8
	[property: LocoStructOffset(0x20)] uint16_t BlockAlign, // (BitsPerSample* Channels) / 8.1 - 8 bit mono2 - 8 bit stereo/16 bit mono4 - 16 bit stereo
	[property: LocoStructOffset(0x22)] uint16_t BitsPerSample,
	[property: LocoStructOffset(0x24)] uint32_t DataMarker,
	[property: LocoStructOffset(0x28)] uint32_t DataLength) : ILocoStruct
{
	public const int StructLength = 0x2C;

	public uint BytesPerSecond
		=> SampleRate * BitsPerSample * Channels / 8;

	public ReadOnlySpan<byte> Write()
	{
		using var bs = new BinaryWriter(new MemoryStream());

		bs.Write(BitConverter.GetBytes(Signature));
		bs.Write(BitConverter.GetBytes(Size));
		bs.Write(BitConverter.GetBytes(RiffType));
		bs.Write(BitConverter.GetBytes(FormatMarker));
		bs.Write(BitConverter.GetBytes(FormatSize));
		bs.Write(BitConverter.GetBytes(FormatType));
		bs.Write(BitConverter.GetBytes(Channels));
		bs.Write(BitConverter.GetBytes(SampleRate));
		bs.Write(BitConverter.GetBytes(ByteRate));
		bs.Write(BitConverter.GetBytes(BlockAlign));
		bs.Write(BitConverter.GetBytes(BitsPerSample));
		bs.Write(BitConverter.GetBytes(DataMarker));
		bs.Write(BitConverter.GetBytes(DataLength));

		bs.Flush();
		return ((MemoryStream)bs.BaseStream).ToArray();
	}

	public bool Validate()
	{
		if (Signature != 0x46464952) // "RIFF"
		{
			return false;
		}

		if (RiffType != 0x45564157) // "WAVE"
		{
			return false;
		}

		if (FormatMarker is not 0x20746d66 and not 0x00746d66) // "fmt\0" or "fmt"
		{
			return false;
		}

		if (FormatType != 1) // expected PCM
		{
			return false;
		}

		if (BitsPerSample != 16)
		{
			return false;
		}

		if (DataMarker != 0x61746164)
		{
			return false;
		}

		return true;
	}
}
