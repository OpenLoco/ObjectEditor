using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class FrictionSound
{
	public uint8_t SoundObjectId { get; set; }
	public int32_t MinSpeed { get; set; }
	public uint8_t SpeedFreqFactor { get; set; }
	public uint16_t BaseFrequency { get; set; }
	public uint8_t SpeedVolumeFactor { get; set; }
	public uint8_t BaseVolume { get; set; }
	public uint8_t MaxVolume { get; set; }
}
