using Dat.Data;
using System.ComponentModel;

namespace Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StringTable
	{
		public StringTable()
			=> Table = [];

		public StringTable(Dictionary<string, Dictionary<LanguageId, string>> table)
			=> Table = table;

		public Dictionary<string, Dictionary<LanguageId, string>> Table { get; } = [];

		public Dictionary<LanguageId, string> this[string key]
		{
			get => Table[key];
			set => Table[key] = value;
		}
	}
}
