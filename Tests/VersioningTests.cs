using Common;
using NuGet.Versioning;
using NUnit.Framework;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Tests;

[TestFixture]
public class VersioningTests
{
	static object[] PlatformInput =>
	[
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.Windows },
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.Linux },
		new object[]{ new SemanticVersion(1, 0, 0), OSPlatform.OSX },
	];

	static object[] PlatformAndArchitectureInput =>
	[
		new object[]{ OSPlatform.Windows, Architecture.X64, "win-x64.zip" },
		new object[]{ OSPlatform.Windows, Architecture.Arm64, "win-arm64.zip" },
		new object[]{ OSPlatform.Linux, Architecture.X64, "linux-x64.tar.gz" },
		new object[]{ OSPlatform.Linux, Architecture.Arm64, "linux-arm64.tar.gz" },
		new object[]{ OSPlatform.OSX, Architecture.X64, "osx-x64.zip" },
		new object[]{ OSPlatform.OSX, Architecture.Arm64, "osx-arm64.zip" },
	];

	static object[] UpdaterNameInput =>
	[
		new object[]{ OSPlatform.Windows, "ObjectEditorUpdater.exe" },
		new object[]{ OSPlatform.Linux, "ObjectEditorUpdater" },
		new object[]{ OSPlatform.OSX, "ObjectEditorUpdater" },
	];

	[TestCaseSource(nameof(PlatformInput))]
	public void TestUrlForDownload(SemanticVersion version, OSPlatform platform)
	{
		Assert.That(VersionHelpers.UrlForDownload(version, platform), Is.EqualTo($"https://github.com/OpenLoco/ObjectEditor/releases/download/{version}/object-editor-{version}-{PlatformSpecific.EditorPlatformZipName(platform)}"));
	}

	[TestCaseSource(nameof(PlatformAndArchitectureInput))]
	public void TestEditorPlatformZipName(OSPlatform platform, Architecture architecture, string expected)
	{
		Assert.That(PlatformSpecific.EditorPlatformZipName(platform, architecture), Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(PlatformAndArchitectureInput))]
	public void TestUrlForDownloadMatchesBuildArtifact(OSPlatform platform, Architecture architecture, string expected)
	{
		var version = new SemanticVersion(1, 2, 3);

		// the URL the updater downloads must be the exact file name build.sh produces
		Assert.That(VersionHelpers.UrlForDownload(version, platform, architecture), Is.EqualTo($"https://github.com/OpenLoco/ObjectEditor/releases/download/{version}/object-editor-{version}-{expected}"));
	}

	[TestCaseSource(nameof(UpdaterNameInput))]
	public void TestEditorPlatformUpdaterName(OSPlatform platform, string expected)
	{
		Assert.That(PlatformSpecific.EditorPlatformUpdaterName(platform), Is.EqualTo(expected));
	}

	[Test]
	public void TestUnsupportedArchitectureThrows()
	{
		Assert.Throws<PlatformNotSupportedException>(() => PlatformSpecific.EditorPlatformZipName(OSPlatform.Windows, Architecture.X86));
	}

	[Test]
	public void TestCurrentAppVersionComesFromAssembly()
	{
		// the version is baked in by MinVer from the git tag, so it must be a real version and must match the
		// informational version of this product's assemblies
		var version = VersionHelpers.GetCurrentAppVersion();

		Assert.That(version, Is.Not.EqualTo(VersionHelpers.UnknownVersion));
		Assert.That(version.Major, Is.GreaterThan(0), "the version should come from the git tag, not a placeholder");

		var informationalVersion = typeof(VersionHelpers).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion?
			.Split('+')[0];

		Assert.That(version.ToString(), Is.EqualTo(informationalVersion));
	}
}
