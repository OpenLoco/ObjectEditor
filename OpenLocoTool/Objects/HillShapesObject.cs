using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record HillShapesObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint8_t HillHeightMapCount,     // 0x02
		[property: LocoStructProperty] uint8_t MountainHeightMapCount, // 0x03
		[property: LocoStructProperty] uint32_t Image,                 // 0x04
		[property: LocoStructProperty] uint32_t var_08,                // 0x08
		[property: LocoStructProperty, LocoArrayLength(0x0E - 0x0C)] uint8_t[] pad_0C
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.hillShapes;
		public int ObjectStructSize => 0xE;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
