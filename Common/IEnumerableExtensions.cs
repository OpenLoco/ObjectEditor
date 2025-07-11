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
}
