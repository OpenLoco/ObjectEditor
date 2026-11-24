namespace Common;

public static class DownloadNameHelper
{
	static readonly char[] PathSeparators = ['/', '\\'];

	public static string SanitizeBaseName(string? name, bool stripDirectoryComponents, bool stripExtension, string fallbackName)
	{
		var sanitizedName = name ?? string.Empty;

		if (stripDirectoryComponents)
		{
			sanitizedName = sanitizedName.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
		}

		if (stripExtension)
		{
			sanitizedName = Path.GetFileNameWithoutExtension(sanitizedName);
		}

		sanitizedName = string.Concat(sanitizedName.Select(x => char.IsControl(x) ? '_' : x));
		sanitizedName = Path.GetInvalidFileNameChars().Aggregate(sanitizedName, (current, c) => current.Replace(c, '_'));
		sanitizedName = sanitizedName
			.Replace('/', '_')
			.Replace('\\', '_')
			.Trim(' ', '.')
			.Replace("..", "__", StringComparison.Ordinal);

		return string.IsNullOrWhiteSpace(sanitizedName) ? fallbackName : sanitizedName;
	}

	public static string MakeSafeDownloadFileName(string? name, string extension, string fallbackBaseName, bool stripDirectoryComponents = true, bool stripExtension = true)
	{
		var normalizedExtension = string.IsNullOrEmpty(extension)
			? string.Empty
			: extension.StartsWith('.')
				? extension
				: $".{extension}";

		var sanitizedBaseName = SanitizeBaseName(name, stripDirectoryComponents, stripExtension, fallbackBaseName);
		return $"{sanitizedBaseName}{normalizedExtension}";
	}
}
