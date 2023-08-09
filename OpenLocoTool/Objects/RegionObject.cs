
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record RegionObject(
		[property: LocoStructProperty] string_id name,
		[property: LocoStructProperty] uint32_t image, // 0x02
		[property: LocoStructProperty, LocoArrayLength(0x8 - 0x6)] uint8_t[] pad_06,
		[property: LocoStructProperty] uint8_t var_08,
		[property: LocoStructProperty, LocoArrayLength(4)] uint8_t[] var_09,
		[property: LocoStructProperty, LocoArrayLength(0x12 - 0xD)] uint8_t[] pad_0D) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.region;
		public int ObjectStructSize => 0x12;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
