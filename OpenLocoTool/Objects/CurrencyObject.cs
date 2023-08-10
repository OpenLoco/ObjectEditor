using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CurrencyObject
	(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] string_id PrefixSymbol,
		[property: LocoStructProperty(0x04)] string_id SuffixSymbol,
		[property: LocoStructProperty(0x06)] uint32_t ObjectIcon,
		[property: LocoStructProperty(0x0A)] uint8_t Separator,
		[property: LocoStructProperty(0x0B)] uint8_t Factor
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.currency;

		public static int ObjectStructSize => 0xC;

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
