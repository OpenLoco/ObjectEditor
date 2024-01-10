using System.ComponentModel;

namespace OpenLocoTool.DatFileParsing
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StringTable
	{
		public Dictionary<string, Dictionary<LanguageId, string>> table { get; set; } = [];

		public void Add(string key, Dictionary<LanguageId, string> value) => table.Add(key, value);

		public Dictionary<LanguageId, string> this[string key]
		{
			get => table[key];
			set => table[key] = value;
		}

		public Dictionary<string, Dictionary<LanguageId, string>>.KeyCollection Keys => table.Keys;
	}
}
