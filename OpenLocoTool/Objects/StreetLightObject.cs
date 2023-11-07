using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xC)]
	[LocoStringTable("Name")]
	public record StreetLightObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02), LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear // 0x2
																														  //[property: LocoStructOffset(0x08)] uint32_t Image
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.StreetLight;
		public static int StructSize => 0x0C;
		public const int DesignedYearLength = 3;
	}
}
