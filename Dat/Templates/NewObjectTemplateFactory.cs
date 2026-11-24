using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.Climate;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Currency;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Scaffolding;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Tree;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;

namespace Dat.Templates;

/// <summary>The in-memory DAT header and object produced by <see cref="NewObjectTemplateFactory"/>.</summary>
public sealed record NewObjectTemplate(DatHeaderInfo DatInfo, LocoObject LocoObject);

/// <summary>
/// Builds a brand-new, empty-but-sane object (a "template") for a given object type, so the editor can
/// offer a "New Object" starting point instead of only being able to open existing files.
/// <para>
/// The defaults here are intentionally conservative: costs are positive, negative sell factors are kept
/// within their build cost, fixed-length lists have the number of entries the DAT format requires, and
/// the required string-table keys are always created. They are a starting point for the user to edit,
/// not a finished object - the object still needs images and (usually) fine tuning before it is useful.
/// </para>
/// </summary>
public static class NewObjectTemplateFactory
{
	const int DefaultBuildCost = 100;
	const int DefaultSellCost = -50;

	/// <summary>Every object type the editor knows how to create a template for, in enum (DAT) order.</summary>
	public static IReadOnlyList<ObjectType> SupportedTypes { get; } = Enum.GetValues<ObjectType>();

	/// <summary>Creates an in-memory DAT (headers + object) for <paramref name="objectType"/>.</summary>
	/// <param name="name">The 8-character internal object name written into the S5 header.</param>
	/// <param name="objectType">Which object type to build.</param>
	/// <param name="objectSource">The declared source; custom is the only source a user can legitimately create.</param>
	/// <param name="encoding">The Sawyer encoding recorded in the object header (used when the object is first saved).</param>
	public static NewObjectTemplate Create(string name, ObjectType objectType, ObjectSource objectSource, SawyerEncoding encoding)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		var model = CreateModel(objectType);
		var stringTable = CreateStringTable(objectType, name);

		// A single, empty "Images" group lets the user append sprites through the image table UI.
		var imageTable = new ImageTable { Groups = [new ImageTableGroup("Images", [])] };

		var locoObject = new LocoObject(objectType, model, stringTable, imageTable);

		// Checksum and data length are computed for real when the object is saved, so 0 is fine here.
		var s5Header = new S5Header(name, 0)
		{
			ObjectSource = objectSource.Convert(),
			ObjectType = (DatObjectType)(byte)objectType,
		};

