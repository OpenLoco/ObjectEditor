using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoToolTests
{
	[TestFixture]
	public class Tests
	{
		//[Test]
		//public void DecodeEncodeIdempotent()
		//{
		//	//const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";
		//	const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
		//	var fileSize = new FileInfo(testFile).Length;
		//	var logger = new Logger();
		//	var ssr = new SawyerStreamReader(logger);
		//	var loaded = ssr.LoadFromFile(testFile);

		//	Assert.AreEqual(SawyerEncoding.runLengthSingle, loaded.ObjHdr2.Encoding);
		//	Assert.AreEqual(fileSize, Constants.DatFileHeaderSize + Constants.ObjHeaderSize + loaded.ObjHdr2.Length);

		//	var decoded = ssr.Decode(loaded.ObjHdr2.Encoding, loaded.RawData);

		//	var ssw = new SawyerStreamWriter(logger);
		//	var encoded = ssw.Encode(loaded.ObjHdr2.Encoding, decoded);

		//	CollectionAssert.AreEqual(loaded.RawData, encoded.ToArray());
		//	//var temp = Path.GetTempFileName();
		//	//ssw.WriteToFile(temp, loaded.ObjHdr1, loaded.ObjHDr2, loaded.Data);
		//}

		ILocoObject LoadObject(string filename)
		{
			var fileSize = new FileInfo(filename).Length;
			var logger = new OpenLocoToolCommon.Logger();
			var ssr = new SawyerStreamReader(logger);
			var loaded = ssr.LoadFull(filename);

			Assert.That(loaded.ObjectHeader.DataLength, Is.EqualTo(fileSize - ObjectHeader.StructLength), "ObjectHeader.Length didn't match actual size of struct");

			return loaded;
		}

		T LoadObject<T>(string filename)
			=> (T)LoadObject(filename).Object;

		[Test]
		public void LoadClimateObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\CLIM1.DAT";
			var obj = LoadObject<ClimateObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.FirstSeason, Is.EqualTo(1), nameof(obj.FirstSeason));
				Assert.That(obj.SeasonLengths[0], Is.EqualTo(57), nameof(obj.SeasonLengths) + "[0]");
				Assert.That(obj.SeasonLengths[1], Is.EqualTo(80), nameof(obj.SeasonLengths) + "[1]");
				Assert.That(obj.SeasonLengths[2], Is.EqualTo(100), nameof(obj.SeasonLengths) + "[2]");
				Assert.That(obj.SeasonLengths[3], Is.EqualTo(80), nameof(obj.SeasonLengths) + "[3]");
				Assert.That(obj.WinterSnowLine, Is.EqualTo(48), nameof(obj.WinterSnowLine));
				Assert.That(obj.SummerSnowLine, Is.EqualTo(76), nameof(obj.SummerSnowLine));
				Assert.That(obj.pad_09, Is.EqualTo(0), nameof(obj.pad_09));
			});
		}

		[Test]
		public void LoadCompetitorObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\COMP1.DAT";
			var obj = LoadObject<CompetitorObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.var_00, Is.EqualTo(0), nameof(obj.var_00));
				Assert.That(obj.var_02, Is.EqualTo(0), nameof(obj.var_02));
				Assert.That(obj.var_04, Is.EqualTo(6672), nameof(obj.var_04));
				Assert.That(obj.var_08, Is.EqualTo(2053), nameof(obj.var_08));
				Assert.That(obj.Emotions, Is.EqualTo(511), nameof(obj.Emotions));
				// Assert.That(obj.SeasonLengths[3], Is.EqualTo(80)); // images
				Assert.That(obj.Intelligence, Is.EqualTo(7), nameof(obj.Intelligence));
				Assert.That(obj.Aggressiveness, Is.EqualTo(5), nameof(obj.Aggressiveness));
				Assert.That(obj.Competitiveness, Is.EqualTo(6), nameof(obj.Competitiveness));
				Assert.That(obj.var_37, Is.EqualTo(0), nameof(obj.var_37));
			});
		}

		[Test]
		public void LoadCurrencyObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\CURRDOLL.DAT";
			var obj = LoadObject<CurrencyObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.PrefixSymbol, Is.EqualTo(0), nameof(obj.PrefixSymbol));
				Assert.That(obj.SuffixSymbol, Is.EqualTo(0), nameof(obj.SuffixSymbol));
				Assert.That(obj.ObjectIcon, Is.EqualTo(0), nameof(obj.ObjectIcon));
				Assert.That(obj.Separator, Is.EqualTo(0), nameof(obj.Separator));
				Assert.That(obj.Factor, Is.EqualTo(1), nameof(obj.Factor));
			});
		}

	}
}