using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x11)]
	public record SimpleMotorSound(
		[property: LocoStructOffset(0x00), Browsable(false)] uint8_t SoundObjectId,
		[property: LocoStructOffset(0x01)] uint16_t IdleFrequency,
		[property: LocoStructOffset(0x03)] uint8_t IdleVolume,
		[property: LocoStructOffset(0x04)] uint16_t CoastingFrequency,
		[property: LocoStructOffset(0x06)] uint8_t CoastingVolume,
		[property: LocoStructOffset(0x07)] uint16_t AccelerationBaseFrequency,
		[property: LocoStructOffset(0x09)] uint8_t AccelerationVolume,
		[property: LocoStructOffset(0x0A)] uint16_t FreqIncreaseStep,
		[property: LocoStructOffset(0x0C)] uint16_t FreqDecreaseStep,
		[property: LocoStructOffset(0x0E)] uint8_t VolumeIncreaseStep,
		[property: LocoStructOffset(0x0F)] uint8_t VolumeDecreaseStep,
		[property: LocoStructOffset(0x10)] uint8_t SpeedFrequencyFactor // bit-shift right of vehicle speed, added to calculated base frequency
		) : ILocoStruct
	{
		public SimpleMotorSound() : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
		{ }

		public bool Validate() => true;
	}
}
