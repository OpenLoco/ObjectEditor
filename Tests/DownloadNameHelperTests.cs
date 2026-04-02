using Common;
using NUnit.Framework;

namespace ObjectService.Tests;

[TestFixture]
public class DownloadNameHelperTests
{
	[Test]
	public void SanitizeBaseName_StripsDirectoryComponentsAndExtension()
	{
		var sanitizedName = DownloadNameHelper.SanitizeBaseName("packs/custom/object-pack.zip", stripDirectoryComponents: true, stripExtension: true, fallbackName: "download");

		Assert.That(sanitizedName, Is.EqualTo("object-pack"));
	}

	[Test]
	public void SanitizeBaseName_PreservesExtensionText_WhenNotStrippingExtension()
	{
		var sanitizedName = DownloadNameHelper.SanitizeBaseName("my pack.zip", stripDirectoryComponents: false, stripExtension: false, fallbackName: "pack");

		Assert.That(sanitizedName, Is.EqualTo("my pack.zip"));
	}

	[Test]
	public void MakeSafeDownloadFileName_ReplacesControlCharacters_AndFallsBackWhenEmpty()
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(DownloadNameHelper.MakeSafeDownloadFileName("unsafe\rname.zip", ".zip", "object-pack"), Is.EqualTo("unsafe_name.zip"));
			Assert.That(DownloadNameHelper.MakeSafeDownloadFileName("...", ".zip", "object-pack"), Is.EqualTo("object-pack.zip"));
		}
	}

	[Test]
	public void MakeSafeDownloadFileName_DoesNotStripDirectoryComponents_WhenDisabled()
	{
		var fileName = DownloadNameHelper.MakeSafeDownloadFileName("packs/custom", ".zip", "scenario-pack", stripDirectoryComponents: false, stripExtension: false);

		Assert.That(fileName, Is.EqualTo("packs_custom.zip"));
	}
}