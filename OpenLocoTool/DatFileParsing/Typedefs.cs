global using uint8_t = System.Byte;
global using int8_t = System.SByte;
global using uint16_t = System.UInt16;
global using int16_t = System.Int16;
global using int32_t = System.Int32;
global using uint32_t = System.UInt32;
global using string_id = System.UInt16;
global using Speed16 = System.Int16;
global using Speed32 = System.Int32;
global using MicroZ = System.Byte;
global using SoundObjectId = System.Byte;
using System.ComponentModel;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record Pos2(
		[property: LocoStructOffset(0x00)] int16_t X,
		[property: LocoStructOffset(0x02)] int16_t Y
		) : ILocoStruct
	{
		public static int StructSize => 0x04;
	}

	// this is only required because WinForms' DataGridView cannot handle plain old dictionary<foo, string>, because string type doesn't have the getter+setter it needs
	//public class StringTable
	//{
	//	Dictionary<string, StringTableInner> table = new();
	//}

	public class StringTable
	{
		public Dictionary<string, Dictionary<LanguageId, StringTableEntry>> table { get; set; } = new();

		public void Add(string key, Dictionary<LanguageId, StringTableEntry> value) => table.Add(key, value);

		public Dictionary<LanguageId, StringTableEntry> this[string key]
		{
			get => table[key];
			set => table[key] = value;
		}

		public Dictionary<string, Dictionary<LanguageId, StringTableEntry>>.KeyCollection Keys => table.Keys;
	}

	public class StringTableEntry
	{
		public string String { get; set; }
	}
}
