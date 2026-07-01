namespace Common;

public static class IEnumerableExtensions
{
	public static IEnumerable<T> Fill<T>(this IEnumerable<T> source, int minLength, T fillValue)
	{
		var i = 0;
		foreach (var item in source)
		{
			i++;
			yield return item;
		}

		while (i < minLength)
		{
			i++;
			yield return fillValue;
		}
	}

	public static bool HasDuplicates<T>(this IEnumerable<T> source)
	{
		if (source is ICollection<T> collection && collection.Count <= 1)
		{
			return false;
		}

		var seen = new HashSet<T>();
		foreach (var item in source)
		{
			if (!seen.Add(item))
			{
				return true;
			}

		}

		return false;
	}

	public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source)
	{
		var seen = new HashSet<T>();
		var duplicates = new HashSet<T>();

		foreach (var item in source)
		{
			if (!seen.Add(item))
			{
				if (duplicates.Add(item))
				{
					yield return item;
				}
			}
		}
	}
}
