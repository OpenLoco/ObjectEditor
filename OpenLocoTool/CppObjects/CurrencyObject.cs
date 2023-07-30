using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0xC)]
	public struct CurrencyObject
	{
		//static constexpr auto kObjectType = ObjectType::currency;

		public string_id name { get; set; }         // 0x00
		public string_id prefixSymbol { get; set; } // 0x02
		public string_id suffixSymbol { get; set; } // 0x04
		public uint32_t objectIcon { get; set; }    // 0x06
		public uint8_t separator { get; set; }      // 0x0A
		public uint8_t factor { get; set; }         // 0x0B
	}
}
