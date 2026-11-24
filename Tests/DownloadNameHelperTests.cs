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

	[TestCase(null, "fallback")]
	[TestCase("", "fallback")]
	[TestCase("   ", "fallback")]
	[TestCase(".", "fallback")]
	[TestCase("..", "fallback")] // trimmed to empty -> fallback
	[TestCase("...", "fallback")]
	public void SanitizeBaseName_UsesFallback_ForEmptyInputs(string? input, string expected)
	{
		var result = DownloadNameHelper.SanitizeBaseName(input, stripDirectoryComponents: true, stripExtension: false, fallbackName: "fallback");
		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCase("a..b", "a__b")]
	[TestCase("foo..bar..baz", "foo__bar__baz")]
	public void SanitizeBaseName_NeutralisesParentTraversalSequencesWithinName(string input, string expected)
	{
		var result = DownloadNameHelper.SanitizeBaseName(input, stripDirectoryComponents: false, stripExtension: false, fallbackName: "fallback");
		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCase("../../etc/passwd", true, false, "passwd")] // dir components stripped
	[TestCase("..\\..\\windows\\system32\\cmd.exe", true, true, "cmd")]
	public void SanitizeBaseName_BlocksPathTraversal(string input, bool stripDirs, bool stripExt, string expected)
	{
		var result = DownloadNameHelper.SanitizeBaseName(input, stripDirs, stripExt, "fallback");
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public void SanitizeBaseName_NoDirStrip_NeutralisesSeparatorsAndTraversal()
	{
		// without stripping dir components, separators become _ and ".." sequences become __
		var result = DownloadNameHelper.SanitizeBaseName("../../sneaky", stripDirectoryComponents: false, stripExtension: false, fallbackName: "fallback");
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Does.Not.Contain("/"));
			Assert.That(result, Does.Not.Contain("\\"));
			Assert.That(result, Does.Not.Contain(".."));
			Assert.That(result, Does.EndWith("sneaky"));
		}
	}

	[Test]
	public void SanitizeBaseName_RemovesControlAndInvalidFileChars()
	{
		var result = DownloadNameHelper.SanitizeBaseName("a\0b\rc:d*e?f|g\"h<i>j", stripDirectoryComponents: false, stripExtension: false, fallbackName: "fallback");
		Assert.That(result, Is.EqualTo("a_b_c_d_e_f_g_h_i_j"));
	}

	[TestCase("name", null, "fallback", "name")] // null extension is normalized to empty
	[TestCase("name", "", "fallback", "name")]
	[TestCase("name", "dat", "fallback", "name.dat")]
	[TestCase("name", ".dat", "fallback", "name.dat")]
	[TestCase("name.zip", ".dat", "fallback", "name.dat")] // strips and replaces
	public void MakeSafeDownloadFileName_ExtensionHandling(string? input, string? extension, string fallback, string expected)
	{
		var result = DownloadNameHelper.MakeSafeDownloadFileName(input, extension ?? string.Empty, fallback);
		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCase("  leading and trailing spaces  ", "leading and trailing spaces")]
	[TestCase(" . . . ", "fallback")] // all dots/spaces -> trimmed away
	public void SanitizeBaseName_TrimsSpacesAndDots(string input, string expected)
	{
		var result = DownloadNameHelper.SanitizeBaseName(input, stripDirectoryComponents: false, stripExtension: false, fallbackName: "fallback");
		Assert.That(result, Is.EqualTo(expected));
	}
}
