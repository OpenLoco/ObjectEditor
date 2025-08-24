namespace Definitions.ObjectModels.Objects.Vehicle;

public class GearboxMotorSound
{
	public uint8_t SoundObjectId { get; set; }
	public uint16_t IdleFrequency { get; set; }
	public uint8_t IdleVolume { get; set; }
	public uint16_t FirstGearFrequency { get; set; } // All subsequent gears are based on this frequency
	public int16_t FirstGearSpeed { get; set; }
	public uint16_t SecondGearFrequencyOffset { get; set; }
	public int16_t SecondGearSpeed { get; set; }
	public uint16_t ThirdGearFrequencyOffset { get; set; }
	public int16_t ThirdGearSpeed { get; set; }
	public uint16_t FourthGearFrequencyOffset { get; set; }
	public uint8_t CoastingVolume { get; set; }
	public uint8_t AcceleratingVolume { get; set; }
	public uint16_t FreqIncreaseStep { get; set; }
	public uint16_t FreqDecreaseStep { get; set; }
	public uint8_t VolumeIncreaseStep { get; set; }
	public uint8_t VolumeDecreaseStep { get; set; }
	public uint8_t SpeedFrequencyFactor { get; set; } // bit-shift right of vehicle speed, added to calculated base frequency
}
