using Common.Logging;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels.Graphics;
using NUnit.Framework;
using SixLabors.ImageSharp;

namespace Dat.Tests;

[TestFixture]
public class G1Tests
{
	readonly ILogger Logger = new Logger();
	const string Steam_G1 = "Q:\\Games\\Locomotion\\G1\\steam-g1.dat"; // todo: check both steam and gog
	const string GoG_G1 = "Q:\\Games\\Locomotion\\G1\\gog-g1.dat"; // todo: check both steam and gog 

	[TestCase(Steam_G1)]
	[TestCase(GoG_G1)]
	public void LoadSaveLoadG1(string g1File)
	{
		var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
		ArgumentNullException.ThrowIfNull(g1);

		var tempName = Path.GetTempFileName();
		SawyerStreamWriter.SaveG1(tempName, g1);
		var g1a = SawyerStreamReader.LoadG1(tempName, Logger);

		ArgumentNullException.ThrowIfNull(g1a);
		using (Assert.EnterMultipleScope())
		{
			Assert.That(g1.G1Header.NumEntries, Is.EqualTo(g1a.G1Header.NumEntries));
			Assert.That(g1.G1Header.TotalSize, Is.EqualTo(g1a.G1Header.TotalSize));
			Assert.That(g1.ImageTable.GraphicsElements, Has.Count.EqualTo(g1a.ImageTable.GraphicsElements.Count));
		}

		using (Assert.EnterMultipleScope())
		{
			foreach (var (expected, actual, i) in g1.ImageTable.GraphicsElements.Zip(g1a.ImageTable.GraphicsElements).Select((item, i) => (item.First, item.Second, i)))
			{
				AssertG1ElementsEqual(expected, actual, i);
			}
		}
	}

	// These images have RLE runs/segment lengths > 127, which require special handling in the encode
	// method. I split these out to initially debug why they weren't working. The code now works but
	// I will leave these tests in as they serve as a kind of documentation of this quirk of the g1 encoding.
	[TestCase(Steam_G1, 3539)]
	[TestCase(Steam_G1, 3540)]
	[TestCase(Steam_G1, 3541)]
	[TestCase(Steam_G1, 3542)]
	[TestCase(Steam_G1, 3618)]
	[TestCase(Steam_G1, 3619)]
	[TestCase(GoG_G1, 3539)]
	[TestCase(GoG_G1, 3540)]
	[TestCase(GoG_G1, 3541)]
	[TestCase(GoG_G1, 3542)]
	[TestCase(GoG_G1, 3620)]
	[TestCase(GoG_G1, 3621)]
	public void LoadSaveLoadG1_RLERunsGreaterThan127(string g1File, int element)
	{
		var g1 = SawyerStreamReader.LoadG1(g1File, Logger);
		var d1 = g1!.ImageTable.GraphicsElements[element];
		var e1 = SawyerStreamWriter.EncodeRLEImageData(d1);
		var dd1 = new DatG1Element32(0, d1.Width, d1.Height, d1.XOffset, d1.YOffset, (DatG1ElementFlags)d1.Flags, d1.ZoomOffset)
		{
			ImageData = e1
		};
		var d2 = SawyerStreamReader.DecodeRLEImageData(dd1);
		Assert.That(d2, Is.EqualTo(d1.ImageData).AsCollection);
	}

	public void AssertG1ElementsEqual(GraphicsElement expected, GraphicsElement actual, int i)
	{
		//Assert.That(actual.Offset, Is.EqualTo(expected.Offset), $"[{i}]");
		Assert.That(actual.Width, Is.EqualTo(expected.Width), $"[{i}]");
		Assert.That(actual.Height, Is.EqualTo(expected.Height), $"[{i}]");
		Assert.That(actual.XOffset, Is.EqualTo(expected.XOffset), $"[{i}]");
		Assert.That(actual.YOffset, Is.EqualTo(expected.YOffset), $"[{i}]");
		Assert.That(actual.Flags, Is.EqualTo(expected.Flags), $"[{i}]");
		Assert.That(actual.ZoomOffset, Is.EqualTo(expected.ZoomOffset), $"[{i}]");
		Assert.That(actual.ImageData, Is.EqualTo(expected.ImageData).AsCollection, $"[{i}]");
	}
}
