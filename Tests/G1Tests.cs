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
				_ = Parallel.ForEach(g1.G1Elements.Zip(g1a.G1Elements), (pair, _, i) =>
				{
					var (a, b) = pair;
					//Assert.That(a.Offset, Is.EqualTo(b.Offset), $"[{i}]");
					Assert.That(a.Width, Is.EqualTo(b.Width), $"[{i}]");
					Assert.That(a.Height, Is.EqualTo(b.Height), $"[{i}]");
					Assert.That(a.XOffset, Is.EqualTo(b.XOffset), $"[{i}]");
					Assert.That(a.YOffset, Is.EqualTo(b.YOffset), $"[{i}]");
					//Assert.That(a.Flags, Is.EqualTo(b.Flags), $"[{i}]"); 
					Assert.That(a.ZoomOffset, Is.EqualTo(b.ZoomOffset), $"[{i}]");
					Assert.That(a.ImageData, Is.EqualTo(b.ImageData), $"[{i}]");
				});
			});
		}
	}
}
