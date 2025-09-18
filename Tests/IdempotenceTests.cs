using Dat.Converters;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Gui.ViewModels;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SixLabors.ImageSharp;
using System.Text.Json;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

[TestFixture]
public class IdempotenceTests
{
	static string[] VanillaFiles => [
		.. Directory.GetFiles(TestConstants.BaseSteamObjDataPath, "*.dat"),
		.. Directory.GetFiles(TestConstants.BaseGoGObjDataPath, "*.dat")
		];

	[TestCaseSource(nameof(VanillaFiles))]
	public void DecodeEncodeDecode(string filename)
	{
		var logger = new Logger();
		using var fs = new FileStream(filename, FileMode.Open);

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
		var logger = new Logger();
		var obj1 = SawyerStreamReader.LoadFullObject(filename, logger)!;

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
			Assert.That(JsonSerializer.Serialize((object)actual.Object), Is.EqualTo(JsonSerializer.Serialize((object)expected.Object)));
			Assert.That(JsonSerializer.Serialize(actual.StringTable), Is.EqualTo(JsonSerializer.Serialize(expected.StringTable)));

			if (actual.ImageTable != null && expected.ImageTable != null)
			{
				var pm = new PaletteMap("C:\\Users\\bigba\\source\\repos\\OpenLoco\\ObjectEditor\\Gui\\Assets\\palette.png");
				var i = 0;
				foreach (var ae in actual.ImageTable.GraphicsElements.Zip(expected.ImageTable.GraphicsElements))
				{
					var ac = JsonSerializer.Serialize(ae.First);
					var ex = JsonSerializer.Serialize(ae.Second);

					if (ac != ex)
					{
						_ = pm.TryConvertG1ToRgba32Bitmap(ae.First, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var img1);
						_ = pm.TryConvertG1ToRgba32Bitmap(ae.Second, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var img2);
						img1.SaveAsBmp($"{Path.GetFileName(filename)}-{i}-actual.bmp");
						img2.SaveAsBmp($"{Path.GetFileName(filename)}-{i}-expected.bmp");
					}

					Assert.That(ac, Is.EqualTo(ex));
					i++;
				}
			}
		}
	}

	[TestCaseSource(nameof(VanillaFiles))]
	public void LoadSaveLoadViewModels(string filename)
	{
		var logger = new Logger();
		var obj1 = SawyerStreamReader.LoadFullObject(filename, logger)!.LocoObject!.Object;

		var vm = ObjectEditorViewModel.GetViewModelFromStruct(obj1);
		var obj2 = vm.CopyBackToModel();

		var expected = JsonSerializer.Serialize((object)obj1);
		var actual = JsonSerializer.Serialize((object)obj2);
		Assert.That(actual, Is.EqualTo(expected));
	}
}
