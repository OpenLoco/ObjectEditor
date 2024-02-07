using System.ComponentModel;

namespace OpenLocoObjectEditor.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StringTable
	{
		public Dictionary<string, Dictionary<LanguageId, string>> Table { get; set; } = [];

		public Dictionary<LanguageId, string> this[string key]
		{
			get => Table[key];
			set => Table[key] = value;
		}
	}
}
