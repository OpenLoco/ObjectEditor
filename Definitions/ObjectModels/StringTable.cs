using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

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

	public Dictionary<LanguageId, string> AddNewString(string str)
	{
		Table.Add(str, GetNewLanguageDictionary());
		return Table[str];
	}

	static Dictionary<LanguageId, string> GetNewLanguageDictionary()
	{
		var languageDict = new Dictionary<LanguageId, string>();
		foreach (var language in Enum.GetValues<LanguageId>())
		{
			languageDict.Add(language, string.Empty);
		}

		return languageDict;
	}
}
