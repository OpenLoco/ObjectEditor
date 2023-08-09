using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record StreetLightObject(
		[property: LocoStructProperty] string_id Name,             // 0x0
		[property: LocoStructProperty, LocoArrayLength(StreetLightObject.DesignedYearLength)] uint16_t[] DesignedYear, // 0x2
		[property: LocoStructProperty] uint32_t Image           // 0x8
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.streetLight;
		public int ObjectStructSize => 0xC;
		public const int DesignedYearLength = 3;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
			=> throw new NotImplementedException();

		public ReadOnlySpan<byte> Write()
			=> throw new NotImplementedException();
	}
}
