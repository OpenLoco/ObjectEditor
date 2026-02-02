using Dat.Converters;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SixLabors.ImageSharp;
using System.Text.Json;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

[TestFixture]
public class IdempotenceTests
{
	static PaletteMap PaletteMap { get; } = new PaletteMap("C:\\Users\\bigba\\source\\repos\\OpenLoco\\ObjectEditor\\Gui\\Assets\\palette.png");

	static string[] VanillaFiles =>
	[
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
		var header = obj1.DatFileInfo.S5Header;

		using var stream = SawyerStreamWriter.WriteLocoObject(
			header.Name,
			header.ObjectType.Convert(),
			header.ObjectSource.Convert(header.Name, header.Checksum),
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
			Assert.That(actual, Is.Not.Null);
			Assert.That(expected, Is.Not.Null);

			Assert.That(JsonSerializer.Serialize((object)actual.Object), Is.EqualTo(JsonSerializer.Serialize((object)expected.Object)), "Object");
			Assert.That(JsonSerializer.Serialize(actual.StringTable), Is.EqualTo(JsonSerializer.Serialize(expected.StringTable)), "String Table");

			if (actual.ImageTable != null && expected.ImageTable != null)
			{
				var i = 0;
				foreach (var (First, Second) in actual.ImageTable.GraphicsElements.Zip(expected.ImageTable.GraphicsElements))
				{
					var ac = JsonSerializer.Serialize(First);
					var ex = JsonSerializer.Serialize(Second);

					if (ac != ex)
					{
						_ = PaletteMap.TryConvertG1ToRgba32Bitmap(First, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var img1);
						_ = PaletteMap.TryConvertG1ToRgba32Bitmap(Second, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var img2);
						img1.SaveAsBmp($"{Path.GetFileName(filename)}-{i}-actual.bmp");
						img2.SaveAsBmp($"{Path.GetFileName(filename)}-{i}-expected.bmp");
					}

					Assert.That(ac, Is.EqualTo(ex), $"GraphicsTable[{i}]");
					i++;
				}
			}
		}
	}

	//[TestCaseSource(nameof(VanillaFiles))]
	//public void LoadSaveLoadViewModels(string filename)
	//{
	//	var logger = new Logger();
	//	var obj1 = SawyerStreamReader.LoadFullObject(filename, logger)!.LocoObject;

	//	var vm = ObjectEditorViewModel.GetViewModelFromStruct(obj1!);
	//	Assert.That(vm, Is.Not.Null);

	//	var obj2 = vm.GetILocoStruct();

	//	var expected = JsonSerializer.Serialize((object)obj1!.Object);
	//	var actual = JsonSerializer.Serialize((object)obj2);
	//	Assert.That(actual, Is.EqualTo(expected));
	//}
}
