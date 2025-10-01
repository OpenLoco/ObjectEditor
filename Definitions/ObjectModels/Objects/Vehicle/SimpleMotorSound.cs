using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SimpleMotorSound
{
	public ObjectModelHeader SoundObject { get; set; }
	public uint16_t IdleFrequency { get; set; }
	public uint8_t IdleVolume { get; set; }
	public uint16_t CoastingFrequency { get; set; }
	public uint8_t CoastingVolume { get; set; }
	public uint16_t AccelerationBaseFrequency { get; set; }
	public uint8_t AccelerationVolume { get; set; }
	public uint16_t FreqIncreaseStep { get; set; }
	public uint16_t FreqDecreaseStep { get; set; }
	public uint8_t VolumeIncreaseStep { get; set; }
	public uint8_t VolumeDecreaseStep { get; set; }
	public uint8_t SpeedFrequencyFactor { get; set; } // bit-shift right of vehicle speed, added to calculated base frequency
}
