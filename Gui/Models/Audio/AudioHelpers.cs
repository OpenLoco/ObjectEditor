using Definitions.ObjectModels.Objects.Sound;
using System.Collections.Generic;

namespace Gui.Models.Audio;

public static class AudioHelpers
{
	public static Dictionary<LocoAudioType, PcmFormat> LocoAudioTypeToPcmFormat = new()
	{
		[LocoAudioType.Music] = new PcmFormat(
			SampleRate: 44100,
			Channels: 2,
			BitsPerSample: 16),

		[LocoAudioType.SoundEffect] = new PcmFormat(
			SampleRate: 22050,
			Channels: 1,
			BitsPerSample: 16)
	};

	public static PcmFormat SoundEffectFormatToPcmFormat(SoundEffectWaveFormat locoWaveFormat)
		=> new(
			SampleRate: locoWaveFormat.SampleRate,
			Channels: locoWaveFormat.Channels,
			BitsPerSample: locoWaveFormat.BitsPerSample);

	public static SoundEffectWaveFormat PcmFormatToSoundEffectFormat(PcmFormat format)
		=> new()
		{
			WaveFormatTag = 1, // PCM
			Channels = (int16_t)format.Channels,
			SampleRate = (int32_t)format.SampleRate,
			AverageBytesPerSecond = (int32_t)(format.SampleRate * format.Channels * (format.BitsPerSample / 8)),
			BlockAlign = (int16_t)(format.Channels * (format.BitsPerSample / 8)),
			BitsPerSample = (int16_t)format.BitsPerSample,
			ExtraSize = 0
		};
}

/// <summary>Simple audio format descriptor — replaces NAudio's WaveFormat.</summary>
public record PcmFormat(int SampleRate, int Channels, int BitsPerSample = 16)
{
	public int AverageBytesPerSecond => SampleRate * Channels * (BitsPerSample / 8);
	public int BlockAlign => Channels * (BitsPerSample / 8);

	/// <summary>Duration in seconds given total sample count (across all channels).</summary>
	public double DurationFromSampleCount(int totalSamples)
		=> totalSamples / (double)(SampleRate * Channels);
}
