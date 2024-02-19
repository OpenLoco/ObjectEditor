using System.ComponentModel;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1B)]
	public record Engine2Sound(
		[property: LocoStructOffset(0x00), Browsable(false)] uint8_t SoundObjectId,
		[property: LocoStructOffset(0x01)] uint16_t DefaultFrequency,
		[property: LocoStructOffset(0x02)] uint8_t DefaultVolume,
		[property: LocoStructOffset(0x04)] uint16_t FirstGearFrequency, // All subsequent gears are based on this frequency
		[property: LocoStructOffset(0x06)] Speed16 FirstGearSpeed,
		[property: LocoStructOffset(0x08)] uint16_t SecondGearFreqFactor,
		[property: LocoStructOffset(0x0A)] Speed16 SecondGearSpeed,
		[property: LocoStructOffset(0x0C)] uint16_t ThirdGearFreqFactor,
		[property: LocoStructOffset(0x0E)] Speed16 ThirdGearSpeed,
		[property: LocoStructOffset(0x10)] uint16_t FourthGearFreqFactor,
		[property: LocoStructOffset(0x12)] uint8_t var_12,
		[property: LocoStructOffset(0x13)] uint8_t var_13,
		[property: LocoStructOffset(0x14)] uint16_t FreqIncreaseStep,
		[property: LocoStructOffset(0x16)] uint16_t FreqDecreaseStep,
		[property: LocoStructOffset(0x18)] uint8_t VolumeIncreaseStep,
		[property: LocoStructOffset(0x19)] uint8_t VolumeDecreaseStep,
		[property: LocoStructOffset(0x1A)] uint8_t SpeedFreqFactor
		) : ILocoStruct
	{
		public bool Validate() => throw new NotImplementedException();
	}
}
