using NUnit.Framework;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;

namespace OpenLocoToolTests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void WriteLocoStruct()
		{
			//const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
			var fileSize = new FileInfo(testFile).Length;
			var logger = new Logger();
			var ssr = new SawyerStreamReader(logger);
			var ssw = new SawyerStreamWriter(logger);
			var loaded = ssr.LoadFull(testFile);

			// load data in raw bytes for test
			ReadOnlySpan<byte> fullData = ssr.LoadBytesFromFile(testFile);

			// make openlocotool useful objects
			var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var originalEncodedData = remainingData.ToArray();
			var decodedData = ssr.Decode(objectHeader.Encoding, originalEncodedData);
			remainingData = decodedData;

			var originalObjectData = decodedData[..TrainSignalObject.StructSize];

			var bytes = ByteWriter.WriteLocoStruct(loaded.Object);
			CollectionAssert.AreEqual(originalObjectData, bytes.ToArray());

			//var encodedData = ssw.Encode(objectHeader.Encoding, decodedData).ToArray();
		}

		//[Test]
		//public void LoadSaveIdempotent()
		//{
		//	//const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";
		//	const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
		//	var fileSize = new FileInfo(testFile).Length;
		//	var logger = new Logger();
		//	var ssr = new SawyerStreamReader(logger);
		//	var ssw = new SawyerStreamWriter(logger);
		//	var loaded = ssr.LoadFull(testFile);

		//	var tempFile = Path.GetTempFileName();
		//	ssw.Save(tempFile, loaded);

		//	var originalBytes = File.ReadAllBytes(testFile);
		//	var savedBytes = File.ReadAllBytes(tempFile);

		//	AssertBytesIdentical(originalBytes, savedBytes);
		//}

		//void AssertBytesIdentical(byte[] a, byte[] b)
		//{
		//	Assert.AreEqual(a.Length, b.Length);

		//	Assert.Multiple(() =>
		//	{
		//		var count = 0;
		//		foreach ((byte aa, byte bb) in a.Zip(b))
		//		{
		//			Assert.AreEqual(aa, bb, $"[{count++}] [{aa}] [{bb}]");
		//		}
		//	});
		//}

		//[Test]
		//public void DecodeEncodeIdempotent()
		//{
		//	//const string path = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEAM.dat";
		//	const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SIGC3.DAT";
		//	var fileSize = new FileInfo(testFile).Length;
		//	var logger = new Logger();
		//	var ssr = new SawyerStreamReader(logger);
		//	var ssw = new SawyerStreamWriter(logger);

		//	ReadOnlySpan<byte> fullData = ssr.LoadBytesFromFile(testFile);

		//	// make openlocotool useful objects
		//	var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
		//	var remainingData = fullData[S5Header.StructLength..];

		//	var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
		//	remainingData = remainingData[ObjectHeader.StructLength..];

		//	var originalEncodedData = remainingData.ToArray();
		//	var decodedData = ssr.Decode(objectHeader.Encoding, originalEncodedData);
		//	remainingData = decodedData;

		//	var encodedData = ssw.Encode(objectHeader.Encoding, decodedData).ToArray();
		//	CollectionAssert.AreEqual(originalEncodedData, encodedData);
		//}
	}

	[TestFixture]
	public class ObjectTests
	{
		static ILocoObject LoadObject(string filename)
		{
			var fileSize = new FileInfo(filename).Length;
			var logger = new OpenLocoToolCommon.Logger();
			var ssr = new SawyerStreamReader(logger);
			var loaded = ssr.LoadFull(filename);

			Assert.That(loaded.ObjectHeader.DataLength, Is.EqualTo(fileSize - S5Header.StructLength - ObjectHeader.StructLength), "ObjectHeader.Length didn't match actual size of struct");

			return loaded;
		}

		T LoadObject<T>(string filename)
			=> (T)LoadObject(filename).Object;

		[Test]
		public void DebuggingLoadObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\260RENFE.DAT";
			var obj = LoadObject<VehicleObject>(testFile);
			Assert.Multiple(() =>

			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
			});
		}

		[Test]
		public void LoadAirportObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadBridgeObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadBuildingObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadCargoObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadCliffEdgeObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\LSBROWN.DAT";
			var obj = LoadObject<CliffEdgeObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
			});
		}

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

		[Test]
		public void LoadDockObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadHillShapesObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadIndustryObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadInterfaceSkinObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadLandObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadLevelCrossingObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadRegionObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadRoadExtraObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadRoadObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadRoadStationObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadScaffoldingObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadScenarioTextObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadSnowObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadSoundObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadSteamObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadStreetLightObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTownNamesObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTrackExtraObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTrackObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTrainSignalObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTrainStationObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTreeObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTunnelObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadVehicleAircraftObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\707.DAT";
			var locoObj = LoadObject(testFile);

			var s5header = locoObj.S5Header;
			var objHeader = locoObj.ObjectHeader;

			Assert.Multiple(() =>
			{
				Assert.That(s5header.Flags, Is.EqualTo(283680407), nameof(s5header.Flags));
				Assert.That(s5header.Name, Is.EqualTo("707     "), nameof(s5header.Name));
				Assert.That(s5header.Checksum, Is.EqualTo(1331114877), nameof(s5header.Checksum));
				Assert.That(s5header.ObjectType, Is.EqualTo(ObjectType.Vehicle), nameof(s5header.ObjectType));

				Assert.That(objHeader.Encoding, Is.EqualTo(SawyerEncoding.RunLengthSingle), nameof(objHeader.Encoding));
				Assert.That(objHeader.DataLength, Is.EqualTo(159566), nameof(objHeader.DataLength));
			});

			var obj = locoObj.Object as VehicleObject;
			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name)); // after loading string table -> 8711
				Assert.That(obj.Mode, Is.EqualTo(TransportMode.Air), nameof(obj.Mode));
				Assert.That(obj.Type, Is.EqualTo(VehicleType.Aircraft), nameof(obj.Type));
				Assert.That(obj.var_04, Is.EqualTo(1), nameof(obj.var_04));
				// Assert.That(obj.TrackType, Is.EqualTo(0xFF), nameof(obj.TrackType)); // is changed after load from 0 to 255
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
				// CollectionAssert.AreEqual(obj.MaxCargo, Enumerable.Repeat(0, 2).ToArray(), nameof(obj.MaxCargo)); // this is changed after load from 0 to 24
				CollectionAssert.AreEqual(obj.CargoTypes, Enumerable.Repeat(0, 2).ToArray(), nameof(obj.CargoTypes));
				CollectionAssert.AreEqual(obj.CargoTypeSpriteOffsets, Enumerable.Repeat(0, 32).ToArray(), nameof(obj.CargoTypeSpriteOffsets));
				Assert.That(obj.NumSimultaneousCargoTypes, Is.EqualTo(0), nameof(obj.NumSimultaneousCargoTypes));
				Assert.That(obj.Animation[0].ObjectId, Is.EqualTo(0), nameof(obj.Animation));
				Assert.That(obj.Animation[0].Height, Is.EqualTo(24), nameof(obj.Animation));
				Assert.That(obj.Animation[0].Type, Is.EqualTo(SimpleAnimationType.None), nameof(obj.Animation));
				Assert.That(obj.Animation[1].ObjectId, Is.EqualTo(0), nameof(obj.Animation));
				Assert.That(obj.Animation[1].Height, Is.EqualTo(0), nameof(obj.Animation));
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

		[Test]
		public void LoadWallObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadWaterObject()
		{
			Assert.Fail();
		}
	}
}