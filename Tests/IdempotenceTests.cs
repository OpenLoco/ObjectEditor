using Dat.Converters;
using Dat.FileParsing;
using Gui.ViewModels;
using NUnit.Framework;
using NUnit.Framework.Internal;
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

		var expected = obj1.LocoObject;
		var actual = obj2.LocoObject;

		using (Assert.EnterMultipleScope())
		{
			var a = JsonSerializer.Serialize((object)expected.Object);
			var b = JsonSerializer.Serialize((object)actual.Object);
			Assert.That(b, Is.EqualTo(a));

			//Assert.That(JsonSerializer.Serialize((object)o2.Object), Is.EqualTo(JsonSerializer.Serialize((object)o1.Object)));
			Assert.That(JsonSerializer.Serialize(actual.StringTable), Is.EqualTo(JsonSerializer.Serialize(expected.StringTable)));
			Assert.That(JsonSerializer.Serialize(actual.GraphicsElements), Is.EqualTo(JsonSerializer.Serialize(expected.GraphicsElements)));
		}
	}

	[TestCaseSource(nameof(VanillaFiles))]
	public void LoadSaveLoadViewModels(string filename)
	{
		var file = Path.Combine(TestConstants.BaseObjDataPath, filename);

		var logger = new Logger();
		var obj1 = SawyerStreamReader.LoadFullObject(file, logger)!.LocoObject!.Object;

		var vm = ObjectEditorViewModel.GetViewModelFromStruct(obj1);
		var obj2 = vm.GetAsModel();

		var expected = JsonSerializer.Serialize((object)obj1);
		var actual = JsonSerializer.Serialize((object)obj2);
		Assert.That(actual, Is.EqualTo(expected));
	}
}
