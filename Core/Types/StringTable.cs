using OpenLoco.ObjectEditor.Data;
using System.ComponentModel;

namespace OpenLoco.ObjectEditor.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StringTable
	{
		public StringTable()
			=> Table = [];

		public StringTable(Dictionary<string, Dictionary<LanguageId, string>> table)
			=> Table = table;

		public Dictionary<string, Dictionary<LanguageId, string>> Table { get; set; } = [];

		public Dictionary<LanguageId, string> this[string key]
		{
			get => Table[key];
			set => Table[key] = value;
		}
	}
}
