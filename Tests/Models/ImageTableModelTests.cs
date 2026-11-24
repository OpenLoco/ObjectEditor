using Core.Graphics;
using Gui.ViewModels.Graphics;
using NUnit.Framework;

namespace Models.Tests;

[TestFixture]
public class ImageTableModelTests
{
	[Test]
	public void TrimZeroes()
		=> Assert.Multiple(() =>
		{
			Assert.That(ImageTableViewModel.TrimZeroes("123"), Is.EqualTo("123"), "base case");
			Assert.That(ImageTableViewModel.TrimZeroes("1230"), Is.EqualTo("1230"), "trailing zero");
			Assert.That(ImageTableViewModel.TrimZeroes("1203"), Is.EqualTo("1203"), "middle zero");
			Assert.That(ImageTableViewModel.TrimZeroes("  987 "), Is.EqualTo("987"), "whitespace zero");

			Assert.That(ImageTableViewModel.TrimZeroes("001"), Is.EqualTo("1"), "leading zero");
			Assert.That(ImageTableViewModel.TrimZeroes("0"), Is.EqualTo("0"), "single zero");
			Assert.That(ImageTableViewModel.TrimZeroes("0000"), Is.EqualTo("0"), "multiple zeroes");
		});

	[Test]
	public void BuildSingleImageFileName_UsesIndexAndSanitisedName()
		=> Assert.Multiple(() =>
		{
			Assert.That(ImageTableIo.BuildSingleImageFileName(42, "body-0"), Is.EqualTo("42_body-0.png"), "normal name");
			Assert.That(ImageTableIo.BuildSingleImageFileName(42, "Body 0"), Is.EqualTo("42_body-0.png"), "spaces become dashes and name is lowercased");
			Assert.That(ImageTableIo.BuildSingleImageFileName(7, "  Spaced  "), Is.EqualTo("7_spaced.png"), "surrounding whitespace is trimmed");
		});

	[Test]
	public void BuildSingleImageFileName_FallsBackToIndexWhenNameIsEmpty()
		=> Assert.Multiple(() =>
		{
			Assert.That(ImageTableIo.BuildSingleImageFileName(42, null), Is.EqualTo("42.png"), "null name");
			Assert.That(ImageTableIo.BuildSingleImageFileName(42, ""), Is.EqualTo("42.png"), "empty name");
			Assert.That(ImageTableIo.BuildSingleImageFileName(42, "   "), Is.EqualTo("42.png"), "whitespace-only name");
		});

	[Test]
	public void SanitizeFileName_RemovesInvalidCharacters()
	{
		var invalid = System.IO.Path.GetInvalidFileNameChars().First();
		Assert.That(ImageTableIo.SanitizeFileName($"a{invalid}b"), Is.EqualTo("ab"));
	}
}
