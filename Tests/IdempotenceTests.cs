using NUnit.Framework;
using NUnit.Framework.Internal;
using Dat.FileParsing;
using System.Text.Json;
using Logger = Common.Logging.Logger;
using Dat.Converters;
using Definitions.ObjectModels.Types;

namespace Dat.Tests;

[TestFixture]
public class IdempotenceTests
{
	static string[] VanillaFiles => [.. Directory
		.GetFiles(TestConstants.BaseObjDataPath, "*.dat")
		.Select(x => Path.GetFileName(x))];

	[TestCaseSource(nameof(VanillaFiles))]
	public void DecodeEncodeDecode(string filename)
	{
		var file = Path.Combine(TestConstants.BaseObjDataPath, filename);

		var logger = new Logger();
		using var fs = new FileStream(file, FileMode.Open);

		if (!SawyerStreamReader.TryGetHeadersFromBytes(fs, out var hdrs, logger))
		{
			Assert.Fail();
			return;
		}

		using var br = new LocoBinaryReader(fs);

		var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, br.ReadToEnd());
		var encoded = SawyerStreamWriter.Encode(hdrs.Obj.Encoding, decoded);
		var decoded2 = SawyerStreamReader.Decode(hdrs.Obj.Encoding, encoded);
		Assert.That(decoded2, Is.EqualTo(decoded).AsCollection);
	}

	[TestCaseSource(nameof(VanillaFiles))]
	public void LoadSaveLoad(string filename)
	{
		var file = Path.Combine(TestConstants.BaseObjDataPath, filename);

		var logger = new Logger();
		var obj1 = SawyerStreamReader.LoadFullObject(file, logger)!;

		using var stream = SawyerStreamWriter.WriteLocoObject(
			obj1.DatFileInfo.S5Header.Name,
			obj1.DatFileInfo.S5Header.ObjectType.Convert(),
			obj1.DatFileInfo.S5Header.ObjectSource.Convert(),
			obj1.DatFileInfo.ObjectHeader.Encoding,
			logger,
			obj1.LocoObject!,
			true);

		stream.Flush();

		var obj2 = SawyerStreamReader.LoadFullObject(stream.ToArray(), logger);

		var o1 = obj1.LocoObject;
		var o2 = obj2.LocoObject;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(JsonSerializer.Serialize(o1.Object), Is.EqualTo(JsonSerializer.Serialize(o2.Object)));
			Assert.That(JsonSerializer.Serialize(o1.StringTable), Is.EqualTo(JsonSerializer.Serialize(o2.StringTable)));
			AssertGraphicsElementsAreEqual(o1.GraphicsElements, o2.GraphicsElements);
		}
	}

	public void AssertGraphicsElementsAreEqual(IEnumerable<GraphicsElement> expected, IEnumerable<GraphicsElement> actual)
	{
		using (Assert.EnterMultipleScope())
		{
			var index = 0;
			foreach (var (a, b) in expected.Zip(actual))
			{
				Assert.That(a.Width, Is.EqualTo(b.Width), $"[{index}] Width");
				Assert.That(a.Height, Is.EqualTo(b.Height), $"[{index}] Height");
				Assert.That(a.XOffset, Is.EqualTo(b.XOffset), $"[{index}] XOffset");
				Assert.That(a.YOffset, Is.EqualTo(b.YOffset), $"[{index}] YOffset");
				Assert.That(a.Flags, Is.EqualTo(b.Flags), $"[{index}] Flags");
				Assert.That(a.ZoomOffset, Is.EqualTo(b.ZoomOffset), $"[{index}] ZoomOffset");
				Assert.That(a.ImageData, Is.EqualTo(b.ImageData).AsCollection, $"[{index}] ImageData");
				index++;
			}
		}
	}
}
