using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Sound;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SoundObjectData
{
	public int32_t var_00 { get; set; }
	public int32_t Offset { get; set; }
	public uint32_t Length { get; set; }
	public SoundEffectWaveFormat PcmHeader { get; set; } = new SoundEffectWaveFormat();
}
