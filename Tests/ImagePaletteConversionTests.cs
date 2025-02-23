using NUnit.Framework;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace OpenLoco.Dat.Tests
{
	[TestFixture]
	public class ImagePaletteConversionTests
	{
		public const string BaseObjDataPath = "Q:\\Games\\Locomotion\\OriginalObjects\\GoG\\";
		public const string BaseImagePath = "Q:\\Games\\Locomotion\\ExportedImagesFromObjectEditor\\";
		public const string BasePalettePath = "Q:\\Games\\Locomotion\\Palettes\\";
		public const string PaletteFileName = "palette.png";
		readonly ILogger Logger = new Logger();

		[Test]
		public void Write00000000ToIndex0()
		{
			var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
			var paletteMap = Image.Load<Rgba32>(paletteFile);
			paletteMap[0, 0] = PaletteMap.Transparent.Color;
			paletteMap.SaveAsPng(paletteFile);
		}

		[Test]
		public void PaletteIndex0IsTransparent()
		{
			var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
			_ = new PaletteMap(paletteFile);

			Assert.That(PaletteMap.Transparent.Color, Is.EqualTo(Color.Transparent));
		}

		[Test]
		public void PaletteHasUniqueColours()
		{
			var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
			var paletteMap = new PaletteMap(paletteFile);
			var paletteColours = paletteMap.Palette.Select(x => x.Color).ToArray();

			Assert.That(PaletteMap.Transparent.Color, Is.EqualTo(Color.Transparent));
			Assert.That(paletteColours.Length, Is.EqualTo(paletteColours.ToHashSet().Count));
		}

		[TestCase("AIRPORT1.DAT")]
		[TestCase("BALDWIN1.DAT")]
		[TestCase("FACTORY.DAT")]
		//[TestCase("INTERDEF.DAT")] // these files use different palettes
		//[TestCase("WATER1.DAT")]   // these files use different palettes
		public void G1ElementToPNGAndBack(string objectSource)
		{
			var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
			var paletteMap = new PaletteMap(paletteFile);
			var obj = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(BaseObjDataPath, objectSource), Logger);
			var g1Elements = obj!.Value!.LocoObject!.G1Elements;

			Assert.Multiple(() =>
			{
				//_ = Parallel.ForEach(g1Elements, (element, _, i) =>
				//{
				//	if (paletteMap.TryConvertG1ToRgba32Bitmap(element, out var image0))
				//	{
				//		var g1Bytes = paletteMap.ConvertRgba32ImageToG1Data(image0!, element.Flags);
				//		Assert.That(g1Bytes, Is.EqualTo(element.ImageData), $"[{i}]");
				//	}
				//});
				var i = 0;
				foreach (var element in g1Elements)
				{
					if (paletteMap.TryConvertG1ToRgba32Bitmap(element, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image0))
					{
						var g1Bytes = paletteMap.ConvertRgba32ImageToG1Data(image0!, element.Flags, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap);
						Assert.That(g1Bytes, Is.EqualTo(element.ImageData), $"[{i++}]");
					}
				}
			});
		}
	}
}
