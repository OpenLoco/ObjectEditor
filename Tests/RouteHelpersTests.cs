using NUnit.Framework;
using ObjectService.RouteHandlers;

namespace ObjectService.Tests;

[TestFixture]
public class RouteHelpersTests
{
	[Test]
	public void TryGetSafeRelativePathUnderRoot_ReturnsTrue_ForValidRelativePath()
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, "packs/object-pack.zip", out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.True);
				Assert.That(fullPath, Is.EqualTo(Path.Combine(rootPath, "packs", "object-pack.zip")));
				Assert.That(normalizedRelativePath, Is.EqualTo("packs/object-pack.zip"));
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[TestCase(null)]
	[TestCase("")]
	[TestCase("../packs/object-pack.zip")]
	[TestCase("packs/./object-pack.zip")]
	[TestCase("packs//object-pack.zip")]
	public void TryGetSafeRelativePathUnderRoot_ReturnsFalse_ForUnsafeRelativePath(string? relativePath)
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, relativePath, out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.False);
				Assert.That(fullPath, Is.Empty);
				Assert.That(normalizedRelativePath, Is.Empty);
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[Test]
	public void TryGetSafeRelativePathUnderRoot_ReturnsFalse_ForAbsolutePath()
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);
			var absolutePath = Path.Combine(rootPath, "packs", "object-pack.zip");

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, absolutePath, out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.False);
				Assert.That(fullPath, Is.Empty);
				Assert.That(normalizedRelativePath, Is.Empty);
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[TestCase("..\\packs\\object-pack.zip")]
	[TestCase("packs\\..\\..\\object-pack.zip")]
	[TestCase("packs/../../object-pack.zip")]
	[TestCase("packs/../../../etc/passwd")]
	[TestCase("..")]
	[TestCase("./")]
	[TestCase(".")]
	[TestCase("../")]
	[TestCase("packs/../object-pack.zip")]
	[TestCase("   ")]
	[TestCase("\t")]
	public void TryGetSafeRelativePathUnderRoot_ReturnsFalse_ForTraversalAttempts(string relativePath)
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, relativePath, out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.False, $"input '{relativePath}' should be rejected");
				Assert.That(fullPath, Is.Empty);
				Assert.That(normalizedRelativePath, Is.Empty);
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[TestCase("packs\\object-pack.zip", "packs/object-pack.zip")]
	[TestCase("a/b/c/d.dat", "a/b/c/d.dat")]
	[TestCase("a\\b\\c\\d.dat", "a/b/c/d.dat")]
	[TestCase("nested/deeply/inside/folder/file.txt", "nested/deeply/inside/folder/file.txt")]
	[TestCase("file with spaces.dat", "file with spaces.dat")]
	[TestCase("UPPER_case-file.DAT", "UPPER_case-file.DAT")]
	[TestCase("unicode_éñ_文件.dat", "unicode_éñ_文件.dat")]
	public void TryGetSafeRelativePathUnderRoot_NormalizesSeparators(string input, string expectedNormalized)
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, input, out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.True);
				Assert.That(normalizedRelativePath, Is.EqualTo(expectedNormalized));
				Assert.That(fullPath, Does.StartWith(rootPath));
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[Test]
	public void TryGetSafeRelativePathUnderRoot_ReturnsFalse_ForUncPath()
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, @"\\server\share\file.dat", out var fullPath, out var normalizedRelativePath);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.False);
				Assert.That(fullPath, Is.Empty);
				Assert.That(normalizedRelativePath, Is.Empty);
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[Test]
	public void TryGetSafeRelativePathUnderRoot_FullPathStaysUnderRoot()
	{
		var rootPath = Path.Combine(Path.GetTempPath(), $"route-helper-{Guid.NewGuid():N}");

		try
		{
			Directory.CreateDirectory(rootPath);

			var result = RouteHelpers.TryGetSafeRelativePathUnderRoot(rootPath, "subfolder/file.dat", out var fullPath, out _);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.True);
				Assert.That(fullPath, Is.EqualTo(Path.Combine(rootPath, "subfolder", "file.dat")));
				Assert.That(Path.IsPathFullyQualified(fullPath), Is.True);
			}
		}
		finally
		{
			Directory.Delete(rootPath, recursive: true);
		}
	}

	[TestCase(null, ".zip", "fallback", "fallback.zip")]
	[TestCase("", ".zip", "fallback", "fallback.zip")]
	[TestCase("   ", ".zip", "fallback", "fallback.zip")]
	[TestCase("simple", ".dat", "fallback", "simple.dat")]
	[TestCase("simple", "dat", "fallback", "simple.dat")] // extension without leading dot is added
	[TestCase("simple", "", "fallback", "simple")]
	[TestCase("My Pack", ".zip", "fallback", "My Pack.zip")]
	[TestCase("path/with/slashes", ".zip", "fallback", "path_with_slashes.zip")]
	[TestCase("path\\with\\backslashes", ".zip", "fallback", "path_with_backslashes.zip")]
	[TestCase("evil:colon", ".zip", "fallback", "evil_colon.zip")]
	[TestCase("with*star?and|pipe", ".zip", "fallback", "with_star_and_pipe.zip")]
	[TestCase("<script>", ".zip", "fallback", "_script_.zip")]
	[TestCase("...trim.dots...", ".zip", "fallback", "trim.dots.zip")]
	[TestCase("  leading and trailing  ", ".zip", "fallback", "leading and trailing.zip")]
	[TestCase("multiple\t\twhitespace\n\nblocks", ".zip", "fallback", "multiple__whitespace__blocks.zip")] // tab/newline are control chars -> '_', not whitespace
	[TestCase("runs   of   spaces", ".zip", "fallback", "runs of spaces.zip")] // real space runs ARE collapsed
	[TestCase("ctrl\u0001\u0002char", ".zip", "fallback", "ctrl__char.zip")]
	public void MakeSafeHttpDownloadFileName_SanitizesInput(string? baseName, string extension, string fallback, string expected)
	{
		var result = RouteHelpers.MakeSafeHttpDownloadFileName(baseName, extension, fallback);
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public void MakeSafeHttpDownloadFileName_PathTraversalAttemptIsNeutralised()
	{
		var result = RouteHelpers.MakeSafeHttpDownloadFileName("../../etc/passwd", ".dat", "fallback");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Does.Not.Contain("/"));
			Assert.That(result, Does.Not.Contain("\\"));
			Assert.That(result, Does.Not.StartWith("."));
			Assert.That(result, Does.EndWith(".dat"));
		}
	}

	[Test]
	public void MakeSafeHttpDownloadFileName_NullByteIsReplaced()
	{
		var result = RouteHelpers.MakeSafeHttpDownloadFileName("name\0with\0nulls", ".zip", "fallback");
		Assert.That(result, Is.EqualTo("name_with_nulls.zip"));
	}

	[TestCase("MyPack", "s", "MyPack.s")]
	[TestCase("MyPack", ".s", "MyPack.s")]
	public void MakeSafeHttpDownloadFileName_NormalisesLeadingDotOnExtension(string baseName, string extension, string expected)
	{
		var result = RouteHelpers.MakeSafeHttpDownloadFileName(baseName, extension, "fallback");
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public void MakeNicePlural_StripsRouteHandlerSuffix()
	{
		Assert.That(RouteHelpers.MakeNicePlural("ObjectRouteHandler"), Is.EqualTo("Objects"));
		Assert.That(RouteHelpers.MakeNicePlural("Tag"), Is.EqualTo("Tags"));
		Assert.That(RouteHelpers.MakeNicePlural("UserRouteHandler"), Is.EqualTo("Users"));
	}
}
