using System.ComponentModel;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace Core.Objects.Sound
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	public record SoundObjectData(
		[property: LocoStructOffset(0x00)] int32_t var_00,
		[property: LocoStructOffset(0x04)] int32_t Offset,
		[property: LocoStructOffset(0x08)] uint32_t Length,
		[property: LocoStructOffset(0x0C)] WaveFormatEx PcmHeader
		) : ILocoStruct
	{
		public bool Validate()
		{
			return Offset >= 0;
		}
	}
}
