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

}
