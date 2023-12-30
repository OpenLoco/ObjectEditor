using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.StreetLight)]
	[LocoStringTable("Name")]
	public class StreetLightObject(uint16_t[] designedYear) : ILocoStruct
	{
		[LocoStructOffset(0x02), LocoArrayLength(DesignedYearLength)] public uint16_t[] DesignedYear { get; set; } = designedYear;

		public const int DesignedYearLength = 3;
	}
}
