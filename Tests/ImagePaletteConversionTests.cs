using NUnit.Framework;
using OpenLoco.Dat.FileParsing;
using SixLabors.ImageSharp.PixelFormats;

namespace OpenLoco.Dat.Tests
{
	[TestFixture]
	public class ImagePaletteConversionTests
	{
		const string BaseObjDataPath = "Q:\\Games\\Locomotion\\OriginalObjects\\GoG\\";
		const string BaseImagePath = "Q:\\Games\\Locomotion\\ExportedImagesFromObjectEditor\\";
		const string BasePalettePath = "Q:\\Games\\Locomotion\\Palettes\\";

		[TestCase("AIRPORT1.DAT")]
		[TestCase("BALDWIN1.DAT")]
		[TestCase("FACTORY.DAT")]
		[TestCase("INTERDEF.DAT")] // nothing i can do here - multiple indexes map to the same RGB colour (0x00FF00) in this case
		[TestCase("WATER1.DAT")]   // nothing i can do here - multiple indexes map to the same RGB colour (0x00FF00) in this case
		public void G1ElementToPNGAndBack(string objectSource)
		{
			var paletteFile = "palette.png";
			var paletteMap = new PaletteMap(SixLabors.ImageSharp.Image.Load<Rgb24>(Path.Combine(BasePalettePath, paletteFile)));

			var obj = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(BaseObjDataPath, objectSource));

			// convert g1 data into an image, and then back

			foreach (var element in obj.Value.LocoObject.G1Elements)
			{
				var image0 = paletteMap.ConvertG1IndexedToRgb24Bitmap(element);
				var g1Bytes = paletteMap.ConvertRgb24ImageToG1Data(image0);
				Assert.That(g1Bytes, Is.EquivalentTo(element.ImageData));
			}
		}
	}
}
