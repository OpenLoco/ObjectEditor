using Common;
using NuGet.Versioning;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Tests;

[TestFixture]
public class VersioningTests
{
	static object[] TestInput =>
	[
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.Windows },
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.Linux },
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.OSX },
	];

	[TestCaseSource(nameof(TestInput))]
	public void TestUrlForDownload(SemanticVersion version, OSPlatform platform)
	{
		Assert.That(VersionHelpers.UrlForDownload(version, platform), Is.EqualTo($"https://github.com/OpenLoco/ObjectEditor/releases/download/{version}/object-editor-{version}-{PlatformSpecific.EditorPlatformZipName(platform)}"));
	}
}
