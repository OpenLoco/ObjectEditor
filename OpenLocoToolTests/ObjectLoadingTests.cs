using NUnit.Framework;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoToolTests
{
	[TestFixture]
	public class ObjectLoadingTests
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

		//[Test]
		//public void DebuggingLoadObject()
		//{
		//	const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\260RENFE.DAT";
		//	var obj = LoadObject<VehicleObject>(testFile);
		//	Assert.Multiple(() =>

		//	{
		//		Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
		//	});
		//}

		[Test]
		public void LoadAirportObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\AIRPORT1.DAT";
			var obj = LoadObject<AirportObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.BuildCostFactor, Is.EqualTo(256), nameof(obj.BuildCostFactor));
				Assert.That(obj.SellCostFactor, Is.EqualTo(-192), nameof(obj.SellCostFactor));
				Assert.That(obj.CostIndex, Is.EqualTo(1), nameof(obj.CostIndex));
				Assert.That(obj.var_07, Is.EqualTo(0), nameof(obj.var_07));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.var_0C, Is.EqualTo(0), nameof(obj.var_0C));
				Assert.That(obj.AllowedPlaneTypes, Is.EqualTo(24), nameof(obj.AllowedPlaneTypes));
				Assert.That(obj.NumSpriteSets, Is.EqualTo(94), nameof(obj.NumSpriteSets));
				Assert.That(obj.NumTiles, Is.EqualTo(23), nameof(obj.NumTiles));

				//Assert.That(obj.var_14, Is.EqualTo(0), nameof(obj.var_14));
				//Assert.That(obj.var_18, Is.EqualTo(0), nameof(obj.var_18));
				//Assert.That(obj.var_1C, Is.EqualTo(0), nameof(obj.var_1C));
				//Assert.That(obj.var_9C, Is.EqualTo(0), nameof(obj.var_9C));

				Assert.That(obj.LargeTiles, Is.EqualTo(917759), nameof(obj.LargeTiles));
				Assert.That(obj.MinX, Is.EqualTo(-4), nameof(obj.MinX));
				Assert.That(obj.MinY, Is.EqualTo(-4), nameof(obj.MinY));
				Assert.That(obj.MaxX, Is.EqualTo(5), nameof(obj.MaxX));
				Assert.That(obj.MaxY, Is.EqualTo(5), nameof(obj.MaxY));
				Assert.That(obj.DesignedYear, Is.EqualTo(1970), nameof(obj.DesignedYear));
				Assert.That(obj.ObsoleteYear, Is.EqualTo(65535), nameof(obj.ObsoleteYear));
				Assert.That(obj.NumMovementNodes, Is.EqualTo(26), nameof(obj.NumMovementNodes));
				Assert.That(obj.NumMovementEdges, Is.EqualTo(30), nameof(obj.NumMovementEdges));

				//Assert.That(obj.MovementNodes, Is.EqualTo(0), nameof(obj.MovementNodes));
				//Assert.That(obj.MovementEdges, Is.EqualTo(0), nameof(obj.MovementEdges));

				Assert.That(obj.pad_B6[0], Is.EqualTo(0), nameof(obj.pad_B6) + "[0]");
				Assert.That(obj.pad_B6[1], Is.EqualTo(19), nameof(obj.pad_B6) + "[1]");
				Assert.That(obj.pad_B6[2], Is.EqualTo(0), nameof(obj.pad_B6) + "[2]");
				Assert.That(obj.pad_B6[3], Is.EqualTo(0), nameof(obj.pad_B6) + "[3]");
			});
		}

		[Test]
		public void LoadBridgeObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\BRDGBRCK.DAT";
			var obj = LoadObject<BridgeObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.NoRoof, Is.EqualTo(0), nameof(obj.NoRoof));

				Assert.That(obj.pad_03[0], Is.EqualTo(0), nameof(obj.pad_03) + "[0]");
				Assert.That(obj.pad_03[1], Is.EqualTo(0), nameof(obj.pad_03) + "[1]");
				Assert.That(obj.pad_03[2], Is.EqualTo(0), nameof(obj.pad_03) + "[2]");

				Assert.That(obj.var_06, Is.EqualTo(16), nameof(obj.var_06));
				Assert.That(obj.SpanLength, Is.EqualTo(1), nameof(obj.SpanLength));
				Assert.That(obj.PillarSpacing, Is.EqualTo(255), nameof(obj.PillarSpacing));
				Assert.That(obj.MaxSpeed, Is.EqualTo(60), nameof(obj.MaxSpeed));
				Assert.That(obj.MaxHeight, Is.EqualTo(10), nameof(obj.MaxHeight));
				Assert.That(obj.CostIndex, Is.EqualTo(1), nameof(obj.CostIndex));
				Assert.That(obj.BaseCostFactor, Is.EqualTo(16), nameof(obj.BaseCostFactor));
				Assert.That(obj.HeightCostFactor, Is.EqualTo(8), nameof(obj.HeightCostFactor));
				Assert.That(obj.SellCostFactor, Is.EqualTo(-12), nameof(obj.SellCostFactor));
				Assert.That(obj.DisabledTrackCfg, Is.EqualTo(0), nameof(obj.DisabledTrackCfg));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.TrackNumCompatible, Is.EqualTo(0), nameof(obj.TrackNumCompatible));
				CollectionAssert.AreEqual(obj.TrackMods, Array.CreateInstance(typeof(byte), 7), nameof(obj.TrackMods));
				Assert.That(obj.RoadNumCompatible, Is.EqualTo(0), nameof(obj.RoadNumCompatible));
				CollectionAssert.AreEqual(obj.RoadMods, Array.CreateInstance(typeof(byte), 7), nameof(obj.RoadMods));
				Assert.That(obj.DesignedYear, Is.EqualTo(0), nameof(obj.DesignedYear));
			});
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
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\RDEXCAT1.DAT";
			var obj = LoadObject<RoadExtraObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.RoadPieces, Is.EqualTo(127), nameof(obj.RoadPieces));
				Assert.That(obj.PaintStyle, Is.EqualTo(1), nameof(obj.PaintStyle));
				Assert.That(obj.CostIndex, Is.EqualTo(1), nameof(obj.CostIndex));
				Assert.That(obj.BuildCostFactor, Is.EqualTo(4), nameof(obj.BuildCostFactor));
				Assert.That(obj.SellCostFactor, Is.EqualTo(-3), nameof(obj.SellCostFactor));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.var_0E, Is.EqualTo(0), nameof(obj.var_0E));
			});
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
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SCAFDEF.DAT";
			var obj = LoadObject<ScaffoldingObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));

				Assert.That(obj.SegmentHeights[0], Is.EqualTo(16), nameof(obj.SegmentHeights) + "[0]");
				Assert.That(obj.SegmentHeights[1], Is.EqualTo(16), nameof(obj.SegmentHeights) + "[1]");
				Assert.That(obj.SegmentHeights[2], Is.EqualTo(32), nameof(obj.SegmentHeights) + "[2]");

				Assert.That(obj.RoofHeights[0], Is.EqualTo(0), nameof(obj.RoofHeights) + "[0]");
				Assert.That(obj.RoofHeights[1], Is.EqualTo(0), nameof(obj.RoofHeights) + "[1]");
				Assert.That(obj.RoofHeights[2], Is.EqualTo(14), nameof(obj.RoofHeights) + "[2]");
			});
		}

		[Test]
		public void LoadScenarioTextObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\STEX000.DAT";
			var obj = LoadObject<ScenarioTextObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Details, Is.EqualTo(0), nameof(obj.Details));
				Assert.That(obj.pad_04, Is.EqualTo(0), nameof(obj.pad_04));
			});
		}

		[Test]
		public void LoadSnowObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SNOW.DAT";
			var obj = LoadObject<SnowObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
			});
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
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\SLIGHT1.DAT";
			var obj = LoadObject<StreetLightObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));

				Assert.That(obj.DesignedYear[0], Is.EqualTo(1900), nameof(obj.DesignedYear) + "[0]");
				Assert.That(obj.DesignedYear[1], Is.EqualTo(1950), nameof(obj.DesignedYear) + "[1]");
				Assert.That(obj.DesignedYear[2], Is.EqualTo(1985), nameof(obj.DesignedYear) + "[2]");

				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
			});
		}

		[Test]
		public void LoadTownNamesObject()
		{
			Assert.Fail();
		}

		[Test]
		public void LoadTrackExtraObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\TREXCAT1.DAT";
			var obj = LoadObject<TrackExtraObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.TrackPieces, Is.EqualTo(1023), nameof(obj.TrackPieces));
				Assert.That(obj.PaintStyle, Is.EqualTo(1), nameof(obj.PaintStyle));
				Assert.That(obj.CostIndex, Is.EqualTo(1), nameof(obj.CostIndex));
				Assert.That(obj.BuildCostFactor, Is.EqualTo(2), nameof(obj.BuildCostFactor));
				Assert.That(obj.SellCostFactor, Is.EqualTo(-1), nameof(obj.SellCostFactor));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.var_0E, Is.EqualTo(0), nameof(obj.var_0E));
			});
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
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\BEECH.DAT";
			var obj = LoadObject<TreeObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.var_02, Is.EqualTo(40), nameof(obj.var_02));
				Assert.That(obj.Height, Is.EqualTo(131), nameof(obj.Height));
				Assert.That(obj.var_04, Is.EqualTo(27), nameof(obj.var_04));
				Assert.That(obj.var_05, Is.EqualTo(83), nameof(obj.var_05));
				Assert.That(obj.NumRotations, Is.EqualTo(1), nameof(obj.NumRotations));
				Assert.That(obj.Growth, Is.EqualTo(4), nameof(obj.Growth));
				Assert.That(obj.Flags, Is.EqualTo(TreeObjectFlags.HighAltitude | TreeObjectFlags.RequiresWater | TreeObjectFlags.HasShadow), nameof(obj.Flags));
				CollectionAssert.AreEqual(obj.Sprites, Array.CreateInstance(typeof(byte), 6), nameof(obj.Sprites));
				CollectionAssert.AreEqual(obj.SnowSprites, Array.CreateInstance(typeof(byte), 6), nameof(obj.SnowSprites));
				Assert.That(obj.ShadowImageOffset, Is.EqualTo(0), nameof(obj.ShadowImageOffset));
				Assert.That(obj.var_3C, Is.EqualTo(15), nameof(obj.var_3C));
				Assert.That(obj.SeasonState, Is.EqualTo(3), nameof(obj.SeasonState));
				Assert.That(obj.var_3E, Is.EqualTo(2), nameof(obj.var_3E));
				Assert.That(obj.CostIndex, Is.EqualTo(3), nameof(obj.CostIndex));
				Assert.That(obj.BuildCostFactor, Is.EqualTo(8), nameof(obj.BuildCostFactor));
				Assert.That(obj.ClearCostFactor, Is.EqualTo(4), nameof(obj.ClearCostFactor));
				Assert.That(obj.Colours, Is.EqualTo(0), nameof(obj.Colours));
				Assert.That(obj.Rating, Is.EqualTo(10), nameof(obj.Rating));
				Assert.That(obj.DemolishRatingReduction, Is.EqualTo(-15), nameof(obj.DemolishRatingReduction));
			});
		}

		[Test]
		public void LoadTunnelObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\TUNNEL1.DAT";
			var obj = LoadObject<TunnelObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
			});
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
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\FENCE1.DAT";
			var obj = LoadObject<WallObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.var_06, Is.EqualTo(15), nameof(obj.var_06));
				Assert.That(obj.Flags, Is.EqualTo(WallObjectFlags.None), nameof(obj.Flags));
				Assert.That(obj.Height, Is.EqualTo(2), nameof(obj.Height));
				Assert.That(obj.var_09, Is.EqualTo(8), nameof(obj.var_09));
			});
		}

		[Test]
		public void LoadWaterObject()
		{
			const string testFile = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\WATER1.DAT";
			var obj = LoadObject<WaterObject>(testFile);

			Assert.Multiple(() =>
			{
				Assert.That(obj.Name, Is.EqualTo(0), nameof(obj.Name));
				Assert.That(obj.CostIndex, Is.EqualTo(2), nameof(obj.CostIndex));
				Assert.That(obj.var_03, Is.EqualTo(0), nameof(obj.var_03));
				Assert.That(obj.CostFactor, Is.EqualTo(51), nameof(obj.CostFactor));
				Assert.That(obj.var_05, Is.EqualTo(0), nameof(obj.var_05));
				Assert.That(obj.Image, Is.EqualTo(0), nameof(obj.Image));
				Assert.That(obj.var_0A, Is.EqualTo(0), nameof(obj.var_0A));
			});
		}
	}
}