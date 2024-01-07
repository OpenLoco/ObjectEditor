using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x11)]
	public record Engine1Sound(
		[property: LocoStructOffset(0x00), Browsable(false)] uint8_t SoundObjectId,
		[property: LocoStructOffset(0x01)] uint16_t DefaultFrequency,
		[property: LocoStructOffset(0x03)] uint8_t DefaultVolume,
		[property: LocoStructOffset(0x04)] uint16_t var_04,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint16_t var_07,
		[property: LocoStructOffset(0x08)] uint8_t var_09,
		[property: LocoStructOffset(0x0A)] uint16_t FreqIncreaseStep,
		[property: LocoStructOffset(0x0C)] uint16_t FreqDecreaseStep,
		[property: LocoStructOffset(0x0E)] uint8_t VolumeIncreaseStep,
		[property: LocoStructOffset(0x0F)] uint8_t VolumeDecreaseStep,
		[property: LocoStructOffset(0x10)] uint8_t SpeedFreqFactor
		) : ILocoStruct;
}
