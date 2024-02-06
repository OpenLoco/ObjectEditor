using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0B)]
	public record FrictionSound(
		[property: LocoStructOffset(0x00), Browsable(false)] uint8_t SoundObjectId,
		[property: LocoStructOffset(0x01)] Speed32 MinSpeed, // below this speed no sound created
		[property: LocoStructOffset(0x05)] uint8_t SpeedFreqFactor,
		[property: LocoStructOffset(0x06)] uint16_t BaseFrequency,
		[property: LocoStructOffset(0x08)] uint8_t SpeedVolumeFactor,
		[property: LocoStructOffset(0x09)] uint8_t BaseVolume,
		[property: LocoStructOffset(0x0A)] uint8_t MaxVolume
		) : ILocoStruct;
}