		return new NewObjectTemplate(new DatHeaderInfo(s5Header, new ObjectHeader(encoding, 0)), locoObject);
	}

	/// <summary>Creates a default <see cref="ILocoStruct"/> for the given object type.</summary>
	public static ILocoStruct CreateModel(ObjectType objectType)
		=> objectType switch
		{
			ObjectType.InterfaceSkin => new Definitions.ObjectModels.Objects.InterfaceSkin.InterfaceSkinObject(),
			ObjectType.Sound => new SoundObject { SoundObjectData = new() },
			ObjectType.Currency => new CurrencyObject(),
			ObjectType.Steam => new SteamObject { SoundEffects = CreatePlaceholderHeaders(9) },
			ObjectType.CliffEdge => new Definitions.ObjectModels.Objects.CliffEdge.CliffEdgeObject(),
			ObjectType.Water => new WaterObject { CostFactor = 1 },
			ObjectType.Land => new LandObject
			{
				CostIndex = 0,
				CostFactor = 1,
				NumGrowthStages = 1,
				NumImageAngles = 1,
				CliffEdgeHeader = new ObjectModelHeader(),
			},
			ObjectType.TownNames => new Definitions.ObjectModels.Objects.TownNames.TownNamesObject
			{
				MorphemeCategories = [.. Enumerable.Range(0, 6).Select(_ => new MorphemeCategory())],
			},
			ObjectType.Cargo => new CargoObject { UnitWeight = 1, CargoTransferTime = 1 },
			ObjectType.Wall => new Definitions.ObjectModels.Objects.Wall.WallObject(),
			ObjectType.TrackSignal => new TrackSignalObject
			{
				AnimationSpeed = 0,
				NumFrames = 4,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.LevelCrossing => new LevelCrossingObject
			{
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
				ClosedAnimationFrameCount = 1,
			},
			ObjectType.StreetLight => new StreetLightObject { DesignedYears = [1900, 1950, 2000] },
			ObjectType.Tunnel => new Definitions.ObjectModels.Objects.Tunnel.TunnelObject(),
			ObjectType.Bridge => new BridgeObject
			{
				DeckDepth = 16,
				SpanLength = 1,
				BaseCostFactor = DefaultBuildCost,
				HeightCostFactor = 0,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.TrackStation => new TrackStationObject
			{
				PaintStyle = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.TrackExtra => new TrackExtraObject
			{
				PaintStyle = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.Track => new TrackObject
			{
				var_06 = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				TunnelCostFactor = 1,
				CostIndex = 0,
			},
			ObjectType.RoadStation => new RoadStationObject
			{
				PaintStyle = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.RoadExtra => new RoadExtraObject
			{
				PaintStyle = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.Road => new RoadObject
			{
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				TunnelCostFactor = 1,
				CostIndex = 0,
			},
			ObjectType.Airport => new AirportObject
			{
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.Dock => new DockObject
			{
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				CostIndex = 0,
			},
			ObjectType.Vehicle => new VehicleObject
			{
				CostIndex = 0,
				CostFactor = DefaultBuildCost,
				Reliability = 100,
				RunCostIndex = 0,
				RunCostFactor = 0,
				MaxCargo = [0, 0],
				CompatibleCargoCategories = [[], []],
			},
			ObjectType.Tree => new TreeObject
			{
				InitialHeight = 0,
				Height = 1,
				MinHeight = 0,
				MaxHeight = 1,
				NumRotations = 1,
				NumGrowthStages = 1,
				CostIndex = 0,
				BuildCostFactor = DefaultBuildCost,
				ClearCostFactor = DefaultSellCost,
			},
			ObjectType.Snow => new Definitions.ObjectModels.Objects.Snow.SnowObject(),
			ObjectType.Climate => new ClimateObject
			{
				FirstSeason = Season.Autumn,
				WinterSnowLine = 0,
				SummerSnowLine = 0,
			},
			ObjectType.HillShapes => new Definitions.ObjectModels.Objects.HillShape.HillShapesObject(),
			ObjectType.Building => new BuildingObject
			{
				BuildingComponents = CreateBuildingComponents(),
				CostIndex = 0,
				ProducedQuantity = [0, 0],
				ProducedCargoQuantity = [0, 0],
				ConsumedCargoQuantity = [0, 0],
			},
			ObjectType.Scaffolding => new ScaffoldingObject
			{
				SegmentHeights = [0, 0, 0],
				RoofHeights = [0, 0, 0],
			},
			ObjectType.Industry => new IndustryObject
			{
				BuildingComponents = CreateBuildingComponents(),
				AnimationSequences = [[], [], [], []],
				InitialProductionRate = [new() { Min = 1, Max = 2 }, new() { Min = 1, Max = 2 }],
				ProducedCargo = CreatePlaceholderHeaders(2),
				RequiredCargo = CreatePlaceholderHeaders(3),
				MinNumBuildings = 1,
				MaxNumBuildings = 1,
				TotalOfTypeInScenario = 1,
				CostIndex = 0,
				BuildCostFactor = DefaultBuildCost,
				SellCostFactor = DefaultSellCost,
				FarmTileNumImageAngles = 1,
				FarmGrowthStageWithNoProduction = 0xFF,
			},
			ObjectType.Region => new Definitions.ObjectModels.Objects.Region.RegionObject(),
			ObjectType.Competitor => new CompetitorObject
			{
				Emotions = EmotionFlags.Neutral,
				Intelligence = 5,
				Aggressiveness = 5,
				Competitiveness = 5,
			},
			ObjectType.ScenarioText => new Definitions.ObjectModels.Objects.ScenarioText.ScenarioTextObject(),
			_ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, "Unsupported object type"),
		};

	/// <summary>A building components block that satisfies the shared validation (one height / one animation,
	/// the same number of each, and one variation referencing layer 0).</summary>
	static BuildingComponents CreateBuildingComponents()
		=> new()
		{
			BuildingHeights = [0],
			BuildingAnimations = [new BuildingPartAnimation { NumFrames = 1, AnimationSpeed = 0 }],
			BuildingVariations = [[0]],
		};

	static List<ObjectModelHeader> CreatePlaceholderHeaders(int count)
		=> [.. Enumerable.Range(0, count).Select(_ => new ObjectModelHeader())];

	static StringTable CreateStringTable(ObjectType objectType, string name)
	{
		var stringTable = new StringTable();
		foreach (var key in ObjectAttributes.StringTable((DatObjectType)(byte)objectType))
		{
			var languages = stringTable.AddNewString(key);
			languages[LanguageId.English_UK] = name;
		}

		return stringTable;
	}
}
