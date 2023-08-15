using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x03)]
	public record SimpleAnimation(
		[property: LocoStructProperty(0x00)] uint8_t ObjectId,
		[property: LocoStructProperty(0x01)] uint8_t Height,
		[property: LocoStructProperty(0x02)] SimpleAnimationType Type
		) : ILocoStruct
	{
		public static int StructLength => 0x03;
	}
}
