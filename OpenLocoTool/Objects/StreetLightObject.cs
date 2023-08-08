using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record StreetLightObject(
		string Name,             // 0x0
		[LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear, // 0x2
		uint32_t Image           // 0x8
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.streetLight;
		public int ObjectStructSize => 0xC;
		public const int DesignedYearLength = 3;
		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";

			var byteReader = new ByteReader(data);
			return new StreetLightObject(
				name,
				byteReader.Read_Array<uint16_t>(DesignedYearLength),
				byteReader.Read<uint32_t>());
		}

		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
