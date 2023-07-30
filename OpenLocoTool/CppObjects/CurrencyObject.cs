using System.ComponentModel;

namespace OpenLocoTool.Objects
{
	// size = 0xC
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CurrencyObject
	(
		string Name,         // 0x00
		string PrefixSymbol, // 0x02
		string SuffixSymbol, // 0x04
		uint32_t ObjectIcon, // 0x06
		uint8_t Separator,   // 0x0A
		uint8_t Factor       // 0x0B
	)
	{
		public static CurrencyObject Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";
			var prefix = "todo: implement code to lookup string table";
			var suffix = "todo: implement code to lookup string table";

			var objectIcon = BitConverter.ToUInt32(data[0x06..0x0A]);
			var separator = data[0x0A];
			var factor = data[0x0B];

			return new CurrencyObject(name, prefix, suffix, objectIcon, separator, factor);
		}
	}
}
