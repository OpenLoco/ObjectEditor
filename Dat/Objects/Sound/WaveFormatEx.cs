using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects.Sound
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record WaveFormatEx(
		[property: LocoStructOffset(0x00)] int16_t WaveFormatTag,
		[property: LocoStructOffset(0x02)] int16_t NumberOfChannels,
		[property: LocoStructOffset(0x04)] int32_t SampleRate,
		[property: LocoStructOffset(0x08)] int32_t AverageBytesPerSecond,
		[property: LocoStructOffset(0x0B)] int16_t BlockAlign,
		[property: LocoStructOffset(0x0D)] int16_t BitsPerSample,
		[property: LocoStructOffset(0x010)] int16_t CBSize
		) : ILocoStruct
	{
		public WaveFormatEx() : this(0, 0, 0, 0, 0, 0, 0)
		{ }

		public bool Validate()
			=> true;
	}
}
