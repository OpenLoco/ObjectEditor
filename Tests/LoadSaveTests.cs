using Dat.Converters;
using Dat.FileParsing;
using Dat.Loaders;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.CliffEdge;
using Definitions.ObjectModels.Objects.Climate;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Currency;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Objects.HillShape;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Scaffolding;
using Definitions.ObjectModels.Objects.ScenarioText;
using Definitions.ObjectModels.Objects.Snow;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Tree;
using Definitions.ObjectModels.Objects.Tunnel;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Objects.Wall;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

[TestFixture]
public class LoadSaveTests
{
	static (DatHeaderInfo, LocoObject, T) LoadObject<T>(string filename) where T : ILocoStruct
		=> LoadObject<T>(File.ReadAllBytes(Path.Combine(TestConstants.BaseSteamObjDataPath, filename)));

	static (DatHeaderInfo, LocoObject, T) LoadObject<T>(ReadOnlySpan<byte> data) where T : ILocoStruct
	{
		var logger = new Logger();
		var (datFileInfo, locoObject) = SawyerStreamReader.LoadFullObject(data.ToArray(), logger);

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable NUnit2045 // Use Assert.Multiple - cannot use a ReadOnlySpan inside an anonymous method
		Assert.That(locoObject, Is.Not.Null);
		Assert.That(datFileInfo.ObjectHeader.DataLength, Is.EqualTo(data.Length - S5Header.StructLength - ObjectHeader.StructLength), "ObjectHeader.Length didn't match actual size of struct");
#pragma warning restore NUnit2045 // Use Assert.Multiple
#pragma warning restore IDE0079 // Remove unnecessary suppression

		return (datFileInfo, locoObject!, (T)locoObject!.Object);
	}

	static void LoadSaveGenericTest<T>(string filename, Action<LocoObject, T> assertFunc) where T : ILocoStruct
	{
		var logger = new Logger();

		var (datInfo1, obj1, struc1) = LoadObject<T>(filename);
		assertFunc(obj1, struc1);
		var bytes1 = SawyerStreamWriter.WriteLocoObject(datInfo1.S5Header.Name, obj1.ObjectType, datInfo1.S5Header.ObjectSource.Convert(datInfo1.S5Header.Name, datInfo1.S5Header.Checksum), datInfo1.ObjectHeader.Encoding, logger, obj1, true).ToArray();

		var (datInfo2, obj2, struc2) = LoadObject<T>(bytes1);
		assertFunc(obj2, struc2);
		var bytes2 = SawyerStreamWriter.WriteLocoObject(datInfo2.S5Header.Name, obj2.ObjectType, datInfo2.S5Header.ObjectSource.Convert(datInfo2.S5Header.Name, datInfo2.S5Header.Checksum), datInfo2.ObjectHeader.Encoding, logger, obj2, true).ToArray();

		// grab headers first
		var s5Header1 = S5Header.Read(bytes1.AsSpan()[0..S5Header.StructLength]);
		var s5Header2 = S5Header.Read(bytes2.AsSpan()[0..S5Header.StructLength]);
		AssertS5Headers(s5Header1, s5Header2);

		var objHeader1 = ObjectHeader.Read(bytes1.AsSpan()[S5Header.StructLength..(S5Header.StructLength + ObjectHeader.StructLength)]);
		var objHeader2 = ObjectHeader.Read(bytes2.AsSpan()[S5Header.StructLength..(S5Header.StructLength + ObjectHeader.StructLength)]);
		AssertObjHeaders(objHeader1, objHeader2);

		// then grab object bytes
		var bytes1ObjArr = bytes1[21..].ToArray();
		var bytes2ObjArr = bytes2[21..].ToArray();
		Assert.That(bytes1ObjArr.ToArray(), Is.EqualTo(bytes2ObjArr.ToArray()));
	}

	static void AssertS5Headers(S5Header expected, S5Header actual)
		=> Assert.Multiple(() =>
			{
				Assert.That(actual.Name, Is.EqualTo(expected.Name));
				Assert.That(actual.Flags, Is.EqualTo(expected.Flags));
				Assert.That(actual.Checksum, Is.EqualTo(expected.Checksum));
			});

	static void AssertObjHeaders(ObjectHeader expected, ObjectHeader actual)
		=> Assert.Multiple(() =>
			{
				Assert.That(actual.Encoding, Is.EqualTo(expected.Encoding));
				Assert.That(actual.DataLength, Is.EqualTo(expected.DataLength));
			});

