using NUnit.Framework;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Tests
{
	[TestFixture]
	public class G1Tests
	{
		readonly ILogger Logger = new Logger();

		[Test]
		public void LoadSaveLoadG1()
		{
			var g1File = "Q:\\Games\\Locomotion\\G1\\g1.dat";
			var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
			var tempName = Path.GetTempFileName();
			SawyerStreamWriter.SaveG1(tempName, g1);
			var g1a = SawyerStreamReader.LoadG1(tempName, Logger);

			Assert.That(g1.G1Header.NumEntries, Is.EqualTo(g1a.G1Header.NumEntries));
			//Assert.That(g1.G1Header.TotalSize, Is.EqualTo(g1a.G1Header.TotalSize));
			Assert.That(g1.G1Elements.Count, Is.EqualTo(g1a.G1Elements.Count));

			Assert.Multiple(() =>
			{
				foreach (var (a, b, i) in g1.G1Elements.Zip(g1a.G1Elements).Select((item, i) => (item.First, item.Second, i)))
				{
					//Assert.That(a.Offset, Is.EqualTo(b.Offset), $"[{i}]");
					Assert.That(a.Width, Is.EqualTo(b.Width), $"[{i}]");
					Assert.That(a.Height, Is.EqualTo(b.Height), $"[{i}]");
					Assert.That(a.XOffset, Is.EqualTo(b.XOffset), $"[{i}]");
					Assert.That(a.YOffset, Is.EqualTo(b.YOffset), $"[{i}]");
					Assert.That(a.Flags, Is.EqualTo(b.Flags), $"[{i}]");
					Assert.That(a.ZoomOffset, Is.EqualTo(b.ZoomOffset), $"[{i}]");
					Assert.That(a.ImageData, Is.EqualTo(b.ImageData), $"[{i}]");
				}
			});
		}

		// these elements seem to cause problems with the current decoding, but I"m not sure what is wrong
		[TestCase(3539)]
		[TestCase(3540)]
		[TestCase(3541)]
		[TestCase(3542)]
		[TestCase(3618)]
		[TestCase(3619)]
		[TestCase(3894)]
		public void LoadSaveLoadG1_EdgeCases(int element)
		{
			var g1File = "Q:\\Games\\Locomotion\\G1\\g1.dat";
			var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
			var d1 = g1!.G1Elements[element];
			var e1 = SawyerStreamWriter.EncodeRLEImageData(d1);
			var d2 = SawyerStreamReader.DecodeRLEImageData(d1 with { ImageData = e1 });

			Assert.That(d2, Is.EqualTo(d1.ImageData).AsCollection);
		}
	}
}
