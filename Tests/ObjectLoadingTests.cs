using Core.Objects;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Objects;
using OpenLocoObjectEditor.Types;
using Logger = OpenLocoObjectEditor.Logging.Logger;

namespace OpenLocoObjectEditor.Tests
{
	[TestFixture]
	public class ObjectLoadingTests
	{
		const string BaseObjDataPath = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData\\";

		static (ILocoObject, T) LoadObjectCore<T>(string filename) where T : ILocoStruct
		{
			var fileSize = new FileInfo(filename).Length;
			var logger = new Logger();
			var loaded = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: logger);

			Assert.That(loaded.DatFileInfo.ObjectHeader.DataLength, Is.EqualTo(fileSize - S5Header.StructLength - ObjectHeader.StructLength), "ObjectHeader.Length didn't match actual size of struct");

			return (loaded.LocoObject, (T)loaded.LocoObject.Object);
		}

		static (ILocoObject, T) LoadObject<T>(string filename) where T : ILocoStruct
			=> LoadObjectCore<T>(Path.Combine(BaseObjDataPath, filename));

		[TestCase("AIRPORT1.DAT")]
		public void LoadAirportObject(string objectName)
		{
			var (obj, struc) = LoadObject<AirportObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.Name, Is.EqualTo(0), nameof(struc.Name));
				Assert.That(struc.BuildCostFactor, Is.EqualTo(256), nameof(struc.BuildCostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-192), nameof(struc.SellCostFactor));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
				Assert.That(struc.Image, Is.EqualTo(0), nameof(struc.Image));
				Assert.That(struc.ImageOffset, Is.EqualTo(0), nameof(struc.ImageOffset));
				Assert.That(struc.AllowedPlaneTypes, Is.EqualTo(24), nameof(struc.AllowedPlaneTypes));
				Assert.That(struc.NumBuildingAnimations, Is.EqualTo(94), nameof(struc.NumBuildingAnimations));
				Assert.That(struc.NumBuildingVariations, Is.EqualTo(23), nameof(struc.NumBuildingVariations));

				//Assert.That(struc.var_14, Is.EqualTo(0), nameof(struc.var_14));
				//Assert.That(struc.var_18, Is.EqualTo(0), nameof(struc.var_18));
				//Assert.That(struc.var_1C, Is.EqualTo(0), nameof(struc.var_1C));
				//Assert.That(struc.var_9C, Is.EqualTo(0), nameof(struc.var_9C));

				Assert.That(struc.LargeTiles, Is.EqualTo(917759), nameof(struc.LargeTiles));
				Assert.That(struc.MinX, Is.EqualTo(-4), nameof(struc.MinX));
				Assert.That(struc.MinY, Is.EqualTo(-4), nameof(struc.MinY));
				Assert.That(struc.MaxX, Is.EqualTo(5), nameof(struc.MaxX));
				Assert.That(struc.MaxY, Is.EqualTo(5), nameof(struc.MaxY));
				Assert.That(struc.DesignedYear, Is.EqualTo(1970), nameof(struc.DesignedYear));
				Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
				Assert.That(struc.NumMovementNodes, Is.EqualTo(26), nameof(struc.NumMovementNodes));
				Assert.That(struc.NumMovementEdges, Is.EqualTo(30), nameof(struc.NumMovementEdges));

				//Assert.That(struc.MovementNodes, Is.EqualTo(0), nameof(struc.MovementNodes));
				//Assert.That(struc.MovementEdges, Is.EqualTo(0), nameof(struc.MovementEdges));

