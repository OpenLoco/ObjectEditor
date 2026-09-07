using Dat.FileParsing;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Graphics.Dithering;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

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
		paletteMap[0, 0] = PaletteMap.Transparent.Color.ToPixel<Rgba32>();
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
		var obj = SawyerStreamReader.LoadFullObject(Path.Combine(BaseObjDataPath, objectSource), Logger);
		var g1Elements = obj!.LocoObject!.ImageTable!.GraphicsElements;

		var i = 0;
		using (Assert.EnterMultipleScope())
		{
			foreach (var element in g1Elements)
			{
				if (paletteMap.TryConvertG1ToRgba32Bitmap(element, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image0))
				{
					var g1Bytes = paletteMap.ConvertRgba32ImageToG1Data(image0!, element.Flags);
					Assert.That(g1Bytes, Is.EqualTo(element.ImageData), $"[{i++}]");
				}
			}
		}
	}

	[TestCase(DitheringMethod.FloydSteinberg)]
	[TestCase(DitheringMethod.Bayer2x2)]
	[TestCase(DitheringMethod.Bayer4x4)]
	[TestCase(DitheringMethod.Bayer8x8)]
	[TestCase(DitheringMethod.Bayer16x16)]
	[TestCase(DitheringMethod.Ordered3x3)]
	[TestCase(DitheringMethod.BlueNoise)]
	[TestCase(DitheringMethod.Riemersma)]
	[TestCase(DitheringMethod.Atkinson)]
	[TestCase(DitheringMethod.Burkes)]
	[TestCase(DitheringMethod.JarvisJudiceNinke)]
	[TestCase(DitheringMethod.Sierra2)]
	[TestCase(DitheringMethod.Sierra3)]
	[TestCase(DitheringMethod.SierraLite)]
	[TestCase(DitheringMethod.StevensonArce)]
	[TestCase(DitheringMethod.Stucki)]
	[Explicit("Requires a local input image and writes output files; not suitable for automated CI runs.")]
	public void ImportImageWithDithering(DitheringMethod ditheringMethod)
	{
		var image = Image.Load<Rgba32>("C:\\Users\\bigba\\OneDrive\\Pictures\\gradient.png");
		var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
		var paletteMap = new PaletteMap(paletteFile);
		var g1Bytes = paletteMap.ConvertRgba32ImageToG1DataWithDithering(image, GraphicsElementFlags.None, ditheringMethod);

		var ele = new GraphicsElement()
		{
			Flags = GraphicsElementFlags.None,
			ImageData = g1Bytes,
			Width = (short)image.Width,
			Height = (short)image.Height,
		};

		var success = paletteMap.TryConvertG1ToRgba32Bitmap(ele, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image2);
		if (success)
		{
			image2!.SaveAsPng($"C:\\Users\\bigba\\OneDrive\\Pictures\\gradient_{ditheringMethod}.png");
		}
		else
		{
			Assert.Fail($"Failed to convert G1 to RGBA32 bitmap using {ditheringMethod}.");
		}

	}
}
