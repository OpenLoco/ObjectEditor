
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record WaterObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint8_t CostIndex, // 0x02
		[property: LocoStructProperty] uint8_t var_03,
		[property: LocoStructProperty] int8_t CostFactor, // 0x04
		[property: LocoStructProperty] uint8_t var_05,
		[property: LocoStructProperty] uint32_t Image, // 0x06
		[property: LocoStructProperty] uint32_t var_0A
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.water;
		public int ObjectStructSize => 0xE;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
