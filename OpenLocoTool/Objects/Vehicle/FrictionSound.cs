using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record FrictionSound(
		[property: LocoStructProperty(0x00)] uint8_t SoundObjectId,
		[property: LocoStructProperty(0x01)] Speed32 MinSpeed, // below this speed no sound created
		[property: LocoStructProperty(0x05)] uint8_t SpeedFreqFactor,
		[property: LocoStructProperty(0x06)] uint16_t BaseFrequency,
		[property: LocoStructProperty(0x08)] uint8_t SpeedVolumeFactor,
		[property: LocoStructProperty(0x09)] uint8_t BaseVolume,
		[property: LocoStructProperty(0x0A)] uint8_t MaxVolume
		) : ILocoStruct
	{
		public static int ObjectStructSize => 0xB;
	}
}
