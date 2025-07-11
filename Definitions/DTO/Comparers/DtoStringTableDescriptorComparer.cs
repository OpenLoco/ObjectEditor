using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoStringTableDescriptorComparer : IEqualityComparer<DtoStringTableDescriptor>
{
	public bool Equals(DtoStringTableDescriptor? x, DtoStringTableDescriptor? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		if (x.ObjectId != y.ObjectId)
		{
			return false;
		}

		if (x.Table.Count != y.Table.Count)
		{
			return false;
		}

		foreach (var key in x.Table.Keys)
		{
			if (!y.Table.ContainsKey(key))
			{
				return false;
			}

			var xLangDict = x.Table[key];
			var yLangDict = y.Table[key];

			if (xLangDict.Count != yLangDict.Count)
			{
				return false;
			}

			foreach (var lang in xLangDict.Keys)
			{
				if (!yLangDict.ContainsKey(lang))
				{
					return false;
				}

				if (xLangDict[lang] != yLangDict[lang])
				{
					return false;
				}
			}
		}

		return true;
	}

	public int GetHashCode([DisallowNull] DtoStringTableDescriptor obj)
	{
		var hash = obj.ObjectId.GetHashCode();
		foreach (var (rowName, langDict) in obj.Table)
		{
			hash = HashCode.Combine(hash, rowName);
			foreach (var (lang, text) in langDict)
			{
				hash = HashCode.Combine(hash, lang, text);
			}
		}
		return hash;
	}
}
