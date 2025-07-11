using NUnit.Framework;
using NUnit.Framework.Internal;
using Dat.FileParsing;
using Dat.Types;
using System.Text.Json;
using Logger = Common.Logging.Logger;

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
		var fullData = File.ReadAllBytes(file);

		if (!SawyerStreamReader.TryGetHeadersFromBytes(fullData, out var hdrs, logger))
		{
			Assert.Fail();
			return;
		}

		var remainingData = fullData[(S5Header.StructLength + ObjectHeader.StructLength)..];

		var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, remainingData);
		var encoded = SawyerStreamWriter.Encode(hdrs.Obj.Encoding, decoded);
		var decoded2 = SawyerStreamReader.Decode(hdrs.Obj.Encoding, encoded);
		Assert.That(decoded2, Is.EqualTo(decoded).AsCollection);
	}

	[TestCaseSource(nameof(VanillaFiles))]
	public void LoadSaveLoad(string filename)
	{
		var file = Path.Combine(TestConstants.BaseObjDataPath, filename);

		var logger = new Logger();
		var obj1 = SawyerStreamReader.LoadFullObjectFromFile(file, logger)!;
		var ms = SawyerStreamWriter.WriteLocoObjectStream(
			obj1.Value.DatFileInfo.S5Header.Name,
			obj1.Value.DatFileInfo.S5Header.SourceGame,
			obj1.Value.DatFileInfo.ObjectHeader.Encoding,
			logger,
			obj1!.Value!.LocoObject!,
			true);

		ms.Flush();

		var obj2 = SawyerStreamReader.LoadFullObjectFromStream(ms.ToArray(), logger);

		var o1 = obj1.Value.LocoObject;
		var o2 = obj2.LocoObject;

		Assert.Multiple(() =>
		{
			Assert.That(JsonSerializer.Serialize(o1.Object), Is.EqualTo(JsonSerializer.Serialize(o2.Object)));
			Assert.That(JsonSerializer.Serialize(o1.StringTable), Is.EqualTo(JsonSerializer.Serialize(o2.StringTable)));
			//Assert.That(JsonSerializer.Serialize(o1.G1Elements), Is.EqualTo(JsonSerializer.Serialize(o2.G1Elements)));
		});
	}
}
