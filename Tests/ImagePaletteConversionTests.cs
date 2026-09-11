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
	public void GraphicsElementRoundTripPreservesReservedIndices()
	{
		var paletteMap = PaletteMapLoader.LoadDefault();

		// Build a small indexed frame containing every reserved/remap index category.
		// Row 0: primary remap indices; Row 1: secondary remap indices; Row 2: text/secondary + transparent.
		byte[][] rows =
		[
			[paletteMap.PrimaryRemap[0].Index, paletteMap.PrimaryRemap[1].Index, paletteMap.ValidColours[0].Index],
			[paletteMap.SecondaryRemap[0].Index, paletteMap.SecondaryRemap[1].Index, paletteMap.ValidColours[1].Index],
			[0, paletteMap.TextRendering[0].Index, paletteMap.ChunkedTransparent.Index],
		];

		IndexedImageFrame<Rgba32> frame;
		Assert.That(paletteMap.TryConvertImageDataToIndexedImage(rows[0].Length, rows.Length, GraphicsElementFlags.None, FlattenRows(rows), out frame), Is.True, "indexed frame built from raw byte data");

		var image = GraphicsElement.FromIndexed(GraphicsElementFlags.None, frame);
		var g1 = image.ToG1Data(paletteMap);

		Assert.That(g1, Is.EqualTo(FlattenRows(rows)), "byte-for-byte round trip preserves indexed data");
		image.Dispose();
	}

	[Test]
	[TestCase(DitheringMethod.FloydSteinberg)]
	[TestCase(DitheringMethod.Atkinson)]
	[TestCase(DitheringMethod.Burkes)]
	[TestCase(DitheringMethod.JarvisJudiceNinke)]
	[TestCase(DitheringMethod.Sierra2)]
	[TestCase(DitheringMethod.Sierra3)]
	[TestCase(DitheringMethod.SierraLite)]
	[TestCase(DitheringMethod.StevensonArce)]
	[TestCase(DitheringMethod.Stucki)]
	[TestCase(DitheringMethod.Bayer2x2)]
	[TestCase(DitheringMethod.Bayer4x4)]
	[TestCase(DitheringMethod.Bayer8x8)]
	[TestCase(DitheringMethod.Bayer16x16)]
	[TestCase(DitheringMethod.Ordered3x3)]
	[TestCase(DitheringMethod.BlueNoise)]
	[TestCase(DitheringMethod.Riemersma)]
	public void GraphicsElementDitheringPreservesCompanyColours(DitheringMethod ditheringMethod)
	{
		var paletteMap = PaletteMapLoader.LoadDefault();

		// Build an RGBA image with a primary-remap stripe, a secondary-remap stripe, and a valid-colour area.
		var primary = paletteMap.PrimaryRemap[0].Color;
		var secondary = paletteMap.SecondaryRemap[0].Color;
		var valid = paletteMap.ValidColours[0].Color;

		using var img = new Image<Rgba32>(6, 2);
		for (var x = 0; x < 6; ++x)
		{
			img[x, 0] = x < 3 ? primary.ToPixel<Rgba32>() : secondary.ToPixel<Rgba32>();
			img[x, 1] = valid.ToPixel<Rgba32>();
		}

		var frame = paletteMap.ConvertRgba32ImageToIndexedImageDithering(img, GraphicsElementFlags.None, ditheringMethod);
		var image = GraphicsElement.FromIndexed(GraphicsElementFlags.None, frame);
		var g1 = image.ToG1Data(paletteMap);

		// The 12 company-colour remap entries are all reserved, so any pixel that is NOT a valid colour
		// after dithering must be a preserved company colour. Check the primary/secondary stripes:
		// primary-coloured input pixels must decode to a primary remap index, secondary to a secondary remap index.
		var primaryIndices = paletteMap.PrimaryRemap.Select(p => (int)p.Index).ToHashSet();
		var secondaryIndices = paletteMap.SecondaryRemap.Select(p => (int)p.Index).ToHashSet();

		for (var x = 0; x < 3; ++x)
		{
			Assert.That(primaryIndices.Contains(g1[x]), Is.True, $"row 0, col {x} must stay a primary remap colour ({ditheringMethod})");
		}

		for (var x = 3; x < 6; ++x)
		{
			Assert.That(secondaryIndices.Contains(g1[x]), Is.True, $"row 0, col {x} must stay a secondary remap colour ({ditheringMethod})");
		}

		// Valid-colour pixels should remain valid (non-reserved).
		Assert.That(paletteMap.ReservedColours.Any(c => c.Index == g1[6]), Is.False, "valid colour stays valid");
		image.Dispose();
	}

	/// <summary>Builds raw palette-index bytes, flattened row by row, from a nested byte array.</summary>
	static byte[] FlattenRows(byte[][] rows)
	{
		var flattened = new List<byte>();
		foreach (var row in rows)
		{
			foreach (var b in row)
			{
				flattened.Add(b);
			}
		}

		return flattened.ToArray();
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

	[Test]
	[Explicit]
	public void OutputValidColourHexCodes()
	{
		var paletteFile = Path.Combine(BasePalettePath, PaletteFileName);
		var paletteMap = new PaletteMap(paletteFile);
		var validColours = paletteMap.ValidColours;

		var jsonEntries = validColours
			.Select(c =>
			{
				var p = c.Color.ToPixel<Rgba32>();
				return $"{{\"hex\":\"#{p.R:X2}{p.G:X2}{p.B:X2}\"}}";
			});

		var json = $"[{string.Join(", ", jsonEntries)}]";
		Console.WriteLine(json);
		Assert.That(validColours.Length, Is.EqualTo(224)); // 256 - 32 reserved
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
				// Bgr24 images aren't palette-indexed, so they have no indexed frame to round-trip.
				if (element.Indexed == null)
				{
					continue;
				}

				// The indexed frame is the source of truth; verify decode-to-RGBA then re-encode is lossless.
				var sourceBytes = paletteMap.ConvertIndexedImageToG1Data(element.Indexed);
				if (paletteMap.TryConvertG1ToRgba32Bitmap(element.Width, element.Height, element.Flags, sourceBytes, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image0))
				{
					var g1Bytes = paletteMap.ConvertRgba32ImageToG1Data(image0!, element.Flags);
					Assert.That(g1Bytes, Is.EqualTo(sourceBytes), $"[{i++}]");
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

		var success = paletteMap.TryConvertG1ToRgba32Bitmap(image.Width, image.Height, GraphicsElementFlags.None, g1Bytes, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image2);
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
