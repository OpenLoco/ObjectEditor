using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CurrencyObject
	(
		[property: LocoStructProperty] string_id Name,         // 0x00
		[property: LocoStructProperty] string_id PrefixSymbol, // 0x02
		[property: LocoStructProperty] string_id SuffixSymbol, // 0x04
		[property: LocoStructProperty] uint32_t ObjectIcon, // 0x06
		[property: LocoStructProperty] uint8_t Separator,   // 0x0A
		[property: LocoStructProperty] uint8_t Factor       // 0x0B
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.currency;

		public int ObjectStructSize => 0xC;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			//var name = "todo: implement code to lookup string table";
			//var prefix = "todo: implement code to lookup string table";
			//var suffix = "todo: implement code to lookup string table";

			//var objectIcon = BitConverter.ToUInt32(data[0x06..0x0A]);
			//var separator = data[0x0A];
			//var factor = data[0x0B];

			//return new CurrencyObject(name, prefix, suffix, objectIcon, separator, factor);
			throw new NotImplementedException("");
		}

		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
