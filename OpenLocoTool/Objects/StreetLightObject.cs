using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record StreetLightObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02), LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear, // 0x2
		[property: LocoStructProperty(0x08)] uint32_t Image
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.streetLight;
		public static int ObjectStructSize => 0xC;
		public const int DesignedYearLength = 3;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
			=> throw new NotImplementedException();

		public ReadOnlySpan<byte> Write()
			=> throw new NotImplementedException();
	}
}
