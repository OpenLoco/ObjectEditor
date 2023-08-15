
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record SoundObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		//[property: LocoStructProperty(0x02)] const SoundObjectData* Data,
		[property: LocoStructProperty(0x06)] uint8_t var_06,
		[property: LocoStructProperty(0x07)] uint8_t pad_07,
		[property: LocoStructProperty(0x08)] uint32_t Volume
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.sound;
		public static int StructLength => 0x0C;
	}
}
