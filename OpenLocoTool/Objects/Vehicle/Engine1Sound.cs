using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x11)]
	public record Engine1Sound(
		[property: LocoStructProperty(0x00)] uint8_t SoundObjectId,
		[property: LocoStructProperty(0x01)] uint16_t DefaultFrequency,
		[property: LocoStructProperty(0x03)] uint8_t DefaultVolume,
		[property: LocoStructProperty(0x04)] uint16_t var_04,
		[property: LocoStructProperty(0x06)] uint8_t var_06,
		[property: LocoStructProperty(0x07)] uint16_t var_07,
		[property: LocoStructProperty(0x08)] uint8_t var_09,
		[property: LocoStructProperty(0x0A)] uint16_t FreqIncreaseStep,
		[property: LocoStructProperty(0x0C)] uint16_t FreqDecreaseStep,
		[property: LocoStructProperty(0x0E)] uint8_t VolumeIncreaseStep,
		[property: LocoStructProperty(0x0F)] uint8_t VolumeDecreaseStep,
		[property: LocoStructProperty(0x10)] uint8_t SpeedFreqFactor
		) : ILocoStruct
	{
		public static int StructLength => 0x11;
	}
}
