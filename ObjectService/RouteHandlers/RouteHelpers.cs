namespace ObjectService.RouteHandlers;

public static class RouteHelpers
{
	static readonly StringComparison PathComparison = OperatingSystem.IsWindows()
		? StringComparison.OrdinalIgnoreCase
		: StringComparison.Ordinal;
	static readonly char[] PathSeparators = ['/', '\\'];
	static readonly char[] HttpUnsafeFilenameChars = ['/', '\\', ':', '*', '?', '"', '<', '>', '|', '\0'];

	public static string MakeNicePlural(string name)
		=> $"{name.Replace("RouteHandler", string.Empty)}s";

	public static bool TryGetSafeRelativePathUnderRoot(string rootPath, string? relativePath, out string fullPath, out string normalizedRelativePath)
	{
		fullPath = string.Empty;
		normalizedRelativePath = string.Empty;

		if (string.IsNullOrWhiteSpace(relativePath) || Path.IsPathRooted(relativePath))
		{
			return false;
		}

		var segments = relativePath.Split(PathSeparators, StringSplitOptions.None);
		if (segments.Any(x => string.IsNullOrEmpty(x) || x is "." or ".."))
		{
			return false;
		}

		var rootFullPath = Path.GetFullPath(rootPath);
		if (!rootFullPath.EndsWith(Path.DirectorySeparatorChar))
		{
			rootFullPath += Path.DirectorySeparatorChar;
		}

		var combinedPath = Path.Combine(rootFullPath, relativePath);
		var candidateFullPath = Path.GetFullPath(combinedPath);
		if (!candidateFullPath.StartsWith(rootFullPath, PathComparison))
		{
			return false;
		}

		fullPath = candidateFullPath;
		normalizedRelativePath = string.Join('/', segments);
		return true;
	}

	public static string MakeSafeHttpDownloadFileName(string? baseName, string extension, string fallbackBaseName)
	{
		var normalizedExtension = string.IsNullOrWhiteSpace(extension)
			? string.Empty
			: extension.StartsWith('.') ? extension : $".{extension}";

		var sanitizedBaseName = new string([.. (baseName ?? string.Empty)
			.Select(c =>
			{
				if (char.IsControl(c) || HttpUnsafeFilenameChars.Contains(c))
				{
					return '_';
				}

				if (char.IsWhiteSpace(c))
				{
					return ' ';
				}

				return c is >= ' ' and <= '~' ? c : '_';
			})]);

		var collapsedWhitespace = string.Join(' ', sanitizedBaseName.Split(' ', StringSplitOptions.RemoveEmptyEntries));
		sanitizedBaseName = collapsedWhitespace.Trim(' ', '.');
		if (string.IsNullOrWhiteSpace(sanitizedBaseName))
		{
			sanitizedBaseName = fallbackBaseName;
		}

		return $"{sanitizedBaseName}{normalizedExtension}";
	}
}
