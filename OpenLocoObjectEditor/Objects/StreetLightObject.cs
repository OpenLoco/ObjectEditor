using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.StreetLight)]
	[LocoStringTable("Name")]
	public record StreetLightObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear
	) : ILocoStruct
	{
		public const int DesignedYearLength = 3;
	}
}