	[TestCase("AIRPORT1.DAT")]
	public void AirportObject(string objectName)
	{
		void assertFunc(LocoObject obj, AirportObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(256), nameof(struc.BuildCostFactor));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-192), nameof(struc.SellCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			// Assert.That(struc.Image, Is.Zero, nameof(struc.Image));
			//Assert.That(struc.ImageOffset, Is.Zero, nameof(struc.ImageOffset));
			Assert.That(struc.Flags, Is.EqualTo(AirportObjectFlags.AcceptsHeavyPlanes | AirportObjectFlags.AcceptsHelicopter), nameof(struc.Flags));
			Assert.That(struc.BuildingComponents.BuildingHeights, Has.Count.EqualTo(94), nameof(struc.BuildingComponents.BuildingHeights));
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(94), nameof(struc.BuildingComponents.BuildingAnimations));
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(23), nameof(struc.BuildingComponents.BuildingVariations));

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
			Assert.That(struc.MovementNodes, Has.Count.EqualTo(26), nameof(struc.MovementNodes));
			Assert.That(struc.MovementEdges, Has.Count.EqualTo(30), nameof(struc.MovementEdges));

			//Assert.That(struc.MovementNodes, Is.EqualTo(0), nameof(struc.MovementNodes));
			//Assert.That(struc.MovementEdges, Is.EqualTo(0), nameof(struc.MovementEdges));

			Assert.That(struc.var_B6, Is.EqualTo(4864), nameof(struc.var_B6));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(377));
		});
		LoadSaveGenericTest<AirportObject>(objectName, assertFunc);
	}

	[TestCase("BRDGBRCK.DAT")]
	public void BridgeObject(string objectName)
	{
		void assertFunc(LocoObject obj, BridgeObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.Flags, Is.EqualTo(BridgeObjectFlags.None), nameof(struc.Flags));

			// Assert.That(struc.var_03[0], Is.EqualTo(0), nameof(struc.var_03) + "[0]");
			// Assert.That(struc.var_03[1], Is.EqualTo(0), nameof(struc.var_03) + "[1]");
			// Assert.That(struc.var_03[2], Is.EqualTo(0), nameof(struc.var_03) + "[2]");

			Assert.That(struc.DeckDepth, Is.EqualTo(16), nameof(struc.DeckDepth));
			Assert.That(struc.SpanLength, Is.EqualTo(1), nameof(struc.SpanLength));
			Assert.That(struc.PillarSpacing, Is.EqualTo(
				SupportPillarSpacing.Tile0A
				| SupportPillarSpacing.Tile0B
				| SupportPillarSpacing.Tile1A
				| SupportPillarSpacing.Tile1B
				| SupportPillarSpacing.Tile2A
				| SupportPillarSpacing.Tile2B
				| SupportPillarSpacing.Tile3A
				| SupportPillarSpacing.Tile3B), nameof(struc.PillarSpacing));
			Assert.That(struc.MaxSpeed, Is.EqualTo(60), nameof(struc.MaxSpeed));
			Assert.That(struc.MaxHeight, Is.EqualTo(10), nameof(struc.MaxHeight));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.BaseCostFactor, Is.EqualTo(16), nameof(struc.BaseCostFactor));
			Assert.That(struc.HeightCostFactor, Is.EqualTo(8), nameof(struc.HeightCostFactor));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-12), nameof(struc.SellCostFactor));
			Assert.That(struc.DisabledTrackFlags, Is.EqualTo(BridgeDisabledTrackFlags.None), nameof(struc.DisabledTrackFlags));
			//Assert.That(struc.TrackNumCompatible, Is.EqualTo(0), nameof(struc.TrackNumCompatible));
			//CollectionAssert.AreEqual(struc.TrackMods, Array.CreateInstance(typeof(byte), 7), nameof(struc.TrackMods));
			//Assert.That(struc.RoadNumCompatible, Is.EqualTo(0), nameof(struc.RoadNumCompatible));
			//CollectionAssert.AreEqual(struc.RoadMods, Array.CreateInstance(typeof(byte), 7), nameof(struc.RoadMods));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(124));
		});
		LoadSaveGenericTest<BridgeObject>(objectName, assertFunc);
	}

	[TestCase("HQ1.DAT")]
	public void BuildingObject(string objectName)
	{
		void assertFunc(LocoObject obj, BuildingObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(5), nameof(struc.BuildingComponents.BuildingVariations));
			// CollectionAssert.AreEqual(struc.VariationHeights, Array.CreateInstance(typeof(byte), 4), nameof(struc.VariationHeights));
			// VariationHeights
			// VariationAnimations
			// VariationParts
			Assert.That(struc.Colours, Is.Zero, nameof(struc.Colours));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
			Assert.That(struc.Flags, Is.EqualTo(BuildingObjectFlags.LargeTile | BuildingObjectFlags.MiscBuilding | BuildingObjectFlags.IsHeadquarters), nameof(struc.Flags));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.SellCostFactor, Is.Zero, nameof(struc.SellCostFactor));
			Assert.That(struc.ScaffoldingSegmentType, Is.EqualTo(1), nameof(struc.ScaffoldingSegmentType));
			Assert.That(struc.ScaffoldingColour, Is.EqualTo(Colour.Yellow), nameof(struc.ScaffoldingColour));
			Assert.That(struc.GeneratorFunction, Is.EqualTo(3), nameof(struc.GeneratorFunction));
			Assert.That(struc.AverageNumberOnMap, Is.EqualTo(3), nameof(struc.AverageNumberOnMap));
			// ProducedQuantity
			// ProducedCargoType
			// RequiredCargoType
			// CollectionAssert.AreEqual(struc.var_A6, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A6));
			// CollectionAssert.AreEqual(struc.var_A8, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A8));
			// CollectionAssert.AreEqual(struc.var_A4, Array.CreateInstance(typeof(byte), 2), nameof(struc.var_A4));
			Assert.That(struc.DemolishRatingReduction, Is.Zero, nameof(struc.DemolishRatingReduction));
			Assert.That(struc.var_AC, Is.EqualTo(255), nameof(struc.var_AC));
			Assert.That(struc.ElevatorHeightSequences, Is.Empty, nameof(struc.ElevatorHeightSequences));
			//Assert.That(struc.ElevatorHeightSequences[0].Count, Is.Zero, nameof(struc.ElevatorHeightSequences) + "[0]");
			//Assert.That(struc.ElevatorHeightSequences[1].Count, Is.Zero, nameof(struc.ElevatorHeightSequences) + "[1]");
			//Assert.That(struc.ElevatorHeightSequences[2].Count, Is.Zero, nameof(struc.ElevatorHeightSequences) + "[2]");
			//Assert.That(struc.ElevatorHeightSequences[3].Count, Is.Zero, nameof(struc.ElevatorHeightSequences) + "[3]");

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(64));
		});
		LoadSaveGenericTest<BuildingObject>(objectName, assertFunc);
	}

	[TestCase("CHEMICAL.DAT")]
	public void CargoObject(string objectName)
	{
		void assertFunc(LocoObject obj, CargoObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.UnitWeight, Is.EqualTo(256), nameof(struc.UnitWeight));
			Assert.That(struc.CargoTransferTime, Is.EqualTo(64), nameof(struc.CargoTransferTime));
			//Assert.That(struc.UnitInlineSprite, Is.Zero, nameof(struc.UnitInlineSprite));
			Assert.That(struc.CargoCategory, Is.EqualTo(CargoCategory.Liquids), nameof(struc.CargoCategory));
			Assert.That(struc.Flags, Is.EqualTo(CargoObjectFlags.Delivering), nameof(struc.Flags));
			Assert.That(struc.NumPlatformVariations, Is.EqualTo(1), nameof(struc.NumPlatformVariations));
			Assert.That(struc.StationCargoDensity, Is.EqualTo(4), nameof(struc.StationCargoDensity));
			Assert.That(struc.PremiumDays, Is.EqualTo(10), nameof(struc.PremiumDays));
			Assert.That(struc.MaxNonPremiumDays, Is.EqualTo(30), nameof(struc.MaxNonPremiumDays));
			Assert.That(struc.NonPremiumRate, Is.EqualTo(128), nameof(struc.NonPremiumRate));
			Assert.That(struc.PenaltyRate, Is.EqualTo(256), nameof(struc.PenaltyRate));
			Assert.That(struc.PaymentFactor, Is.EqualTo(62), nameof(struc.PaymentFactor));
			Assert.That(struc.PaymentIndex, Is.EqualTo(10), nameof(struc.PaymentIndex));
			Assert.That(struc.UnitSize, Is.EqualTo(10), nameof(struc.UnitSize));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(9));
		});
		LoadSaveGenericTest<CargoObject>(objectName, assertFunc);
	}

	[TestCase("LSBROWN.DAT")]
	public void CliffEdgeObject(string objectName)
	{
		void assertFunc(LocoObject obj, CliffEdgeObject _) => Assert.Multiple(() =>
		{
			var strTable = obj.StringTable;

			Assert.That(strTable.Table, Has.Count.EqualTo(1));
			Assert.That(strTable.Table.ContainsKey("Name"), Is.True);

			var entry = strTable.Table["Name"];

			Assert.That(entry[LanguageId.English_UK], Is.EqualTo("Brown Rock"));
			Assert.That(entry[LanguageId.English_US], Is.EqualTo("Brown Rock"));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(70));
		});
		LoadSaveGenericTest<CliffEdgeObject>(objectName, assertFunc);
	}

	[TestCase("CLIM1.DAT")]
	public void ClimateObject(string objectName)
	{
		void assertFunc(LocoObject obj, ClimateObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.FirstSeason, Is.EqualTo(Season.Winter), nameof(struc.FirstSeason));
			Assert.That(struc.SeasonLength1, Is.EqualTo(57), nameof(struc.SeasonLength1));
			Assert.That(struc.SeasonLength2, Is.EqualTo(80), nameof(struc.SeasonLength2));
			Assert.That(struc.SeasonLength3, Is.EqualTo(100), nameof(struc.SeasonLength3));
			Assert.That(struc.SeasonLength4, Is.EqualTo(80), nameof(struc.SeasonLength4));
			Assert.That(struc.WinterSnowLine, Is.EqualTo(48), nameof(struc.WinterSnowLine));
			Assert.That(struc.SummerSnowLine, Is.EqualTo(76), nameof(struc.SummerSnowLine));

			Assert.That(obj.ImageTable, Is.Null);
		});
		LoadSaveGenericTest<ClimateObject>(objectName, assertFunc);
	}

	[TestCase("COMP1.DAT")]
	public void CompetitorObject(string objectName)
	{
		void assertFunc(LocoObject obj, CompetitorObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.AvailableNamePrefixes,
				Is.EqualTo(
					NamePrefixFlags.unk4
					| NamePrefixFlags.unk9
					| NamePrefixFlags.unk11
					| NamePrefixFlags.unk12),
				nameof(struc.AvailableNamePrefixes));

			Assert.That(struc.AvailablePlayStyles,
				Is.EqualTo(
					PlaystyleFlags.unk0
					| PlaystyleFlags.unk2
					| PlaystyleFlags.unk11),
				nameof(struc.AvailablePlayStyles));

			Assert.That(struc.Emotions,
				Is.EqualTo(
					EmotionFlags.Neutral
					| EmotionFlags.Happy
					| EmotionFlags.Worried
					| EmotionFlags.Thinking
					| EmotionFlags.Dejected
					| EmotionFlags.Surprised
					| EmotionFlags.Surprised
					| EmotionFlags.Scared
					| EmotionFlags.Angry
					| EmotionFlags.Disgusted),
				nameof(struc.Emotions));

			Assert.That(struc.Intelligence, Is.EqualTo(7), nameof(struc.Intelligence));
			Assert.That(struc.Aggressiveness, Is.EqualTo(5), nameof(struc.Aggressiveness));
			Assert.That(struc.Competitiveness, Is.EqualTo(6), nameof(struc.Competitiveness));
			Assert.That(struc.var_37, Is.Zero, nameof(struc.var_37));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(18));
		});
		LoadSaveGenericTest<CompetitorObject>(objectName, assertFunc);
	}

	[TestCase("CURRDOLL.DAT")]
	public void CurrencyObject(string objectName)
	{
		void assertFunc(LocoObject obj, CurrencyObject struc) => Assert.Multiple(() =>
		{
			//Assert.That(struc.ObjectIcon, Is.Zero, nameof(struc.ObjectIcon));
			Assert.That(struc.Separator, Is.Zero, nameof(struc.Separator));
			Assert.That(struc.Factor, Is.EqualTo(1), nameof(struc.Factor));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(5));
		});
		LoadSaveGenericTest<CurrencyObject>(objectName, assertFunc);
	}

	[TestCase("SHIPST1.DAT")]
	public void DockObject(string objectName)
	{
		void assertFunc(LocoObject obj, DockObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(38), nameof(struc.BuildCostFactor));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-35), nameof(struc.SellCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.var_07, Is.Zero, nameof(struc.var_07));
			//Assert.That(struc.UnkImage, Is.Zero, nameof(struc.UnkImage));
			Assert.That(struc.Flags, Is.EqualTo(DockObjectFlags.None), nameof(struc.Flags));
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(2), nameof(struc.BuildingComponents.BuildingAnimations));
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(1), nameof(struc.BuildingComponents.BuildingVariations));

			//Assert.That(struc.var_14, Is.EqualTo(1), nameof(struc.var_14));
			//Assert.That(struc.var_14, Is.EqualTo(1), nameof(struc.var_18));
			//Assert.That(struc.var_1C[0], Is.EqualTo(1), nameof(struc.var_1C[0]));

			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
			Assert.That(struc.BoatPosition.X, Is.EqualTo(48), nameof(struc.BoatPosition));
			Assert.That(struc.BoatPosition.Y, Is.EqualTo(0), nameof(struc.BoatPosition));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(9));
		});
		LoadSaveGenericTest<DockObject>(objectName, assertFunc);
	}

	[TestCase("HS1.DAT")]
	public void HillShapesObject(string objectName)
	{
		void assertFunc(LocoObject obj, HillShapesObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.HillHeightMapCount, Is.EqualTo(2), nameof(struc.HillHeightMapCount));
			Assert.That(struc.MountainHeightMapCount, Is.EqualTo(2), nameof(struc.MountainHeightMapCount));
			//Assert.That(struc.var_08, Is.EqualTo(0), nameof(struc.var_08));
			Assert.That(struc.IsHeightMap, Is.False, nameof(struc.IsHeightMap));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(5));
		});
		LoadSaveGenericTest<HillShapesObject>(objectName, assertFunc);
	}

	[TestCase("CHEMWORK.DAT")]
	public void IndustryObject(string objectName)
	{
		void assertFunc(LocoObject obj, IndustryObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.AnimationSequences, Is.All.EqualTo(new byte[0]));
			Assert.That(struc.Colours, Is.EqualTo(4), nameof(struc.Colours));
			// Buildings
			Assert.That(struc.BuildingSizeFlags, Is.EqualTo(7), nameof(struc.BuildingSizeFlags));
			// BuildingPartHeights
			Assert.That(struc.BuildingComponents.BuildingHeights, Is.EqualTo(new List<byte>() { 0, 56, 0, 66, 0, 122, 0, 48, 0, 36 }));
			// BuildingPartAnimations
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(10));
			Assert.That(struc.BuildingComponents.BuildingAnimations[0].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[0].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[1].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[1].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[2].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[2].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[3].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[3].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[4].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[4].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[5].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[5].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[6].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[6].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[7].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[7].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[8].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[8].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[9].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[9].AnimationSpeed, Is.Zero);
			// BuildingParts
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(5));
			Assert.That(struc.BuildingComponents.BuildingVariations[0], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[0][0], Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingVariations[0][1], Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingVariations[1], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[1][0], Is.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[1][1], Is.EqualTo(3));
			Assert.That(struc.BuildingComponents.BuildingVariations[2], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[2][0], Is.EqualTo(4));
			Assert.That(struc.BuildingComponents.BuildingVariations[2][1], Is.EqualTo(5));
			Assert.That(struc.BuildingComponents.BuildingVariations[3], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[3][0], Is.EqualTo(6));
			Assert.That(struc.BuildingComponents.BuildingVariations[3][1], Is.EqualTo(7));
			Assert.That(struc.BuildingComponents.BuildingVariations[4], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[4][0], Is.EqualTo(8));
			Assert.That(struc.BuildingComponents.BuildingVariations[4][1], Is.EqualTo(9));
			// Rest of object
			Assert.That(struc.SellCostFactor, Is.EqualTo(240), nameof(struc.SellCostFactor));
			Assert.That(struc.BuildCostFactor, Is.EqualTo(400), nameof(struc.BuildCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.Flags, Is.EqualTo(IndustryObjectFlags.BuiltOnLowGround | IndustryObjectFlags.BuiltAwayFromTown | IndustryObjectFlags.CanIncreaseProduction | IndustryObjectFlags.CanDecreaseProduction), nameof(struc.Flags));
			Assert.That(struc.InitialProductionRate[0].Min, Is.EqualTo(8));
			Assert.That(struc.InitialProductionRate[0].Max, Is.EqualTo(12));
			Assert.That(struc.InitialProductionRate[1].Min, Is.Zero);
			Assert.That(struc.InitialProductionRate[1].Max, Is.Zero);
			Assert.That(struc.MaxNumBuildings, Is.EqualTo(11), nameof(struc.MaxNumBuildings));
			Assert.That(struc.MinNumBuildings, Is.EqualTo(9), nameof(struc.MinNumBuildings));
			Assert.That(struc.BuildingComponents.BuildingHeights, Has.Count.EqualTo(10), nameof(struc.BuildingComponents.BuildingHeights));
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(10), nameof(struc.BuildingComponents.BuildingAnimations));
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(5), nameof(struc.BuildingComponents.BuildingVariations));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
			Assert.That(struc.MapColour, Is.EqualTo(Colour.Yellow), nameof(struc.MapColour));
			// ProducedCargo
			Assert.That(struc.ProducedCargo, Has.Count.EqualTo(1));
			Assert.That(struc.ProducedCargo[0].Name, Is.EqualTo("CHEMICAL"));
			Assert.That(struc.ProducedCargo[0].ObjectType, Is.EqualTo(ObjectType.Cargo));
			// RequiredCargo
			Assert.That(struc.RequiredCargo, Has.Count.EqualTo(0));
			// Rest of object
			Assert.That(struc.ScaffoldingColour, Is.EqualTo(Colour.Grey), nameof(struc.ScaffoldingColour));
			Assert.That(struc.ScaffoldingSegmentType, Is.EqualTo(2), nameof(struc.ScaffoldingSegmentType));
			Assert.That(struc.TotalOfTypeInScenario, Is.EqualTo(3), nameof(struc.TotalOfTypeInScenario));
			//Assert.That(struc.var_0E, Is.Zero, nameof(struc.var_0E));
			//Assert.That(struc.var_12, Is.Zero, nameof(struc.var_12));
			//Assert.That(struc.var_16, Is.EqualTo(40), nameof(struc.var_16));
			//Assert.That(struc.var_1A, Is.EqualTo(21), nameof(struc.var_1A));
			Assert.That(struc.var_E8, Is.EqualTo(1), nameof(struc.var_E8));
			Assert.That(struc.FarmTileNumImageAngles, Is.EqualTo(1), nameof(struc.FarmTileNumImageAngles));
			Assert.That(struc.FarmGrowthStageWithNoProduction, Is.Zero, nameof(struc.FarmGrowthStageWithNoProduction));
			Assert.That(struc.FarmNumFields, Is.Zero, nameof(struc.FarmNumFields));
			Assert.That(struc.FarmNumStagesOfGrowth, Is.Zero, nameof(struc.FarmNumStagesOfGrowth));
			Assert.That(struc.MonthlyClosureChance, Is.EqualTo(1), nameof(struc.MonthlyClosureChance));
			// Walls
			Assert.That(struc.WallTypes, Has.Count.EqualTo(0));
			// WallTypes
			Assert.That(struc.BuildingWall!.Name, Is.EqualTo("SECFENCE"));
			Assert.That(struc.BuildingWall!.ObjectType, Is.EqualTo(ObjectType.Wall));
			Assert.That(struc.BuildingWallEntrance!.Name, Is.EqualTo("SECFENCG"));
			Assert.That(struc.BuildingWallEntrance.ObjectType, Is.EqualTo(ObjectType.Wall));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(61));
		});
		LoadSaveGenericTest<IndustryObject>(objectName, assertFunc);
	}

	[TestCase("BREWERY.DAT")]
	public void IndustryObject2(string objectName)
	{
		void assertFunc(LocoObject obj, IndustryObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.AnimationSequences, Is.All.EqualTo(new byte[0]));
			Assert.That(struc.Colours, Is.EqualTo(4), nameof(struc.Colours));
			// Buildings
			Assert.That(struc.BuildingSizeFlags, Is.EqualTo(1), nameof(struc.BuildingSizeFlags));
			//Assert.That(struc._BuildingWall, Is.Zero, nameof(struc._BuildingWall));
			//Assert.That(struc._BuildingWallEntrance, Is.Zero, nameof(struc._BuildingWallEntrance));
			// BuildingPartHeights
			Assert.That(struc.BuildingComponents.BuildingHeights, Is.EqualTo(new List<byte>() { 0, 166, 0, 64, }));
			// BuildingPartAnimations
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(4));
			Assert.That(struc.BuildingComponents.BuildingAnimations[0].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[0].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[1].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[1].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[2].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[2].AnimationSpeed, Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingAnimations[3].NumFrames, Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingAnimations[3].AnimationSpeed, Is.Zero);
			// BuildingParts
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[0], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[0][0], Is.Zero);
			Assert.That(struc.BuildingComponents.BuildingVariations[0][1], Is.EqualTo(1));
			Assert.That(struc.BuildingComponents.BuildingVariations[1], Has.Count.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[1][0], Is.EqualTo(2));
			Assert.That(struc.BuildingComponents.BuildingVariations[1][1], Is.EqualTo(3));

			// Rest of object
			Assert.That(struc.SellCostFactor, Is.EqualTo(240), nameof(struc.SellCostFactor));
			Assert.That(struc.BuildCostFactor, Is.EqualTo(320), nameof(struc.BuildCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.Flags, Is.EqualTo(IndustryObjectFlags.BuiltOnLowGround | IndustryObjectFlags.BuiltNearWater | IndustryObjectFlags.BuiltNearTown | IndustryObjectFlags.BuiltNearDesert | IndustryObjectFlags.CanBeFoundedByPlayer), nameof(struc.Flags));
			Assert.That(struc.InitialProductionRate[0].Min, Is.Zero);
			Assert.That(struc.InitialProductionRate[0].Max, Is.Zero);
			Assert.That(struc.InitialProductionRate[1].Min, Is.Zero);
			Assert.That(struc.InitialProductionRate[1].Max, Is.Zero);
			Assert.That(struc.MaxNumBuildings, Is.EqualTo(8), nameof(struc.MaxNumBuildings));
			Assert.That(struc.MinNumBuildings, Is.EqualTo(4), nameof(struc.MinNumBuildings));
			Assert.That(struc.BuildingComponents.BuildingHeights, Has.Count.EqualTo(4), nameof(struc.BuildingComponents.BuildingHeights));
			Assert.That(struc.BuildingComponents.BuildingAnimations, Has.Count.EqualTo(4), nameof(struc.BuildingComponents.BuildingAnimations));
			Assert.That(struc.BuildingComponents.BuildingVariations, Has.Count.EqualTo(2), nameof(struc.BuildingComponents.BuildingVariations));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
			Assert.That(struc.MapColour, Is.EqualTo(Colour.MutedPurple), nameof(struc.MapColour));
			// ProducedCargo
			Assert.That(struc.ProducedCargo, Has.Count.EqualTo(1));
			Assert.That(struc.ProducedCargo[0].Name, Is.EqualTo("FOOD"));
			Assert.That(struc.ProducedCargo[0].ObjectType, Is.EqualTo(ObjectType.Cargo));
			// RequiredCargo
			Assert.That(struc.RequiredCargo, Has.Count.EqualTo(1));
			Assert.That(struc.RequiredCargo[0].Name, Is.EqualTo("GRAIN"));
			Assert.That(struc.RequiredCargo[0].ObjectType, Is.EqualTo(ObjectType.Cargo));
			// Rest of object
			Assert.That(struc.ScaffoldingColour, Is.EqualTo(Colour.Grey), nameof(struc.ScaffoldingColour));
			Assert.That(struc.ScaffoldingSegmentType, Is.Zero, nameof(struc.ScaffoldingSegmentType));
			Assert.That(struc.TotalOfTypeInScenario, Is.EqualTo(2), nameof(struc.TotalOfTypeInScenario));
			//Assert.That(struc.var_0E, Is.Zero, nameof(struc.var_0E));
			//Assert.That(struc.var_12, Is.Zero, nameof(struc.var_12));
			//Assert.That(struc.var_16, Is.EqualTo(16), nameof(struc.var_16));
			//Assert.That(struc.var_1A, Is.EqualTo(21), nameof(struc.var_1A));
			Assert.That(struc.var_E8, Is.EqualTo(1), nameof(struc.var_E8));
			Assert.That(struc.FarmTileNumImageAngles, Is.EqualTo(1), nameof(struc.FarmTileNumImageAngles));
			Assert.That(struc.FarmGrowthStageWithNoProduction, Is.Zero, nameof(struc.FarmGrowthStageWithNoProduction));
			Assert.That(struc.FarmNumFields, Is.Zero, nameof(struc.FarmNumFields));
			Assert.That(struc.FarmNumStagesOfGrowth, Is.Zero, nameof(struc.FarmNumStagesOfGrowth));
			Assert.That(struc.MonthlyClosureChance, Is.EqualTo(1), nameof(struc.MonthlyClosureChance));
			// Walls
			Assert.That(struc.WallTypes, Has.Count.EqualTo(0));
			// WallTypes
			Assert.That(struc.BuildingWall, Is.Null);
			Assert.That(struc.BuildingWallEntrance, Is.Null);

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(37));
		});
		LoadSaveGenericTest<IndustryObject>(objectName, assertFunc);
	}

	[TestCase("INTERDEF.DAT")]
	public void InterfaceSkinObject(string objectName)
	{
		void assertFunc(LocoObject obj, InterfaceSkinObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.MapTooltipObjectColour, Is.EqualTo(Colour.Orange), nameof(struc.MapTooltipObjectColour));
			Assert.That(struc.MapTooltipCargoColour, Is.EqualTo(Colour.DarkOrange), nameof(struc.MapTooltipCargoColour));
			Assert.That(struc.TooltipColour, Is.EqualTo(Colour.Orange), nameof(struc.TooltipColour));
			Assert.That(struc.ErrorColour, Is.EqualTo(Colour.DarkRed), nameof(struc.ErrorColour));
			Assert.That(struc.WindowPlayerColour, Is.EqualTo(Colour.Grey), nameof(struc.WindowPlayerColour));
			Assert.That(struc.WindowTitlebarColour, Is.EqualTo(Colour.Black), nameof(struc.WindowTitlebarColour));
			Assert.That(struc.WindowColour, Is.EqualTo(Colour.Grey), nameof(struc.WindowColour));
			Assert.That(struc.WindowConstructionColour, Is.EqualTo(Colour.Brown), nameof(struc.WindowConstructionColour));
			Assert.That(struc.WindowTerraFormColour, Is.EqualTo(Colour.Brown), nameof(struc.WindowTerraFormColour));
			Assert.That(struc.WindowMapColour, Is.EqualTo(Colour.MutedSeaGreen), nameof(struc.WindowMapColour));
			Assert.That(struc.WindowOptionsColour, Is.EqualTo(Colour.Blue), nameof(struc.WindowOptionsColour));
			Assert.That(struc.Colour_11, Is.EqualTo(Colour.Black), nameof(struc.Colour_11));
			Assert.That(struc.TopToolbarPrimaryColour, Is.EqualTo(Colour.Blue), nameof(struc.TopToolbarPrimaryColour));
			Assert.That(struc.TopToolbarSecondaryColour, Is.EqualTo(Colour.MutedSeaGreen), nameof(struc.TopToolbarSecondaryColour));
			Assert.That(struc.TopToolbarTertiaryColour, Is.EqualTo(Colour.Brown), nameof(struc.TopToolbarTertiaryColour));
			Assert.That(struc.TopToolbarQuaternaryColour, Is.EqualTo(Colour.Grey), nameof(struc.TopToolbarQuaternaryColour));
			Assert.That(struc.PlayerInfoToolbarColour, Is.EqualTo(Colour.Grey), nameof(struc.PlayerInfoToolbarColour));
			Assert.That(struc.TimeToolbarColour, Is.EqualTo(Colour.Grey), nameof(struc.TimeToolbarColour));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(470));
		});
		LoadSaveGenericTest<InterfaceSkinObject>(objectName, assertFunc);
	}

	[TestCase("GRASS1.DAT")]
	public void LandObject(string objectName)
	{
		void assertFunc(LocoObject obj, LandObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.CostIndex, Is.EqualTo(2), nameof(struc.CostIndex));
			Assert.That(struc.NumGrowthStages, Is.EqualTo(5), nameof(struc.NumGrowthStages));
			Assert.That(struc.NumImageAngles, Is.EqualTo(1), nameof(struc.NumImageAngles));
			Assert.That(struc.Flags, Is.EqualTo(LandObjectFlags.HasGrowthStages), nameof(struc.Flags));
			Assert.That(struc.CostFactor, Is.EqualTo(20), nameof(struc.CostFactor));
			Assert.That(struc.NumImagesPerGrowthStage, Is.Zero, nameof(struc.NumImagesPerGrowthStage));
			Assert.That(struc.DistributionPattern, Is.Zero, nameof(struc.DistributionPattern));
			Assert.That(struc.NumVariations, Is.EqualTo(3), nameof(struc.NumVariations));
			Assert.That(struc.VariationLikelihood, Is.EqualTo(10), nameof(struc.VariationLikelihood));

			Assert.That(struc.CliffEdgeHeader.Name, Is.EqualTo("LSBROWN"), nameof(struc.CliffEdgeHeader));
			Assert.That(struc.CliffEdgeHeader.DatChecksum, Is.Zero, nameof(struc.CliffEdgeHeader));
			Assert.That(struc.CliffEdgeHeader.ObjectType, Is.EqualTo(ObjectType.CliffEdge), nameof(struc.CliffEdgeHeader));
			Assert.That(struc.CliffEdgeHeader.ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.CliffEdgeHeader));

			Assert.That(struc.ReplacementLandHeader, Is.Null, nameof(struc.ReplacementLandHeader));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(417));
		});
		LoadSaveGenericTest<LandObject>(objectName, assertFunc);
	}

	[TestCase("LCROSS1.DAT")]
	public void LevelCrossingObject(string objectName)
	{
		void assertFunc(LocoObject obj, LevelCrossingObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(30), nameof(struc.BuildCostFactor));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-10), nameof(struc.SellCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));

			Assert.That(struc.AnimationSpeed, Is.EqualTo(3), nameof(struc.AnimationSpeed));
			Assert.That(struc.ClosingFrames, Is.EqualTo(4), nameof(struc.ClosingFrames));
			Assert.That(struc.ClosedFrames, Is.EqualTo(11), nameof(struc.ClosedFrames));

			Assert.That(struc.var_0A, Is.EqualTo(3), nameof(struc.var_0A));
			//Assert.That(struc.pad_0B, Is.Zero, nameof(struc.pad_0B));

			Assert.That(struc.DesignedYear, Is.EqualTo(1955), nameof(struc.DesignedYear));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(128));
		});
		LoadSaveGenericTest<LevelCrossingObject>(objectName, assertFunc);
	}

	[TestCase("REGUK.DAT")]
	public void RegionObject(string objectName)
	{
		void assertFunc(LocoObject obj, RegionObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.VehiclesDriveOnThe, Is.EqualTo(DrivingSide.Left), nameof(struc.VehiclesDriveOnThe));
			Assert.That(struc.CargoInfluenceObjects, Has.Count.EqualTo(1), nameof(struc.CargoInfluenceObjects));
			Assert.That(struc.DependentObjects, Has.Count.EqualTo(239), nameof(struc.DependentObjects));
			Assert.That(struc.CargoInfluenceTownFilter, Is.EquivalentTo(Enumerable.Repeat(CargoInfluenceTownFilterType.AllTowns, 4)), nameof(struc.CargoInfluenceTownFilter));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(1));
		});
		LoadSaveGenericTest<RegionObject>(objectName, assertFunc);
	}

	[TestCase("RDEXCAT1.DAT")]
	public void RoadExtraObject(string objectName)
	{
		void assertFunc(LocoObject obj, RoadExtraObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(4), nameof(struc.BuildCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.PaintStyle, Is.EqualTo(1), nameof(struc.PaintStyle));
			Assert.That(struc.RoadPieces, Is.EqualTo(RoadTraitFlags.SmallCurve | RoadTraitFlags.VerySmallCurve | RoadTraitFlags.Slope | RoadTraitFlags.SteepSlope | RoadTraitFlags.unk_04 | RoadTraitFlags.Turnaround | RoadTraitFlags.Junction), nameof(struc.RoadPieces));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-3), nameof(struc.SellCostFactor));
			//Assert.That(struc.BaseImageOffset, Is.Zero, nameof(struc.BaseImageOffset));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(46));
		});
		LoadSaveGenericTest<RoadExtraObject>(objectName, assertFunc);
	}

	[TestCase("ROADONE.DAT")]
	public void RoadObject(string objectName)
	{
		void assertFunc(LocoObject obj, RoadObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(22), nameof(struc.BuildCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.MaxCurveSpeed, Is.EqualTo(400), nameof(struc.MaxCurveSpeed));
			Assert.That(struc.PaintStyle, Is.Zero, nameof(struc.PaintStyle));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-20), nameof(struc.SellCostFactor));
			Assert.That(struc.TargetTownSize, Is.EqualTo(TownSize.Town), nameof(struc.TargetTownSize));
			Assert.That(struc.TunnelCostFactor, Is.EqualTo(27), nameof(struc.TunnelCostFactor));

			Assert.That(struc.Flags, Is.EqualTo(
				RoadObjectFlags.IsOneWay
				| RoadObjectFlags.unk_02
				| RoadObjectFlags.unk_03
				| RoadObjectFlags.NoWheelSlipping
				| RoadObjectFlags.IsRoad
				| RoadObjectFlags.AllowUseByAllCompanies
				| RoadObjectFlags.CanHaveStreetLights), nameof(struc.Flags));

			Assert.That(struc.RoadPieces, Is.EqualTo(
				RoadTraitFlags.SmallCurve
				| RoadTraitFlags.VerySmallCurve
				| RoadTraitFlags.Slope
				| RoadTraitFlags.SteepSlope
				| RoadTraitFlags.unk_04
				| RoadTraitFlags.Junction), nameof(struc.RoadPieces));

			Assert.That(struc.Bridges, Has.Count.EqualTo(5), nameof(struc.Bridges));
			Assert.That(struc.Bridges[0].Name, Is.EqualTo("BRDGBRCK"), nameof(struc.Bridges));
			Assert.That(struc.Bridges[0].DatChecksum, Is.Zero, nameof(struc.Bridges));
			Assert.That(struc.Bridges[0].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Bridges));
			Assert.That(struc.Bridges[0].ObjectType, Is.EqualTo(ObjectType.Bridge), nameof(struc.Bridges));
			Assert.That(struc.Bridges[1].Name, Is.EqualTo("BRDGSTAR"), nameof(struc.Bridges));
			Assert.That(struc.Bridges[1].DatChecksum, Is.Zero, nameof(struc.Bridges));
			Assert.That(struc.Bridges[1].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Bridges));
			Assert.That(struc.Bridges[1].ObjectType, Is.EqualTo(ObjectType.Bridge), nameof(struc.Bridges));
			Assert.That(struc.Bridges[2].Name, Is.EqualTo("BRDGGIRD"), nameof(struc.Bridges));
			Assert.That(struc.Bridges[2].DatChecksum, Is.Zero, nameof(struc.Bridges));
			Assert.That(struc.Bridges[2].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Bridges));
			Assert.That(struc.Bridges[2].ObjectType, Is.EqualTo(ObjectType.Bridge), nameof(struc.Bridges));
			Assert.That(struc.Bridges[3].Name, Is.EqualTo("BRDGSUSP"), nameof(struc.Bridges));
			Assert.That(struc.Bridges[3].DatChecksum, Is.Zero, nameof(struc.Bridges));
			Assert.That(struc.Bridges[3].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Bridges));
			Assert.That(struc.Bridges[3].ObjectType, Is.EqualTo(ObjectType.Bridge), nameof(struc.Bridges));
			Assert.That(struc.Bridges[4].Name, Is.EqualTo("BRDGWOOD"), nameof(struc.Bridges));
			Assert.That(struc.Bridges[4].DatChecksum, Is.Zero, nameof(struc.Bridges));
			Assert.That(struc.Bridges[4].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Bridges));
			Assert.That(struc.Bridges[4].ObjectType, Is.EqualTo(ObjectType.Bridge), nameof(struc.Bridges));

			Assert.That(struc.TracksAndRoads, Has.Count.EqualTo(1), nameof(struc.TracksAndRoads));
			Assert.That(struc.TracksAndRoads[0].Name, Is.EqualTo("ROADTRAM"), nameof(struc.TracksAndRoads));
			Assert.That(struc.TracksAndRoads[0].DatChecksum, Is.Zero, nameof(struc.TracksAndRoads));
			Assert.That(struc.TracksAndRoads[0].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.TracksAndRoads));
			Assert.That(struc.TracksAndRoads[0].ObjectType, Is.EqualTo(ObjectType.Road), nameof(struc.TracksAndRoads));

			Assert.That(struc.RoadMods, Is.Empty, nameof(struc.RoadMods));

			Assert.That(struc.Stations, Has.Count.EqualTo(1), nameof(struc.Stations));
			Assert.That(struc.Stations[0].Name, Is.EqualTo("BUSSTOP"), nameof(struc.Stations));
			Assert.That(struc.Stations[0].DatChecksum, Is.Zero, nameof(struc.Stations));
			Assert.That(struc.Stations[0].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Stations));
			Assert.That(struc.Stations[0].ObjectType, Is.EqualTo(ObjectType.RoadStation), nameof(struc.Stations));

			Assert.That(struc.Tunnel.Name, Is.EqualTo("TUNNEL2"), nameof(struc.Tunnel));
			Assert.That(struc.Tunnel.DatChecksum, Is.Zero, nameof(struc.Tunnel.DatChecksum));
			Assert.That(struc.Tunnel.ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.Tunnel.ObjectSource));
			Assert.That(struc.Tunnel.ObjectType, Is.EqualTo(ObjectType.Tunnel), nameof(struc.Tunnel.ObjectType));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(73));
		});
		LoadSaveGenericTest<RoadObject>(objectName, assertFunc);
	}

	[TestCase("RDSTAT1.DAT")]
	public void RoadStationObject(string objectName)
	{
		void assertFunc(LocoObject obj, RoadStationObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(24), nameof(struc.BuildCostFactor));
			// Cargo
			//Assert.That(struc.CargoOffsetBytes, Is.All.EqualTo(0), nameof(struc.CargoOffsetBytes));
			// Compatible
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.Flags, Is.EqualTo(RoadStationObjectFlags.Passenger | RoadStationObjectFlags.RoadEnd), nameof(struc.Flags));
			Assert.That(struc.CompatibleRoadObjects, Is.Empty, nameof(struc.CompatibleRoadObjects));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(1945), nameof(struc.ObsoleteYear));
			Assert.That(struc.PaintStyle, Is.Zero, nameof(struc.PaintStyle));
			Assert.That(struc.RoadPieces, Is.EqualTo(RoadTraitFlags.None), nameof(struc.RoadPieces));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-17), nameof(struc.SellCostFactor));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(18));
		});
		LoadSaveGenericTest<RoadStationObject>(objectName, assertFunc);
	}

	[TestCase("SCAFDEF.DAT")]
	public void ScaffoldingObject(string objectName)
	{
		void assertFunc(LocoObject obj, ScaffoldingObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.SegmentHeights[0], Is.EqualTo(16), nameof(struc.SegmentHeights) + "[0]");
			Assert.That(struc.SegmentHeights[1], Is.EqualTo(16), nameof(struc.SegmentHeights) + "[1]");
			Assert.That(struc.SegmentHeights[2], Is.EqualTo(32), nameof(struc.SegmentHeights) + "[2]");

			Assert.That(struc.RoofHeights[0], Is.Zero, nameof(struc.RoofHeights) + "[0]");
			Assert.That(struc.RoofHeights[1], Is.Zero, nameof(struc.RoofHeights) + "[1]");
			Assert.That(struc.RoofHeights[2], Is.EqualTo(14), nameof(struc.RoofHeights) + "[2]");

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(36));
		});
		LoadSaveGenericTest<ScaffoldingObject>(objectName, assertFunc);
	}

	[TestCase("STEX000.DAT")]
	public void ScenarioTextObject(string objectName)
	{
		void assertFunc(LocoObject obj, ScenarioTextObject struc)
			=> Assert.Multiple(() => Assert.That(obj.ImageTable, Is.Null));
		LoadSaveGenericTest<ScenarioTextObject>(objectName, assertFunc);
	}

	[TestCase("SNOW.DAT")]
	public void SnowObject(string objectName)
	{
		void assertFunc(LocoObject obj, SnowObject struc) => Assert.Multiple(() =>
		{
			Assert.That(obj.StringTable.Table, Has.Count.EqualTo(1), nameof(obj.StringTable.Table));
			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(139), nameof(obj.ImageTable.GraphicsElements));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(139));
		});
		LoadSaveGenericTest<SnowObject>(objectName, assertFunc);
	}

	[TestCase("SNDA1.DAT")]
	public void SoundObject(string objectName)
	{
		void assertFunc(LocoObject obj, SoundObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.ShouldLoop, Is.EqualTo(1), nameof(struc.ShouldLoop));
			Assert.That(struc.Volume, Is.Zero, nameof(struc.Volume));

			Assert.That(struc.PcmData, Has.Length.EqualTo(119666), nameof(struc.PcmData.Length));

			Assert.That(struc.SoundObjectData.Length, Is.EqualTo(119662), nameof(struc.SoundObjectData.Length));
			Assert.That(struc.SoundObjectData.Offset, Is.EqualTo(8), nameof(struc.SoundObjectData.Offset));
			Assert.That(struc.SoundObjectData.var_00, Is.EqualTo(1), nameof(struc.SoundObjectData.var_00));

			Assert.That(struc.SoundObjectData.PcmHeader.AverageBytesPerSecond, Is.EqualTo(44100), nameof(struc.SoundObjectData.PcmHeader.AverageBytesPerSecond));
			Assert.That(struc.SoundObjectData.PcmHeader.BitsPerSample, Is.EqualTo(16), nameof(struc.SoundObjectData.PcmHeader.BitsPerSample));
			Assert.That(struc.SoundObjectData.PcmHeader.BlockAlign, Is.EqualTo(2), nameof(struc.SoundObjectData.PcmHeader.BlockAlign));
			Assert.That(struc.SoundObjectData.PcmHeader.ExtraSize, Is.Zero, nameof(struc.SoundObjectData.PcmHeader.ExtraSize));
			Assert.That(struc.SoundObjectData.PcmHeader.Channels, Is.EqualTo(1), nameof(struc.SoundObjectData.PcmHeader.Channels));
			Assert.That(struc.SoundObjectData.PcmHeader.SampleRate, Is.EqualTo(22050), nameof(struc.SoundObjectData.PcmHeader.SampleRate));
			Assert.That(struc.SoundObjectData.PcmHeader.WaveFormatTag, Is.EqualTo(1), nameof(struc.SoundObjectData.PcmHeader.WaveFormatTag));

			Assert.That(obj.ImageTable, Is.Null);
		});
		LoadSaveGenericTest<SoundObject>(objectName, assertFunc);
	}

	[TestCase("STEAM.DAT")]
	public void SteamObject(string objectName)
	{
		void assertFunc(LocoObject obj, SteamObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.Flags, Is.EqualTo(SteamObjectFlags.ApplyWind | SteamObjectFlags.DisperseOnCollision | SteamObjectFlags.HasTunnelSounds), nameof(struc.Flags));
			// FrameInfoType0 contents
			// FrameInfoType1 contents
			//Assert.That(struc.NumImages, Is.EqualTo(57), nameof(struc.NumImages));
			Assert.That(struc.SoundEffects, Has.Count.EqualTo(8), nameof(struc.SoundEffects));
			Assert.That(struc.NumStationaryTicks, Is.EqualTo(2), nameof(struc.NumStationaryTicks));

			// these aren't currently calculated in this tool
			Assert.That(struc.SpriteWidth, Is.Zero, nameof(struc.SpriteWidth));
			Assert.That(struc.SpriteHeightNegative, Is.Zero, nameof(struc.SpriteHeightNegative));
			Assert.That(struc.SpriteHeightPositive, Is.Zero, nameof(struc.SpriteHeightPositive));

			Assert.That(struc.FrameInfoType0, Has.Count.EqualTo(47), nameof(struc.FrameInfoType0));
			Assert.That(struc.FrameInfoType1, Has.Count.EqualTo(30), nameof(struc.FrameInfoType1));
			Assert.That(struc.var_0A, Is.Zero, nameof(struc.var_0A));
			// SoundEffects

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(57));
		});
		LoadSaveGenericTest<SteamObject>(objectName, assertFunc);
	}

	[TestCase("SLIGHT1.DAT")]
	public void StreetLightObject(string objectName)
	{
		void assertFunc(LocoObject obj, StreetLightObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.DesignedYears[0], Is.EqualTo(1900), nameof(struc.DesignedYears) + "[0]");
			Assert.That(struc.DesignedYears[1], Is.EqualTo(1950), nameof(struc.DesignedYears) + "[1]");
			Assert.That(struc.DesignedYears[2], Is.EqualTo(1985), nameof(struc.DesignedYears) + "[2]");

			Assert.That(obj.StringTable["Name"][LanguageId.English_UK], Is.EqualTo("Street Lights"));
			Assert.That(obj.StringTable["Name"][LanguageId.English_US], Is.EqualTo("Street Lights"));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(12));
		});
		LoadSaveGenericTest<StreetLightObject>(objectName, assertFunc);
	}

	[TestCase("ATOWNNAM.DAT")]
	public void TownNamesObject(string objectName)
	{
		void assertFunc(LocoObject obj, TownNamesObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.Categories[0].Count, Is.EqualTo(2), nameof(struc.Categories) + "[0] Count");
			Assert.That(struc.Categories[0].Bias, Is.EqualTo(30), nameof(struc.Categories) + "[0] Bias");
			Assert.That(struc.Categories[0].Offset, Is.EqualTo(93), nameof(struc.Categories) + "[0] Offset");

			Assert.That(struc.Categories[1].Count, Is.EqualTo(94), nameof(struc.Categories) + "[1] Count");
			Assert.That(struc.Categories[1].Bias, Is.Zero, nameof(struc.Categories) + "[1] Bias");
			Assert.That(struc.Categories[1].Offset, Is.EqualTo(110), nameof(struc.Categories) + "[1] Offset");

			Assert.That(struc.Categories[2].Count, Is.Zero, nameof(struc.Categories) + "[2] Count");
			Assert.That(struc.Categories[2].Bias, Is.Zero, nameof(struc.Categories) + "[2] Bias");
			Assert.That(struc.Categories[2].Offset, Is.Zero, nameof(struc.Categories) + "[2] Offset");

			Assert.That(struc.Categories[3].Count, Is.Zero, nameof(struc.Categories) + "[3] Count");
			Assert.That(struc.Categories[3].Bias, Is.Zero, nameof(struc.Categories) + "[3] Bias");
			Assert.That(struc.Categories[3].Offset, Is.Zero, nameof(struc.Categories) + "[3] Offset");

			Assert.That(struc.Categories[4].Count, Is.EqualTo(18), nameof(struc.Categories) + "[4] Count");
			Assert.That(struc.Categories[4].Bias, Is.Zero, nameof(struc.Categories) + "[4] Bias");
			Assert.That(struc.Categories[4].Offset, Is.EqualTo(923), nameof(struc.Categories) + "[4] Offset");

			Assert.That(struc.Categories[5].Count, Is.EqualTo(6), nameof(struc.Categories) + "[5] Count");
			Assert.That(struc.Categories[5].Bias, Is.EqualTo(20), nameof(struc.Categories) + "[5] Bias");
			Assert.That(struc.Categories[5].Offset, Is.EqualTo(1071), nameof(struc.Categories) + "[5] Offset");

			Assert.That(obj.StringTable["Name"][LanguageId.English_UK], Is.EqualTo("North-American style town names"));
			Assert.That(obj.StringTable["Name"][LanguageId.English_US], Is.EqualTo("North-American style town names"));

			Assert.That(obj.ImageTable, Is.Null);
		});
		LoadSaveGenericTest<TownNamesObject>(objectName, assertFunc);
	}

	[TestCase("TREXCAT1.DAT")]
	public void TrackExtraObject(string objectName)
	{
		void assertFunc(LocoObject obj, TrackExtraObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.TrackPieces, Is.EqualTo(TrackTraitFlags.Diagonal | TrackTraitFlags.LargeCurve | TrackTraitFlags.NormalCurve | TrackTraitFlags.SmallCurve | TrackTraitFlags.VerySmallCurve | TrackTraitFlags.Slope | TrackTraitFlags.SteepSlope | TrackTraitFlags.OneSided | TrackTraitFlags.SlopedCurve | TrackTraitFlags.SBend), nameof(struc.TrackPieces));
			Assert.That(struc.PaintStyle, Is.EqualTo(1), nameof(struc.PaintStyle));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.BuildCostFactor, Is.EqualTo(2), nameof(struc.BuildCostFactor));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-1), nameof(struc.SellCostFactor));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(134));
		});
		LoadSaveGenericTest<TrackExtraObject>(objectName, assertFunc);
	}

	[TestCase("TRACKST.DAT")]
	public void TrackObject(string objectName)
	{
		void assertFunc(LocoObject obj, TrackObject struc) => Assert.Multiple(() =>
		{
			// Bridges
			Assert.That(struc.BuildCostFactor, Is.EqualTo(11), nameof(struc.BuildCostFactor));
			// Compatible
			//Assert.That(struc._CompatibleRoads, Is.Zero, nameof(struc._CompatibleRoads));
			//Assert.That(struc._CompatibleTracks, Is.Zero, nameof(struc._CompatibleTracks));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.MaxCurveSpeed, Is.EqualTo(400), nameof(struc.MaxCurveSpeed));
			Assert.That(struc.VehicleDisplayListVerticalOffset, Is.EqualTo(3), nameof(struc.VehicleDisplayListVerticalOffset));
			Assert.That(struc.Flags, Is.EqualTo(TrackObjectFlags.unk_00), nameof(struc.Flags));
			// Mods
			Assert.That(struc.Bridges, Has.Count.EqualTo(5), nameof(struc.Bridges));
			Assert.That(struc.TracksAndRoads, Has.Count.EqualTo(7), nameof(struc.TracksAndRoads));
			Assert.That(struc.TrackMods, Has.Count.EqualTo(2), nameof(struc.TrackMods));
			Assert.That(struc.Signals, Has.Count.EqualTo(10), nameof(struc.Signals));
			Assert.That(struc.Stations, Has.Count.EqualTo(5), nameof(struc.Stations));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-10), nameof(struc.SellCostFactor));
			// Signals
			// Stations
			Assert.That(struc.StationTrackPieces, Is.EqualTo(TrackTraitFlags.Diagonal | TrackTraitFlags.NormalCurve | TrackTraitFlags.SmallCurve | TrackTraitFlags.SBend), nameof(struc.StationTrackPieces));
			Assert.That(struc.TrackPieces, Is.EqualTo(TrackTraitFlags.Diagonal | TrackTraitFlags.LargeCurve | TrackTraitFlags.NormalCurve | TrackTraitFlags.SmallCurve | TrackTraitFlags.Slope | TrackTraitFlags.SlopedCurve | TrackTraitFlags.SBend | TrackTraitFlags.Junction), nameof(struc.TrackPieces));
			Assert.That(struc.TunnelCostFactor, Is.EqualTo(24), nameof(struc.TunnelCostFactor));
			Assert.That(struc.var_06, Is.Zero, nameof(struc.var_06));

			Assert.That(obj.StringTable.Table, Has.Count.EqualTo(1), nameof(obj.StringTable.Table));
			Assert.That(obj.ImageTable.GraphicsElements, Is.Not.Null);
			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(400), nameof(obj.ImageTable.GraphicsElements));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(400));
		});
		LoadSaveGenericTest<TrackObject>(objectName, assertFunc);
	}

	[TestCase("SIGSUS.DAT")]
	public void TrackSignalObject(string objectName)
	{
		void assertFunc(LocoObject obj, TrackSignalObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.AnimationSpeed, Is.EqualTo(1), nameof(struc.AnimationSpeed));
			Assert.That(struc.BuildCostFactor, Is.EqualTo(4), nameof(struc.BuildCostFactor));
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.DesignedYear, Is.Zero, nameof(struc.DesignedYear));
			Assert.That(struc.Flags, Is.EqualTo(TrackSignalObjectFlags.IsLeft), nameof(struc.Flags));
			// Mods
			Assert.That(struc.CompatibleTrackObjects, Is.Empty, nameof(struc.CompatibleTrackObjects));
			Assert.That(struc.NumFrames, Is.EqualTo(7), nameof(struc.NumFrames));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(1955), nameof(struc.ObsoleteYear));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-3), nameof(struc.SellCostFactor));
			Assert.That(struc.var_0B, Is.Zero, nameof(struc.var_0B));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(56));
		});
		LoadSaveGenericTest<TrackSignalObject>(objectName, assertFunc);
	}

	[TestCase("TRSTAT1.DAT")]
	public void TrackStationObject(string objectName)
	{
		void assertFunc(LocoObject obj, TrackStationObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.BuildCostFactor, Is.EqualTo(7), nameof(struc.BuildCostFactor));
			// CargoOffsetBytes
			// Compatible
			Assert.That(struc.CostIndex, Is.EqualTo(1), nameof(struc.CostIndex));
			Assert.That(struc.DesignedYear, Is.EqualTo(1960), nameof(struc.DesignedYear));
			Assert.That(struc.PaintStyle, Is.Zero, nameof(struc.PaintStyle));
			Assert.That(struc.Flags, Is.EqualTo(TrackStationObjectFlags.None), nameof(struc.Flags));
			// ManualPower
			Assert.That(struc.Height, Is.Zero, nameof(struc.Height));
			Assert.That(struc.CompatibleTrackObjects, Is.Empty, nameof(struc.CompatibleTrackObjects));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(65535), nameof(struc.ObsoleteYear));
			Assert.That(struc.SellCostFactor, Is.EqualTo(-7), nameof(struc.SellCostFactor));
			Assert.That(struc.TrackPieces, Is.EqualTo(TrackTraitFlags.None), nameof(struc.TrackPieces));
			Assert.That(struc.var_0B, Is.EqualTo(2), nameof(struc.var_0B));
			Assert.That(struc.var_0D, Is.Zero, nameof(struc.var_0D));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(36));
		});
		LoadSaveGenericTest<TrackStationObject>(objectName, assertFunc);
	}

	[TestCase("BEECH.DAT")]
	public void TreeObject(string objectName)
	{
		void assertFunc(LocoObject obj, TreeObject struc) => Assert.Multiple(() =>
		{
			//Assert.That(struc.var_02, Is.EqualTo(40), nameof(struc.var_02));
			Assert.That(struc.Height, Is.EqualTo(131), nameof(struc.Height));
			Assert.That(struc.var_04, Is.EqualTo(27), nameof(struc.var_04));
			Assert.That(struc.var_05, Is.EqualTo(83), nameof(struc.var_05));
			Assert.That(struc.NumRotations, Is.EqualTo(1), nameof(struc.NumRotations));
			Assert.That(struc.NumGrowthStages, Is.EqualTo(4), nameof(struc.NumGrowthStages));
			Assert.That(struc.Flags, Is.EqualTo(TreeObjectFlags.LowAltitude | TreeObjectFlags.RequiresWater | TreeObjectFlags.HasShadow), nameof(struc.Flags));
			//Assert.That(struc.Sprites, Is.EquivalentTo(Array.CreateInstance(typeof(byte), 6)), nameof(struc.Sprites));
			//Assert.That(struc.SnowSprites, Is.EquivalentTo(Array.CreateInstance(typeof(byte), 6)), nameof(struc.SnowSprites));
			Assert.That(struc.ShadowImageOffset, Is.Zero, nameof(struc.ShadowImageOffset));
			Assert.That(struc.SeasonalVariants, Is.EqualTo(TreeObjectSeasonalVariantFlags.Variant0 | TreeObjectSeasonalVariantFlags.Variant1 | TreeObjectSeasonalVariantFlags.Variant2 | TreeObjectSeasonalVariantFlags.Variant3), nameof(struc.SeasonalVariants));
			Assert.That(struc.SeasonState, Is.EqualTo(3), nameof(struc.SeasonState));
			Assert.That(struc.CurrentSeason, Is.EqualTo(2), nameof(struc.CurrentSeason));
			Assert.That(struc.CostIndex, Is.EqualTo(3), nameof(struc.CostIndex));
			Assert.That(struc.BuildCostFactor, Is.EqualTo(8), nameof(struc.BuildCostFactor));
			Assert.That(struc.ClearCostFactor, Is.EqualTo(4), nameof(struc.ClearCostFactor));
			Assert.That(struc.Colours, Is.Zero, nameof(struc.Colours));
			Assert.That(struc.Rating, Is.EqualTo(10), nameof(struc.Rating));
			Assert.That(struc.DemolishRatingReduction, Is.EqualTo(-15), nameof(struc.DemolishRatingReduction));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(32));
		});
		LoadSaveGenericTest<TreeObject>(objectName, assertFunc);
	}

	[TestCase("TUNNEL1.DAT")]
	public void TunnelObject(string objectName)
	{
		void assertFunc(LocoObject obj, TunnelObject struc) => Assert.Multiple(() => Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(4)));
		LoadSaveGenericTest<TunnelObject>(objectName, assertFunc);
	}

	[TestCase("707.DAT")]
	public void VehicleAircraftObject(string objectName)
	{
		void assertFunc(LocoObject obj, VehicleObject struc) => Assert.Multiple(() =>
		{
			//var s5header = obj.S5Header;
			//Assert.That(s5header.Flags, Is.EqualTo(283680407), nameof(s5header.Flags));
			//Assert.That(s5header.Name, Is.EqualTo("707     "), nameof(s5header.Name));
			//Assert.That(s5header.Checksum, Is.EqualTo(1331114877), nameof(s5header.Checksum));
			//Assert.That(s5header.ObjectType, Is.EqualTo(ObjectType.Vehicle), nameof(s5header.ObjectType));

			//var objHeader = obj.ObjectHeader;
			//Assert.That(objHeader.Encoding, Is.EqualTo(SawyerEncoding.RunLengthSingle), nameof(objHeader.Encoding));
			//Assert.That(objHeader.DataLength, Is.EqualTo(159566), nameof(objHeader.DataLength));

			Assert.That(struc.Mode, Is.EqualTo(TransportMode.Air), nameof(struc.Mode));
			Assert.That(struc.Type, Is.EqualTo(VehicleType.Aircraft), nameof(struc.Type));
			Assert.That(struc.NumCarComponents, Is.EqualTo(1), nameof(struc.NumCarComponents));
			// Assert.That(struc.TrackType, Is.EqualTo(0xFF), nameof(struc.TrackType)); // is changed after load from 0 to 255
			Assert.That(struc.RequiredTrackExtras.Count, Is.Zero, nameof(struc.RequiredTrackExtras));
			Assert.That(struc.CostIndex, Is.EqualTo(8), nameof(struc.CostIndex));
			Assert.That(struc.CostFactor, Is.EqualTo(345), nameof(struc.CostFactor));
			Assert.That(struc.Reliability, Is.EqualTo(88), nameof(struc.Reliability));
			Assert.That(struc.RunCostIndex, Is.EqualTo(4), nameof(struc.RunCostIndex));
			Assert.That(struc.RunCostFactor, Is.EqualTo(55), nameof(struc.RunCostFactor));
			Assert.That(struc.CompanyColourSchemeIndex, Is.EqualTo(CompanyColourType.Airplane), nameof(struc.CompanyColourSchemeIndex));
			Assert.That(struc.CompatibleVehicles.Count, Is.Zero, nameof(struc.CompatibleVehicles));
			//CollectionAssert.AreEqual(Enumerable.Repeat(0, 8).ToArray(), struc.CompatibleVehicles, nameof(struc.CompatibleVehicles));
			//CollectionAssert.AreEqual(Enumerable.Repeat(0, 4).ToArray(), struc.RequiredTrackExtras, nameof(struc.RequiredTrackExtras));
			//Assert.That(struc.var_24, Is.EqualTo(0), nameof(struc.var_24));
			//Assert.That(struc.BodySprites, Is.EqualTo(0), nameof(struc.BodySprites));
			//Assert.That(struc.BogieSprites, Is.EqualTo(1), nameof(struc.BogieSprites));
			Assert.That(struc.Power, Is.EqualTo(3000), nameof(struc.Power));
			Assert.That(struc.Speed, Is.EqualTo(604), nameof(struc.Speed));
			Assert.That(struc.RackSpeed, Is.EqualTo(120), nameof(struc.RackSpeed));
			Assert.That(struc.Weight, Is.EqualTo(141), nameof(struc.Weight));
			Assert.That(struc.Flags, Is.EqualTo(VehicleObjectFlags.Refittable), nameof(struc.Flags));
			// CollectionAssert.AreEqual(struc.MaxCargo, Enumerable.Repeat(0, 2).ToArray(), nameof(struc.MaxCargo)); // this is changed after load from 0 to 24
			//CollectionAssert.AreEqual(Enumerable.Repeat(0, 2).ToArray(), struc.CompatibleCargoCategories, nameof(struc.CompatibleCargoCategories));
			//CollectionAssert.AreEqual(Enumerable.Repeat(0, 32).ToArray(), struc.CargoTypeSpriteOffsets, nameof(struc.CargoTypeSpriteOffsets));
			Assert.That(struc.NumSimultaneousCargoTypes, Is.EqualTo(1), nameof(struc.NumSimultaneousCargoTypes));
			Assert.That(struc.ParticleEmitters[0].AnimationObject, Is.Null, nameof(struc.ParticleEmitters));
			Assert.That(struc.ParticleEmitters[0].EmitterVerticalPos, Is.EqualTo(24), nameof(struc.ParticleEmitters));
			Assert.That(struc.ParticleEmitters[0].Type, Is.EqualTo(SimpleAnimationType.None), nameof(struc.ParticleEmitters));
			Assert.That(struc.ParticleEmitters[1].AnimationObject, Is.Null, nameof(struc.ParticleEmitters));
			Assert.That(struc.ParticleEmitters[1].EmitterVerticalPos, Is.Zero, nameof(struc.ParticleEmitters));
			Assert.That(struc.ParticleEmitters[1].Type, Is.EqualTo(SimpleAnimationType.None), nameof(struc.ParticleEmitters));
			Assert.That(struc.ShipWakeSpacing, Is.Zero, nameof(struc.ShipWakeSpacing));
			Assert.That(struc.DesignedYear, Is.EqualTo(1957), nameof(struc.DesignedYear));
			Assert.That(struc.ObsoleteYear, Is.EqualTo(1987), nameof(struc.ObsoleteYear));
			//Assert.That(struc.RackRailType, Is.Zero, nameof(struc.RackRailType));
			//Assert.That(struc.DrivingSoundType, Is.EqualTo(DrivingSoundType.Engine1), nameof(struc.DrivingSoundType));
			//Assert.That(struc.Sound, Is.EqualTo(0), nameof(struc.Sound));
			//Assert.That(struc.var_135, Is.EqualTo(0), nameof(struc.var_135));
			Assert.That(struc.StartSounds.Count, Is.EqualTo(2), nameof(struc.StartSounds));

			Assert.That(struc.StartSounds[0].Name, Is.EqualTo("SNDTD1"), nameof(struc.StartSounds) + "[0]Name");
			Assert.That(struc.StartSounds[0].DatChecksum, Is.Zero, nameof(struc.StartSounds) + "[0]Checksum");
			//Assert.That(struc.StartSounds[0].Flags, Is.EqualTo(1), nameof(struc.StartSounds) + "[0]Flags");
			Assert.That(struc.StartSounds[0].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.StartSounds) + "[0]Checksum");
			Assert.That(struc.StartSounds[0].ObjectType, Is.EqualTo(ObjectType.Sound), nameof(struc.StartSounds) + "[0]Flags");

			Assert.That(struc.StartSounds[1].Name, Is.EqualTo("SNDTD2"), nameof(struc.StartSounds) + "[1]Name");
			Assert.That(struc.StartSounds[1].DatChecksum, Is.Zero, nameof(struc.StartSounds) + "[1]Checksum");
			//Assert.That(struc.StartSounds[1].Flags, Is.EqualTo(1), nameof(struc.StartSounds) + "[1]Flags");
			Assert.That(struc.StartSounds[1].ObjectSource, Is.EqualTo(ObjectSource.Custom), nameof(struc.StartSounds) + "[1]Checksum");
			Assert.That(struc.StartSounds[1].ObjectType, Is.EqualTo(ObjectType.Sound), nameof(struc.StartSounds) + "[1]Flags");

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(168));
		});
		LoadSaveGenericTest<VehicleObject>(objectName, assertFunc);
	}

	[TestCase("FENCE1.DAT")]
	public void WallObject(string objectName)
	{
		void assertFunc(LocoObject obj, WallObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.ToolId, Is.EqualTo(15), nameof(struc.ToolId));
			Assert.That(struc.Flags1, Is.EqualTo(WallObjectFlags1.None), nameof(struc.Flags1));
			Assert.That(struc.Height, Is.EqualTo(2), nameof(struc.Height));
			Assert.That(struc.Flags2, Is.EqualTo(WallObjectFlags2.Opaque), nameof(struc.Flags2));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(6));
		});
		LoadSaveGenericTest<WallObject>(objectName, assertFunc);
	}

	[TestCase("WATER1.DAT")]
	public void WaterObject(string objectName)
	{
		void assertFunc(LocoObject obj, WaterObject struc) => Assert.Multiple(() =>
		{
			Assert.That(struc.CostIndex, Is.EqualTo(2), nameof(struc.CostIndex));
			Assert.That(struc.var_03, Is.Zero, nameof(struc.var_03));
			Assert.That(struc.CostFactor, Is.EqualTo(51), nameof(struc.CostFactor));
			//Assert.That(struc.var_0A, Is.EqualTo(0), nameof(struc.var_0A));

			Assert.That(obj.ImageTable.GraphicsElements, Has.Count.EqualTo(76));
		});
		LoadSaveGenericTest<WaterObject>(objectName, assertFunc);
	}
}
