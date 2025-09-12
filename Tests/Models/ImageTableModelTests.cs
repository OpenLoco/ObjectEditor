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
}
