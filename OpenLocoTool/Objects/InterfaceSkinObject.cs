
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record InterfaceSkinObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint32_t Image,
		[property: LocoStructProperty(0x06)] Colour Colour_06,
		[property: LocoStructProperty(0x07)] Colour Colour_07,
		[property: LocoStructProperty(0x08)] Colour TooltipColour,
		[property: LocoStructProperty(0x09)] Colour ErrorColour,
		[property: LocoStructProperty(0x0A)] Colour Colour_0A,
		[property: LocoStructProperty(0x0B)] Colour Colour_0B,
		[property: LocoStructProperty(0x0C)] Colour Colour_0C,
		[property: LocoStructProperty(0x0D)] Colour Colour_0D,
		[property: LocoStructProperty(0x0E)] Colour Colour_0E,
		[property: LocoStructProperty(0x0F)] Colour Colour_0F,
		[property: LocoStructProperty(0x10)] Colour Colour_10,
		[property: LocoStructProperty(0x11)] Colour Colour_11,
		[property: LocoStructProperty(0x12)] Colour Colour_12,
		[property: LocoStructProperty(0x13)] Colour Colour_13,
		[property: LocoStructProperty(0x14)] Colour Colour_14,
		[property: LocoStructProperty(0x15)] Colour Colour_15,
		[property: LocoStructProperty(0x16)] Colour Colour_16,
		[property: LocoStructProperty(0x17)] Colour Colour_17
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.interfaceSkin;
		public static int ObjectStructSize => 0x18;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
