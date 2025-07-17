using Dat.Types.Audio;
using NAudio.Wave;
using System.Collections.Generic;

namespace Gui.Models.Audio;

public static class AudioHelpers
{
	public static Dictionary<LocoAudioType, WaveFormat> LocoAudioTypeToWaveFormat = new()
	{
		[LocoAudioType.Music] = WaveFormat.CreateCustomFormat(
				WaveFormatEncoding.Pcm,
				44100, // SampleRate
				2, // Channels
				44100, // AverageBytesPerSecond
				4, // BlockAlign
				16), // BitsPerSample
		[LocoAudioType.SoundEffect] = WaveFormat.CreateCustomFormat(
				WaveFormatEncoding.Pcm,
				22050, // SampleRate
				1, // Channels
				22050, // AverageBytesPerSecond
				2, // BlockAlign
				16) // BitsPerSample
	};

	public static WaveFormat SoundEffectFormatToWaveFormat(SoundEffectWaveFormat locoWaveFormat)
		=> WaveFormat.CreateCustomFormat(
			(WaveFormatEncoding)locoWaveFormat.WaveFormatTag,
			locoWaveFormat.SampleRate,
			locoWaveFormat.Channels,
			locoWaveFormat.AverageBytesPerSecond,
			2, //locoWaveFormat.BlockAlign,
			16); //locoWaveFormat.BitsPerSample);

	public static SoundEffectWaveFormat WaveFormatToSoundEffectFormat(WaveFormat waveFormat)
		=> new()
		{
			WaveFormatTag = (int16_t)waveFormat.Encoding,
			Channels = (int16_t)waveFormat.Channels,
			SampleRate = (int32_t)waveFormat.SampleRate,
			AverageBytesPerSecond = (int32_t)waveFormat.AverageBytesPerSecond,
			BlockAlign = (int16_t)waveFormat.BlockAlign,
			BitsPerSample = (int16_t)waveFormat.BitsPerSample,
			ExtraSize = (int16_t)waveFormat.ExtraSize
		};
}
