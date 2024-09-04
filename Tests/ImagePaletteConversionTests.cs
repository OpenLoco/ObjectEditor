using NUnit.Framework;
using OpenLoco.Dat.FileParsing;

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
		//[TestCase("INTERDEF.DAT")] // these files use different palettes
		//[TestCase("WATER1.DAT")]   // these files use different palettes
		public void G1ElementToPNGAndBack(string objectSource)
		{
			var paletteFile = Path.Combine(BasePalettePath, "palette.png");
			var paletteMap = new PaletteMap(paletteFile);

			var obj = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(BaseObjDataPath, objectSource));

			// convert g1 data into an image, and then back

			Assert.Multiple(() =>
			{
				var i = 0;
				foreach (var element in obj.Value.LocoObject.G1Elements)
				{
					var image0 = paletteMap.ConvertG1ToRgb32Bitmap(element);
					var g1Bytes = paletteMap.ConvertRgb32ImageToG1Data(image0);
					Assert.That(g1Bytes, Is.EquivalentTo(element.ImageData), $"[{i++}]");
				}
			});
		}
	}
}