				Assert.That(struc.pad_B6[0], Is.EqualTo(0), nameof(struc.pad_B6) + "[0]");
				Assert.That(struc.pad_B6[1], Is.EqualTo(19), nameof(struc.pad_B6) + "[1]");
				Assert.That(struc.pad_B6[2], Is.EqualTo(0), nameof(struc.pad_B6) + "[2]");
				Assert.That(struc.pad_B6[3], Is.EqualTo(0), nameof(struc.pad_B6) + "[3]");
			});
		}

		[TestCase("BRDGBRCK.DAT")]
		public void LoadBridgeObject(string objectName)
		{
			var (obj, struc) = LoadObject<BridgeObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.NoRoof, Is.EqualTo(0), nameof(struc.NoRoof));

				// Assert.That(struc.pad_03[0], Is.EqualTo(0), nameof(struc.pad_03) + "[0]");
				// Assert.That(struc.pad_03[1], Is.EqualTo(0), nameof(struc.pad_03) + "[1]");
				// Assert.That(struc.pad_03[2], Is.EqualTo(0), nameof(struc.pad_03) + "[2]");

				Assert.That(struc.var_06, Is.EqualTo(16), nameof(struc.var_06));
				Assert.That(struc.SpanLength, Is.EqualTo(1), nameof(struc.SpanLength));
				Assert.That(struc.PillarSpacing, Is.EqualTo(255), nameof(struc.PillarSpacing));
				Assert.That(struc.MaxSpeed, Is.EqualTo(60), nameof(struc.MaxSpeed));
				Assert.That(struc.MaxHeight, Is.EqualTo(10), nameof(struc.MaxHeight));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
				Assert.That(struc.BaseCostFactor, Is.EqualTo(16), nameof(struc.BaseCostFactor));
				Assert.That(struc.HeightCostFactor, Is.EqualTo(8), nameof(struc.HeightCostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-12), nameof(struc.SellCostFactor));
				Assert.That(struc.DisabledTrackCfg, Is.EqualTo(0), nameof(struc.DisabledTrackCfg));
				//Assert.That(struc.TrackNumCompatible, Is.EqualTo(0), nameof(struc.TrackNumCompatible));
				//CollectionAssert.AreEqual(struc.TrackMods, Array.CreateInstance(typeof(byte), 7), nameof(struc.TrackMods));
				//Assert.That(struc.RoadNumCompatible, Is.EqualTo(0), nameof(struc.RoadNumCompatible));
				//CollectionAssert.AreEqual(struc.RoadMods, Array.CreateInstance(typeof(byte), 7), nameof(struc.RoadMods));
				Assert.That(struc.DesignedYear, Is.EqualTo(0), nameof(struc.DesignedYear));
			});
		}

		[TestCase("HQ1.DAT")]
		public void LoadBuildingObject(string objectName)
		{
			var (obj, struc) = LoadObject<BuildingObject>(objectName);

			Assert.Multiple(() =>
			{
				//Assert.That(struc.var_06, Is.EqualTo(16), nameof(struc.var_06));
				Assert.That(struc.NumVariations, Is.EqualTo(5), nameof(struc.NumVariations));
				CollectionAssert.AreEqual(struc.VariationHeights, Array.CreateInstance(typeof(byte), 4), nameof(struc.VariationHeights));
				//CollectionAssert.AreEqual(struc.var_0C, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_0C));
				Assert.That(struc.Colours, Is.EqualTo(0), nameof(struc.Colours));
				Assert.That(struc.DesignedYear, Is.EqualTo(0), nameof(struc.DesignedYear));
				Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
				Assert.That(struc.Flags, Is.EqualTo(BuildingObjectFlags.LargeTile | BuildingObjectFlags.MiscBuilding | BuildingObjectFlags.IsHeadquarters), nameof(struc.Flags));
				Assert.That(struc.ClearCostIndex, Is.EqualTo(1), nameof(struc.ClearCostIndex));
				Assert.That(struc.ClearCostFactor, Is.EqualTo(0), nameof(struc.ClearCostFactor));
				Assert.That(struc.ScaffoldingSegmentType, Is.EqualTo(1), nameof(struc.ScaffoldingSegmentType));
				Assert.That(struc.ScaffoldingColour, Is.EqualTo(Colour.yellow), nameof(struc.ScaffoldingColour));

				//Assert.That(struc.pad_9E[0], Is.EqualTo(3), nameof(struc.pad_9E) + "[0]");
				//Assert.That(struc.pad_9E[1], Is.EqualTo(3), nameof(struc.pad_9E) + "[1]");

				//CollectionAssert.AreEqual(struc.ProducedQuantity, Array.CreateInstance(typeof(byte), 2), nameof(struc.ProducedQuantity));
				//CollectionAssert.AreEqual(struc.ProducedCargoType, Array.CreateInstance(typeof(byte), 2), nameof(struc.ProducedCargoType));
				CollectionAssert.AreEqual(struc.var_A6, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A6));
				CollectionAssert.AreEqual(struc.var_A8, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A8));
				CollectionAssert.AreEqual(struc.var_A4, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A4));
				Assert.That(struc.DemolishRatingReduction, Is.EqualTo(0), nameof(struc.DemolishRatingReduction));
				Assert.That(struc.var_AC, Is.EqualTo(255), nameof(struc.var_AC));
				Assert.That(struc.var_AD, Is.EqualTo(0), nameof(struc.var_AD));
			});
		}

		[TestCase("CHEMICAL.DAT")]
		public void LoadCargoObject(string objectName)
		{
			var (obj, struc) = LoadObject<CargoObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.var_02, Is.EqualTo(256), nameof(struc.var_02));
				Assert.That(struc.CargoTransferTime, Is.EqualTo(64), nameof(struc.CargoTransferTime));
				Assert.That(struc.UnitInlineSprite, Is.EqualTo(0), nameof(struc.UnitInlineSprite));
				Assert.That(struc.CargoCategory, Is.EqualTo(CargoCategory.liquids), nameof(struc.CargoCategory));
				Assert.That(struc.Flags, Is.EqualTo(CargoObjectFlags.Delivering), nameof(struc.Flags));
				Assert.That(struc.NumPlatformVariations, Is.EqualTo(1), nameof(struc.NumPlatformVariations));
				Assert.That(struc.var_14, Is.EqualTo(4), nameof(struc.var_14));
				Assert.That(struc.PremiumDays, Is.EqualTo(10), nameof(struc.PremiumDays));
				Assert.That(struc.MaxNonPremiumDays, Is.EqualTo(30), nameof(struc.MaxNonPremiumDays));
				Assert.That(struc.MaxPremiumRate, Is.EqualTo(128), nameof(struc.MaxPremiumRate));
				Assert.That(struc.PenaltyRate, Is.EqualTo(256), nameof(struc.PenaltyRate));
				Assert.That(struc.PaymentFactor, Is.EqualTo(62), nameof(struc.PaymentFactor));
				Assert.That(struc.PaymentIndex, Is.EqualTo(10), nameof(struc.PaymentIndex));
				Assert.That(struc.UnitSize, Is.EqualTo(10), nameof(struc.UnitSize));
			});
		}

		[TestCase("LSBROWN.DAT")]
		public void LoadCliffEdgeObject(string objectName)
		{
			var (obj, struc) = LoadObject<CliffEdgeObject>(objectName);

			var strTable = obj.StringTable;

			Assert.That(strTable.Table, Has.Count.EqualTo(1));
			Assert.That(strTable.Table.ContainsKey("Name"), Is.True);

			var entry = strTable.Table["Name"];
			Assert.That(entry[LanguageId.english_uk], Is.EqualTo("Brown Rock"));
			Assert.That(entry[LanguageId.english_us], Is.EqualTo("Brown Rock"));
		}

		[TestCase("CLIM1.DAT")]
		public void LoadClimateObject(string objectName)
		{
			var (obj, struc) = LoadObject<ClimateObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.FirstSeason, Is.EqualTo(1), nameof(struc.FirstSeason));
				Assert.That(struc.SeasonLengths[0], Is.EqualTo(57), nameof(struc.SeasonLengths) + "[0]");
				Assert.That(struc.SeasonLengths[1], Is.EqualTo(80), nameof(struc.SeasonLengths) + "[1]");
				Assert.That(struc.SeasonLengths[2], Is.EqualTo(100), nameof(struc.SeasonLengths) + "[2]");
				Assert.That(struc.SeasonLengths[3], Is.EqualTo(80), nameof(struc.SeasonLengths) + "[3]");
				Assert.That(struc.WinterSnowLine, Is.EqualTo(48), nameof(struc.WinterSnowLine));
				Assert.That(struc.SummerSnowLine, Is.EqualTo(76), nameof(struc.SummerSnowLine));
				Assert.That(struc.pad_09, Is.EqualTo(0), nameof(struc.pad_09));
			});
		}

		[TestCase("COMP1.DAT")]
		public void LoadCompetitorObject(string objectName)
		{
			var (obj, struc) = LoadObject<CompetitorObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.var_04, Is.EqualTo(6672), nameof(struc.var_04));
				Assert.That(struc.var_08, Is.EqualTo(2053), nameof(struc.var_08));
				Assert.That(struc.Emotions, Is.EqualTo(255), nameof(struc.Emotions));
				CollectionAssert.AreEqual(struc.Images, Array.CreateInstance(typeof(byte), 9), nameof(struc.Images));
				Assert.That(struc.Intelligence, Is.EqualTo(7), nameof(struc.Intelligence));
				Assert.That(struc.Aggressiveness, Is.EqualTo(5), nameof(struc.Aggressiveness));
				Assert.That(struc.Competitiveness, Is.EqualTo(6), nameof(struc.Competitiveness));
				Assert.That(struc.var_37, Is.EqualTo(0), nameof(struc.var_37));
			});
		}

		[TestCase("CURRDOLL.DAT")]
		public void LoadCurrencyObject(string objectName)
		{
			var (obj, struc) = LoadObject<CurrencyObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.ObjectIcon, Is.EqualTo(0), nameof(struc.ObjectIcon));
				Assert.That(struc.Separator, Is.EqualTo(0), nameof(struc.Separator));
				Assert.That(struc.Factor, Is.EqualTo(1), nameof(struc.Factor));
			});
		}

		[TestCase("SHIPST1.DAT")]
		public void LoadDockObject(string objectName)
		{
			var (obj, struc) = LoadObject<DockObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.BuildCostFactor, Is.EqualTo(38), nameof(struc.BuildCostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-35), nameof(struc.SellCostFactor));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
				Assert.That(struc.var_07, Is.EqualTo(0), nameof(struc.var_07));
				Assert.That(struc.UnkImage, Is.EqualTo(0), nameof(struc.UnkImage));
				Assert.That(struc.Flags, Is.EqualTo(DockObjectFlags.None), nameof(struc.Flags));
				Assert.That(struc.NumAux01, Is.EqualTo(2), nameof(struc.NumAux01));
				Assert.That(struc.NumAux02Ent, Is.EqualTo(1), nameof(struc.NumAux02Ent));

				//Assert.That(struc.var_14, Is.EqualTo(1), nameof(struc.var_14));
				//Assert.That(struc.var_14, Is.EqualTo(1), nameof(struc.var_18));
				//Assert.That(struc.var_1C[0], Is.EqualTo(1), nameof(struc.var_1C[0]));

				Assert.That(struc.DesignedYear, Is.EqualTo(0), nameof(struc.DesignedYear));
				Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
				Assert.That(struc.BoatPosition, Is.EqualTo(new Pos2(48, 0)), nameof(struc.BoatPosition));
			});
		}

		[TestCase("HS1.DAT")]
		public void LoadHillShapesObject(string objectName)
		{
			var (obj, struc) = LoadObject<HillShapesObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.HillHeightMapCount, Is.EqualTo(2), nameof(struc.HillHeightMapCount));
				Assert.That(struc.MountainHeightMapCount, Is.EqualTo(2), nameof(struc.MountainHeightMapCount));
				//Assert.That(struc.var_08, Is.EqualTo(0), nameof(struc.var_08));
				CollectionAssert.AreEqual(struc.pad_0C, Array.CreateInstance(typeof(byte), 2), nameof(struc.pad_0C));
			});
		}

		[TestCase(".DAT")]
		public void LoadIndustryObject(string objectName)
			=> Assert.Fail();

		[TestCase(".DAT")]
		public void LoadInterfaceSkinObject(string objectName)
			=> Assert.Fail();

		[TestCase("GRASS1.DAT")]
		public void LoadLandObject(string objectName)
		{
			var (obj, struc) = LoadObject<LandObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.CostIndex, Is.EqualTo(2), nameof(struc.CostIndex));
				Assert.That(struc.var_03, Is.EqualTo(5), nameof(struc.var_03));
				Assert.That(struc.var_04, Is.EqualTo(1), nameof(struc.var_04));
				Assert.That(struc.Flags, Is.EqualTo(LandObjectFlags.unk0), nameof(struc.Flags));
				//Assert.That(struc.var_06, Is.EqualTo(0), nameof(struc.var_06));
				//Assert.That(struc.var_07, Is.EqualTo(0), nameof(struc.var_07));
				Assert.That(struc.CostFactor, Is.EqualTo(20), nameof(struc.CostFactor));
				//Assert.That(struc.pad_09, Is.EqualTo(0), nameof(struc.pad_09));
				//Assert.That(struc.var_0E, Is.EqualTo(0), nameof(struc.var_0E));
				//Assert.That(struc.CliffEdgeImage, Is.EqualTo(0), nameof(struc.CliffEdgeImage));
				//Assert.That(struc.mapPixelImage, Is.EqualTo(0), nameof(struc.mapPixelImage));
				//Assert.That(struc.pad_1A, Is.EqualTo(0), nameof(struc.pad_1A));
				Assert.That(struc.NumVariations, Is.EqualTo(3), nameof(struc.NumVariations));
				Assert.That(struc.VariationLikelihood, Is.EqualTo(10), nameof(struc.VariationLikelihood));
				//Assert.That(struc.pad_1D, Is.EqualTo(0), nameof(struc.pad_1D));
			});
		}

		[TestCase("LCROSS1.DAT")]
		public void LoadLevelCrossingObject(string objectName)
		{
			var (obj, struc) = LoadObject<LevelCrossingObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.CostFactor, Is.EqualTo(30), nameof(struc.CostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-10), nameof(struc.SellCostFactor));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));

				Assert.That(struc.AnimationSpeed, Is.EqualTo(3), nameof(struc.AnimationSpeed));
				Assert.That(struc.ClosingFrames, Is.EqualTo(4), nameof(struc.ClosingFrames));
				Assert.That(struc.ClosedFrames, Is.EqualTo(11), nameof(struc.ClosedFrames));

				Assert.That(struc.pad_0A[0], Is.EqualTo(3), nameof(struc.pad_0A) + "[0]");
				Assert.That(struc.pad_0A[1], Is.EqualTo(0), nameof(struc.pad_0A) + "[1]");

				Assert.That(struc.DesignedYear, Is.EqualTo(1955), nameof(struc.DesignedYear));
			});
		}

		[TestCase("REGUK.DAT")]
		public void LoadRegionObject(string objectName)
		{
			var (obj, struc) = LoadObject<RegionObject>(objectName);

			Assert.Multiple(() =>
			{
				CollectionAssert.AreEqual(struc.pad_06, Array.CreateInstance(typeof(byte), 2), nameof(struc.pad_06));
				Assert.That(struc.RequiredObjectCount, Is.EqualTo(1), nameof(struc.RequiredObjectCount));
				//CollectionAssert.AreEqual(struc.requiredObjects, Array.CreateInstance(typeof(byte), 4), nameof(struc.requiredObjects));
				CollectionAssert.AreEqual(struc.pad_0D, Array.CreateInstance(typeof(byte), 5), nameof(struc.pad_0D));
			});
		}

		[TestCase("ROADONE.DAT")]
		public void LoadRoadObject(string objectName)
		{
			var (obj, struc) = LoadObject<RoadObject>(objectName);
			Assert.Fail();
		}

		[TestCase("RDEXCAT1.DAT")]
		public void LoadRoadExtraObject(string objectName)
		{
			var (obj, struc) = LoadObject<RoadExtraObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.RoadPieces, Is.EqualTo(127), nameof(struc.RoadPieces));
				Assert.That(struc.PaintStyle, Is.EqualTo(1), nameof(struc.PaintStyle));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
				Assert.That(struc.BuildCostFactor, Is.EqualTo(4), nameof(struc.BuildCostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-3), nameof(struc.SellCostFactor));
				Assert.That(struc.var_0E, Is.EqualTo(0), nameof(struc.var_0E));
			});
		}

		[TestCase("RDSTAT1.DAT")]
		public void LoadRoadStationObject(string objectName)
		{
			var (obj, struc) = LoadObject<RoadStationObject>(objectName);
			Assert.Fail();
		}

		[TestCase("SCAFDEF.DAT")]
		public void LoadScaffoldingObject(string objectName)
		{
			var (obj, struc) = LoadObject<ScaffoldingObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.SegmentHeights[0], Is.EqualTo(16), nameof(struc.SegmentHeights) + "[0]");
				Assert.That(struc.SegmentHeights[1], Is.EqualTo(16), nameof(struc.SegmentHeights) + "[1]");
				Assert.That(struc.SegmentHeights[2], Is.EqualTo(32), nameof(struc.SegmentHeights) + "[2]");

				Assert.That(struc.RoofHeights[0], Is.EqualTo(0), nameof(struc.RoofHeights) + "[0]");
				Assert.That(struc.RoofHeights[1], Is.EqualTo(0), nameof(struc.RoofHeights) + "[1]");
				Assert.That(struc.RoofHeights[2], Is.EqualTo(14), nameof(struc.RoofHeights) + "[2]");
			});
		}

		[TestCase("STEX000.DAT")]
		public void LoadScenarioTextObject(string objectName)
		{
			var (obj, struc) = LoadObject<ScenarioTextObject>(objectName);
			Assert.That(struc.pad_04, Is.EqualTo(0), nameof(struc.pad_04));
		}

		[TestCase("SNOW.DAT")]
		public void LoadSnowObject(string objectName)
		{
			var (obj, struc) = LoadObject<SnowObject>(objectName);
			Assert.Fail();
		}

		[TestCase("SNDA1.DAT")]
		public void LoadSoundObject(string objectName)
		{
			Assert.Fail();
		}

		[TestCase("STEAM.DAT")]
		public void LoadSteamObject(string objectName)
		{
			Assert.Fail();
		}

		[TestCase("SLIGHT1.DAT")]
		public void LoadStreetLightObject(string objectName)
		{
			var (obj, struc) = LoadObject<StreetLightObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.DesignedYear[0], Is.EqualTo(1900), nameof(struc.DesignedYear) + "[0]");
				Assert.That(struc.DesignedYear[1], Is.EqualTo(1950), nameof(struc.DesignedYear) + "[1]");
				Assert.That(struc.DesignedYear[2], Is.EqualTo(1985), nameof(struc.DesignedYear) + "[2]");

				//Assert.That(struc.Image, Is.EqualTo(0));

				Assert.That(obj.StringTable["Name"].Count, Is.EqualTo(2));
				Assert.That(obj.StringTable["Name"][LanguageId.english_uk], Is.EqualTo("Street Lights"));
				Assert.That(obj.StringTable["Name"][LanguageId.english_us], Is.EqualTo("Street Lights"));
			});
		}

		[TestCase("ATOWNNAM.DAT")]
		public void LoadTownNamesObject(string objectName)
		{
			var (obj, struc) = LoadObject<TownNamesObject>(objectName);
			Assert.Fail();
		}

		[TestCase("TRACKST.DAT")]
		public void LoadTrackObject(string objectName)
		{
			var (obj, struc) = LoadObject<TrackObject>(objectName);
			Assert.Fail();
		}

		[TestCase("TREXCAT1.DAT")]
		public void LoadTrackExtraObject(string objectName)
		{
			var (obj, struc) = LoadObject<TrackExtraObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.TrackPieces, Is.EqualTo(1023), nameof(struc.TrackPieces));
				Assert.That(struc.PaintStyle, Is.EqualTo(1), nameof(struc.PaintStyle));
				Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
				Assert.That(struc.BuildCostFactor, Is.EqualTo(2), nameof(struc.BuildCostFactor));
				Assert.That(struc.SellCostFactor, Is.EqualTo(-1), nameof(struc.SellCostFactor));
				Assert.That(struc.var_0E, Is.EqualTo(0), nameof(struc.var_0E));
			});
		}

		[TestCase("SIGUS.DAT")]
		public void LoadTrainSignalObject(string objectName)
		{
			var (obj, struc) = LoadObject<TrainSignalObject>(objectName);

			Assert.Fail();
		}

		[TestCase("TRSTAT1.DAT")]
		public void LoadTrainStationObject(string objectName)
		{
			var (obj, struc) = LoadObject<TrainStationObject>(objectName);
			Assert.Fail();
		}

		[TestCase("BEECH.DAT")]
		public void LoadTreeObject(string objectName)
		{
			var (obj, struc) = LoadObject<TreeObject>(objectName);

			Assert.Multiple(() =>
			{
				//Assert.That(struc.var_02, Is.EqualTo(40), nameof(struc.var_02));
				Assert.That(struc.Height, Is.EqualTo(131), nameof(struc.Height));
				Assert.That(struc.var_04, Is.EqualTo(27), nameof(struc.var_04));
				Assert.That(struc.var_05, Is.EqualTo(83), nameof(struc.var_05));
				Assert.That(struc.NumRotations, Is.EqualTo(1), nameof(struc.NumRotations));
				Assert.That(struc.Growth, Is.EqualTo(4), nameof(struc.Growth));
				Assert.That(struc.Flags, Is.EqualTo(TreeObjectFlags.HighAltitude | TreeObjectFlags.RequiresWater | TreeObjectFlags.HasShadow), nameof(struc.Flags));
				CollectionAssert.AreEqual(struc.Sprites, Array.CreateInstance(typeof(byte), 6), nameof(struc.Sprites));
				CollectionAssert.AreEqual(struc.SnowSprites, Array.CreateInstance(typeof(byte), 6), nameof(struc.SnowSprites));
				Assert.That(struc.ShadowImageOffset, Is.EqualTo(0), nameof(struc.ShadowImageOffset));
				Assert.That(struc.var_3C, Is.EqualTo(15), nameof(struc.var_3C));
				Assert.That(struc.SeasonState, Is.EqualTo(3), nameof(struc.SeasonState));
				Assert.That(struc.var_3E, Is.EqualTo(2), nameof(struc.var_3E));
				Assert.That(struc.CostIndex, Is.EqualTo(3), nameof(struc.CostIndex));
				Assert.That(struc.BuildCostFactor, Is.EqualTo(8), nameof(struc.BuildCostFactor));
				Assert.That(struc.ClearCostFactor, Is.EqualTo(4), nameof(struc.ClearCostFactor));
				Assert.That(struc.Colours, Is.EqualTo(0), nameof(struc.Colours));
				Assert.That(struc.Rating, Is.EqualTo(10), nameof(struc.Rating));
				Assert.That(struc.DemolishRatingReduction, Is.EqualTo(-15), nameof(struc.DemolishRatingReduction));
			});
		}

		[TestCase("TUNNEL1.DAT")]
		public void LoadTunnelObject(string objectName)
		{
			var (obj, struc) = LoadObject<TunnelObject>(objectName);
			Assert.Fail();
		}

		[TestCase("707.DAT")]
		public void LoadVehicleAircraftObject(string objectName)
		{
			var (obj, struc) = LoadObject<VehicleObject>(objectName);

			//var s5header = obj.S5Header;
			//var objHeader = obj.ObjectHeader;

			//Assert.Multiple(() =>
			//{
			//	Assert.That(s5header.Flags, Is.EqualTo(283680407), nameof(s5header.Flags));
			//	Assert.That(s5header.Name, Is.EqualTo("707     "), nameof(s5header.Name));
			//	Assert.That(s5header.Checksum, Is.EqualTo(1331114877), nameof(s5header.Checksum));
			//	Assert.That(s5header.ObjectType, Is.EqualTo(ObjectType.Vehicle), nameof(s5header.ObjectType));

			//	Assert.That(objHeader.Encoding, Is.EqualTo(SawyerEncoding.RunLengthSingle), nameof(objHeader.Encoding));
			//	Assert.That(objHeader.DataLength, Is.EqualTo(159566), nameof(objHeader.DataLength));
			//});

			Assert.Multiple(() =>
			{
				Assert.That(struc.Mode, Is.EqualTo(TransportMode.Air), nameof(struc.Mode));
				Assert.That(struc.Type, Is.EqualTo(VehicleType.Aircraft), nameof(struc.Type));
				Assert.That(struc.var_04, Is.EqualTo(1), nameof(struc.var_04));
				// Assert.That(struc.TrackType, Is.EqualTo(0xFF), nameof(struc.TrackType)); // is changed after load from 0 to 255
				Assert.That(struc.NumTrackExtras, Is.EqualTo(0), nameof(struc.NumTrackExtras));
				Assert.That(struc.CostIndex, Is.EqualTo(8), nameof(struc.CostIndex));
				Assert.That(struc.CostFactor, Is.EqualTo(345), nameof(struc.CostFactor));
				Assert.That(struc.Reliability, Is.EqualTo(88), nameof(struc.Reliability));
				Assert.That(struc.RunCostIndex, Is.EqualTo(4), nameof(struc.RunCostIndex));
				Assert.That(struc.RunCostFactor, Is.EqualTo(55), nameof(struc.RunCostFactor));
				Assert.That(struc.ColourType, Is.EqualTo(9), nameof(struc.ColourType));
				Assert.That(struc.NumCompatibleVehicles, Is.EqualTo(0), nameof(struc.NumCompatibleVehicles));
				CollectionAssert.AreEqual(Enumerable.Repeat(0, 8).ToArray(), struc.CompatibleVehicles, nameof(struc.CompatibleVehicles));
				CollectionAssert.AreEqual(Enumerable.Repeat(0, 4).ToArray(), struc.RequiredTrackExtras, nameof(struc.RequiredTrackExtras));
				//Assert.That(struc.var_24, Is.EqualTo(0), nameof(struc.var_24));
				//Assert.That(struc.BodySprites, Is.EqualTo(0), nameof(struc.BodySprites));
				//Assert.That(struc.BogieSprites, Is.EqualTo(1), nameof(struc.BogieSprites));
				Assert.That(struc.Power, Is.EqualTo(3000), nameof(struc.Power));
				Assert.That(struc.Speed, Is.EqualTo(604), nameof(struc.Speed));
				Assert.That(struc.RackSpeed, Is.EqualTo(120), nameof(struc.RackSpeed));
				Assert.That(struc.Weight, Is.EqualTo(141), nameof(struc.Weight));
				Assert.That(struc.Flags, Is.EqualTo((VehicleObjectFlags)16384), nameof(struc.Flags));
				// CollectionAssert.AreEqual(struc.MaxCargo, Enumerable.Repeat(0, 2).ToArray(), nameof(struc.MaxCargo)); // this is changed after load from 0 to 24
				CollectionAssert.AreEqual(Enumerable.Repeat(0, 2).ToArray(), struc.CompatibleCargoCategories, nameof(struc.CompatibleCargoCategories));
				CollectionAssert.AreEqual(Enumerable.Repeat(0, 32).ToArray(), struc.CargoTypeSpriteOffsets, nameof(struc.CargoTypeSpriteOffsets));
				Assert.That(struc.NumSimultaneousCargoTypes, Is.EqualTo(1), nameof(struc.NumSimultaneousCargoTypes));
				Assert.That(struc.Animation[0].ObjectId, Is.EqualTo(0), nameof(struc.Animation));
				Assert.That(struc.Animation[0].Height, Is.EqualTo(24), nameof(struc.Animation));
				Assert.That(struc.Animation[0].Type, Is.EqualTo(SimpleAnimationType.None), nameof(struc.Animation));
				Assert.That(struc.Animation[1].ObjectId, Is.EqualTo(0), nameof(struc.Animation));
				Assert.That(struc.Animation[1].Height, Is.EqualTo(0), nameof(struc.Animation));
				Assert.That(struc.Animation[1].Type, Is.EqualTo(SimpleAnimationType.None), nameof(struc.Animation));
				Assert.That(struc.var_113, Is.EqualTo(0), nameof(struc.var_113));
				Assert.That(struc.Designed, Is.EqualTo(1957), nameof(struc.Designed));
				Assert.That(struc.Obsolete, Is.EqualTo(1987), nameof(struc.Obsolete));
				Assert.That(struc.RackRailType, Is.EqualTo(0), nameof(struc.RackRailType));
				//Assert.That(struc.DrivingSoundType, Is.EqualTo(DrivingSoundType.Engine1), nameof(struc.DrivingSoundType));
				//Assert.That(struc.Sound, Is.EqualTo(0), nameof(struc.Sound));
				//Assert.That(struc.pad_135, Is.EqualTo(0), nameof(struc.pad_135));
				Assert.That(struc.NumStartSounds, Is.EqualTo(2), nameof(struc.NumStartSounds));
				CollectionAssert.AreEqual(Enumerable.Repeat(0, 3).ToArray(), struc.StartSounds, nameof(struc.StartSounds));
			});
		}

		[TestCase("FENCE1.DAT")]
		public void LoadWallObject(string objectName)
		{
			var (obj, struc) = LoadObject<WallObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.var_06, Is.EqualTo(15), nameof(struc.var_06));
				Assert.That(struc.Flags, Is.EqualTo(WallObjectFlags.None), nameof(struc.Flags));
				Assert.That(struc.Height, Is.EqualTo(2), nameof(struc.Height));
				Assert.That(struc.var_09, Is.EqualTo(8), nameof(struc.var_09));
			});
		}

		[TestCase("WATER1.DAT")]
		public void LoadWaterObject(string objectName)
		{
			var (obj, struc) = LoadObject<WaterObject>(objectName);

			Assert.Multiple(() =>
			{
				Assert.That(struc.CostIndex, Is.EqualTo(2), nameof(struc.CostIndex));
				Assert.That(struc.var_03, Is.EqualTo(0), nameof(struc.var_03));
				Assert.That(struc.CostFactor, Is.EqualTo(51), nameof(struc.CostFactor));
				Assert.That(struc.var_05, Is.EqualTo(0), nameof(struc.var_05));
				//Assert.That(struc.var_0A, Is.EqualTo(0), nameof(struc.var_0A));
			});
		}
	}
}