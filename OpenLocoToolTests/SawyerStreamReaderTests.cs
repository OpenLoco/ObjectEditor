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
			var locoObj = LoadObject(testFile);

			//var header = locoObj.ObjectHeader;
			//Assert.Multiple(() =>
			//{
			//	Assert.That(locoObj.ObjectHeader.Checksum, Is.EqualTo(123));
			//});

			var obj = locoObj.Object as ClimateObject;
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

		[Test]
		public void LoadVehicleAircraftObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\707.DAT";
			var locoObj = LoadObject(testFile);

			var header = locoObj.ObjectHeader;
			Assert.Multiple(() =>
			{
				Assert.That(header.Flags, Is.EqualTo(283680407), nameof(header.Flags));
				Assert.That(header.Name, Is.EqualTo("707     "), nameof(header.Name));
				Assert.That(header.Checksum, Is.EqualTo(1331114877), nameof(header.Checksum));

				Assert.That(header.Encoding, Is.EqualTo(SawyerEncoding.runLengthSingle), nameof(header.Encoding));
				Assert.That(header.ObjectType, Is.EqualTo(ObjectType.vehicle), nameof(header.ObjectType));
			});

			var obj = locoObj.Object as VehicleObject;
			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name)); // after loading string table -> 8711
				Assert.That(obj.Mode, Is.EqualTo(TransportMode.Air), nameof(obj.Mode));
				Assert.That(obj.Type, Is.EqualTo(VehicleType.Aircraft), nameof(obj.Type));
				Assert.That(obj.var_04, Is.EqualTo(1), nameof(obj.var_04));
				Assert.That(obj.TrackType, Is.EqualTo(0xFF), nameof(obj.TrackType));
				Assert.That(obj.NumMods, Is.EqualTo(0), nameof(obj.NumMods));
				Assert.That(obj.CostIndex, Is.EqualTo(8), nameof(obj.CostIndex));
				Assert.That(obj.CostFactor, Is.EqualTo(345), nameof(obj.CostFactor));
				Assert.That(obj.Reliability, Is.EqualTo(88), nameof(obj.Reliability));
				Assert.That(obj.RunCostIndex, Is.EqualTo(4), nameof(obj.RunCostIndex));
				Assert.That(obj.RunCostFactor, Is.EqualTo(55), nameof(obj.RunCostFactor));
				Assert.That(obj.ColourType, Is.EqualTo(9), nameof(obj.ColourType));
				Assert.That(obj.NumCompat, Is.EqualTo(0), nameof(obj.NumCompat));
				CollectionAssert.AreEqual(obj.CompatibleVehicles, Enumerable.Repeat(0, 8).ToArray(), nameof(obj.CompatibleVehicles));
				CollectionAssert.AreEqual(obj.RequiredTrackExtras, Enumerable.Repeat(0, 4).ToArray(), nameof(obj.RequiredTrackExtras));
				//Assert.That(obj.var_24, Is.EqualTo(0), nameof(obj.var_24));
				//Assert.That(obj.BodySprites, Is.EqualTo(0), nameof(obj.BodySprites));
				//Assert.That(obj.BogieSprites, Is.EqualTo(1), nameof(obj.BogieSprites));
				Assert.That(obj.Power, Is.EqualTo(3000), nameof(obj.Power));
				Assert.That(obj.Speed, Is.EqualTo(604), nameof(obj.Speed));
				Assert.That(obj.RackSpeed, Is.EqualTo(120), nameof(obj.RackSpeed));
				Assert.That(obj.Weight, Is.EqualTo(141), nameof(obj.Weight));
				Assert.That(obj.Flags, Is.EqualTo((VehicleObjectFlags)16384), nameof(obj.Flags));
				CollectionAssert.AreEqual(obj.MaxCargo, Enumerable.Repeat(0, 2).ToArray(), nameof(obj.MaxCargo));
				CollectionAssert.AreEqual(obj.CargoTypes, Enumerable.Repeat(0, 2).ToArray(), nameof(obj.CargoTypes));
				CollectionAssert.AreEqual(obj.CargoTypeSpriteOffsets, Enumerable.Repeat(0, 32).ToArray(), nameof(obj.CargoTypeSpriteOffsets));
				Assert.That(obj.NumSimultaneousCargoTypes, Is.EqualTo(0), nameof(obj.NumSimultaneousCargoTypes));

				//Assert.That(obj.Animation, Is.EqualTo(0), nameof(obj.Animation));
				Assert.That(obj.Animation[0].ObjectId, Is.EqualTo(0), nameof(obj.MaxCargo), nameof(obj.Animation));
				Assert.That(obj.Animation[0].Height, Is.EqualTo(24), nameof(obj.MaxCargo), nameof(obj.Animation));
				Assert.That(obj.Animation[0].Type, Is.EqualTo(SimpleAnimationType.None), nameof(obj.Animation));
				Assert.That(obj.Animation[1].ObjectId, Is.EqualTo(0), nameof(obj.MaxCargo), nameof(obj.Animation));
				Assert.That(obj.Animation[1].Height, Is.EqualTo(0), nameof(obj.MaxCargo), nameof(obj.Animation));
				Assert.That(obj.Animation[1].Type, Is.EqualTo(SimpleAnimationType.None), nameof(obj.Animation));
				Assert.That(obj.var_113, Is.EqualTo(0), nameof(obj.var_113));
				Assert.That(obj.Designed, Is.EqualTo(1957), nameof(obj.Designed));
				Assert.That(obj.Obsolete, Is.EqualTo(1987), nameof(obj.Obsolete));
				Assert.That(obj.RackRailType, Is.EqualTo(0), nameof(obj.RackRailType));
				Assert.That(obj.DrivingSoundType, Is.EqualTo(DrivingSoundType.Engine1), nameof(obj.DrivingSoundType));
				//Assert.That(obj.Sound, Is.EqualTo(0), nameof(obj.Sound));
				//Assert.That(obj.pad_135, Is.EqualTo(0), nameof(obj.pad_135));
				Assert.That(obj.NumStartSounds, Is.EqualTo(2), nameof(obj.NumStartSounds));
				CollectionAssert.AreEqual(obj.StartSounds, Enumerable.Repeat(0, 3).ToArray(), nameof(obj.StartSounds));
			});
		}
	}
}