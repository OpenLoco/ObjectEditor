using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xC)]
	struct CurrencyObject
	{
		//static constexpr auto kObjectType = ObjectType::currency;

		string_id name;         // 0x00
		string_id prefixSymbol; // 0x02
		string_id suffixSymbol; // 0x04
		uint32_t objectIcon;    // 0x06
		uint8_t separator;      // 0x0A
		uint8_t factor;         // 0x0B
	}
}
