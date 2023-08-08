using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CliffEdgeObject(
		[property: LocoStructProperty] string_id Name,   // 0x0,
		[property: LocoStructProperty] uint32_t Image // 0x2
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.cliffEdge;

		public int ObjectStructSize => 0x6;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			throw new NotImplementedException("");
		}

		public ReadOnlySpan<byte> Write()
		{
			throw new NotImplementedException("");
		}
	}
}
