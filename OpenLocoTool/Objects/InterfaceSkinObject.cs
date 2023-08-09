
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record InterfaceSkinObject(
		[property: LocoStructProperty] string_id Name, // 0x00
		[property: LocoStructProperty] uint32_t Image,   // 0x02
		[property: LocoStructProperty] Colour Colour_06,
		[property: LocoStructProperty] Colour Colour_07,
		[property: LocoStructProperty] Colour TooltipColour, // 0x08
		[property: LocoStructProperty] Colour ErrorColour,   // 0x09
		[property: LocoStructProperty] Colour Colour_0A,
		[property: LocoStructProperty] Colour Colour_0B,
		[property: LocoStructProperty] Colour Colour_0C,
		[property: LocoStructProperty] Colour Colour_0D,
		[property: LocoStructProperty] Colour Colour_0E,
		[property: LocoStructProperty] Colour Colour_0F,
		[property: LocoStructProperty] Colour Colour_10,
		[property: LocoStructProperty] Colour Colour_11,
		[property: LocoStructProperty] Colour Colour_12,
		[property: LocoStructProperty] Colour Colour_13,
		[property: LocoStructProperty] Colour Colour_14,
		[property: LocoStructProperty] Colour Colour_15,
		[property: LocoStructProperty] Colour Colour_16,
		[property: LocoStructProperty] Colour Colour_17
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.interfaceSkin;
		public int ObjectStructSize => 0x18;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
