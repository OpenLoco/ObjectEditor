using NUnit.Framework;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using SixLabors.ImageSharp;

namespace OpenLoco.Dat.Tests
{
	[TestFixture]
	public class G1Tests
	{
		readonly ILogger Logger = new Logger();
		const string g1File = "Q:\\Games\\Locomotion\\G1\\g1.dat";

		[Test]
		public void LoadSaveLoadG1()
		{
			var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
			var tempName = Path.GetTempFileName();
			SawyerStreamWriter.SaveG1(tempName, g1);
			var g1a = SawyerStreamReader.LoadG1(tempName, Logger);

			Assert.That(g1.G1Header.NumEntries, Is.EqualTo(g1a.G1Header.NumEntries));
			//Assert.That(g1.G1Header.TotalSize, Is.EqualTo(g1a.G1Header.TotalSize));
			Assert.That(g1.G1Elements.Count, Is.EqualTo(g1a.G1Elements.Count));

			Assert.Multiple(() =>
			{
				foreach (var (expected, actual, i) in g1.G1Elements.Zip(g1a.G1Elements).Select((item, i) => (item.First, item.Second, i)))
				{
					// debugging
					if (i == 3894)
					{
						var paletteFile = Path.Combine(ImagePaletteConversionTests.BasePalettePath, ImagePaletteConversionTests.PaletteFileName);
						var pm = new PaletteMap(paletteFile);
						if (pm.TryConvertG1ToRgba32Bitmap(g1.G1Elements[3894], out var i1))
						{
							i1.Save("i1.png");
						}
						if (pm.TryConvertG1ToRgba32Bitmap(g1a.G1Elements[3894], out var i2))
						{
							i2.Save("i2.png");
						}
					}
					//

					AssertG1ElementsEqual(expected, actual, i);
				}
			});
		}

		// These images have RLE runs/segment lengths > 127, which require special handling in the encode
		// method. I split these out to initially debug why they weren't working but I will leave these tests
		// in as they serve as a kind of documentation of this quirk of the g1 encoding.
		[TestCase(3539)]
		[TestCase(3540)]
		[TestCase(3541)]
		[TestCase(3542)]
		[TestCase(3618)]
		[TestCase(3619)]
		public void LoadSaveLoadG1_RLERunsGreaterThan127(int element)
		{
			var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
			var d1 = g1!.G1Elements[element];
			var e1 = SawyerStreamWriter.EncodeRLEImageData(d1);
			var d2 = SawyerStreamReader.DecodeRLEImageData(d1 with { ImageData = e1 });
			Assert.That(d2, Is.EqualTo(d1.ImageData).AsCollection);
		}

		public void AssertG1ElementsEqual(G1Element32 expected, G1Element32 actual, int i)
		{
			//Assert.That(a.Offset, Is.EqualTo(b.Offset), $"[{i}]");
			Assert.That(actual.Width, Is.EqualTo(expected.Width), $"[{i}]");
			Assert.That(actual.Height, Is.EqualTo(expected.Height), $"[{i}]");
			Assert.That(actual.XOffset, Is.EqualTo(expected.XOffset), $"[{i}]");
			Assert.That(actual.YOffset, Is.EqualTo(expected.YOffset), $"[{i}]");
			Assert.That(actual.Flags, Is.EqualTo(expected.Flags), $"[{i}]");
			Assert.That(actual.ZoomOffset, Is.EqualTo(expected.ZoomOffset), $"[{i}]");
			Assert.That(actual.ImageData, Is.EqualTo(expected.ImageData).AsCollection, $"[{i}]");
		}
	}
}
