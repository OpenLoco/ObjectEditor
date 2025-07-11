using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x1B)]
public record GearboxMotorSound(
	[property: LocoStructOffset(0x00), Browsable(false)] uint8_t SoundObjectId,
	[property: LocoStructOffset(0x01)] uint16_t IdleFrequency,
	[property: LocoStructOffset(0x03)] uint8_t IdleVolume,
	[property: LocoStructOffset(0x04)] uint16_t FirstGearFrequency, // All subsequent gears are based on this frequency
	[property: LocoStructOffset(0x06)] Speed16 FirstGearSpeed,
	[property: LocoStructOffset(0x08)] uint16_t SecondGearFrequencyOffset,
	[property: LocoStructOffset(0x0A)] Speed16 SecondGearSpeed,
	[property: LocoStructOffset(0x0C)] uint16_t ThirdGearFrequencyOffset,
	[property: LocoStructOffset(0x0E)] Speed16 ThirdGearSpeed,
	[property: LocoStructOffset(0x10)] uint16_t FourthGearFrequencyOffset,
	[property: LocoStructOffset(0x12)] uint8_t CoastingVolume,
	[property: LocoStructOffset(0x13)] uint8_t AcceleratingVolume,
	[property: LocoStructOffset(0x14)] uint16_t FreqIncreaseStep,
	[property: LocoStructOffset(0x16)] uint16_t FreqDecreaseStep,
	[property: LocoStructOffset(0x18)] uint8_t VolumeIncreaseStep,
	[property: LocoStructOffset(0x19)] uint8_t VolumeDecreaseStep,
	[property: LocoStructOffset(0x1A)] uint8_t SpeedFrequencyFactor // bit-shift right of vehicle speed, added to calculated base frequency
	) : ILocoStruct
{
	public GearboxMotorSound() : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
	{ }

	public bool Validate() => true;
}
