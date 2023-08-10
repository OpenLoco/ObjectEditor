
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ScaffoldingObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint32_t Image,
		[property: LocoStructProperty(0x06), LocoArrayLength(3)] uint16_t[] SegmentHeights, // 0x06
		[property: LocoStructProperty(0x0C), LocoArrayLength(3)] uint16_t[] RoofHeights    // 0x0C
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.scaffolding;
		public static int ObjectStructSize => 0x12;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
