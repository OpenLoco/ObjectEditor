using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xC)]
	public record StreetLightObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02), LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear, // 0x2
		[property: LocoStructOffset(0x08)] uint32_t Image
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.StreetLight;
		public static int StructSize => 0x0C;
		public const int DesignedYearLength = 3;
	}

	//public class StreetLightObject2 : LocoObject
	//{
	//	public StreeLightObject2(S5Header s5Hdr, ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable, G1Header g1Header, List<G1Element32> g1Elements) : base(s5Hdr, objHdr, obj, stringTable, g1Header, g1Elements)
	//	{
	//	}

	//	public string Name { get; set; }
	//	public uint16_t DesignedYear { get; set; }

	//	public G1Element32 Image { get; set; }

	//}
}
