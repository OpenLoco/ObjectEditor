using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record Engine2Sound(
		[property: LocoStructProperty(0x00)] uint8_t SoundObjectId,
		[property: LocoStructProperty(0x01)] uint16_t DefaultFrequency,
		[property: LocoStructProperty(0x02)] uint8_t DefaultVolume,
		[property: LocoStructProperty(0x04)] uint16_t FirstGearFrequency, // All subsequent gears are based on this frequency
		[property: LocoStructProperty(0x06)] Speed16 FirstGearSpeed,
		[property: LocoStructProperty(0x08)] uint16_t SecondGearFreqFactor,
		[property: LocoStructProperty(0x0A)] Speed16 SecondGearSpeed,
		[property: LocoStructProperty(0x0C)] uint16_t ThirdGearFreqFactor,
		[property: LocoStructProperty(0x0E)] Speed16 ThirdGearSpeed,
		[property: LocoStructProperty(0x10)] uint16_t FourthGearFreqFactor,
		[property: LocoStructProperty(0x12)] uint8_t var_12,
		[property: LocoStructProperty(0x13)] uint8_t var_13,
		[property: LocoStructProperty(0x14)] uint16_t FreqIncreaseStep,
		[property: LocoStructProperty(0x16)] uint16_t FreqDecreaseStep,
		[property: LocoStructProperty(0x18)] uint8_t VolumeIncreaseStep,
		[property: LocoStructProperty(0x19)] uint8_t VolumeDecreaseStep,
		[property: LocoStructProperty(0x1A)] uint8_t SpeedFreqFactor
		) : ILocoStruct
	{
		public static int StructLength => 0x1B;
	}
}
