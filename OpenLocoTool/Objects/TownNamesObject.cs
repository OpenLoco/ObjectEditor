
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1A)]
	public record TownNamesUnk(
		[property: LocoStructProperty(0x00)] uint8_t Count,
		[property: LocoStructProperty(0x00)] uint8_t Fill,
		[property: LocoStructProperty(0x00)] uint16_t Offset
	) : ILocoStruct
	{
		public static int StructLength => 0x04;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record TownNamesObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02), LocoArrayLength(6)] TownNamesUnk[] unks
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.townNames;
		public static int StructLength => 0x1A;
	}
}
