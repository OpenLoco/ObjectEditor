
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record SnowObject(
		[property: LocoStructProperty] string_id Name, // 0x00
		[property: LocoStructProperty] uint32_t Image  // 0x02
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.snow;
		public int ObjectStructSize => 0x6;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
