using Definitions.ObjectModels.Types;
using System.Diagnostics;

namespace Definitions.ObjectModels;

public static class ImageTableGrouper
{
	public static ImageTable CreateImageTable(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		var originalCount = imageList.Count;

		ImageTableNamer.NameImages(obj, objectType, imageList);

		var imageTable = new ImageTable();

		switch (objectType)
		{
			case ObjectType.InterfaceSkin:
				CreateInterfaceGroups(imageList, imageTable);
				break;
			case ObjectType.Sound:
				imageTable.Groups.Add(new("<none>", [.. imageList]));
				break;
			case ObjectType.Currency:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Steam:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.CliffEdge:
				CreateCliffEdgeGroups(imageList, imageTable);
				break;
			case ObjectType.Water:
				CreateWaterGroups(imageList, imageTable);
				break;
			case ObjectType.Land:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.TownNames:
				imageTable.Groups.Add(new("<none>", [.. imageList]));
				break;
			case ObjectType.Cargo:
				CreateCargoGroups(imageList, imageTable);
				break;
			case ObjectType.Wall:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.TrackSignal:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.LevelCrossing:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.StreetLight:
				CreateStreetLightGroups(imageList, imageTable);
				break;
			case ObjectType.Tunnel:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Bridge:
				CreateBridgeGroups(imageList, imageTable);
				break;
			case ObjectType.TrackStation:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.TrackExtra:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Track:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.RoadStation:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.RoadExtra:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Road:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Airport:
				CreateAirportGroups(imageList, imageTable);
				break;
			case ObjectType.Dock:
				CreateDockGroups(imageList, imageTable);
				break;
			case ObjectType.Vehicle:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Tree:
				CreateTreeGroups(imageList, imageTable);
				break;
			case ObjectType.Snow:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Climate:
				imageTable.Groups.Add(new("<none>", [.. imageList]));
				break;
			case ObjectType.HillShapes:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Building:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Industry:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Region:
				imageTable.Groups.Add(new("<uncategorised>", [.. imageList]));
				break;
			case ObjectType.Competitor:
				CreateCompetitorGroups(imageList, imageTable);
				break;
			case ObjectType.ScenarioText:
				imageTable.Groups.Add(new("<none>", [.. imageList]));
				break;
			case ObjectType.Scaffolding:
				CreateScaffoldingGroups(imageList, imageTable);
				break;
			default:
				break;
		}

		Debug.Assert(imageTable.GraphicsElements.Count == originalCount, "Image grouping lost or gained images");

		return imageTable;
	}

	private static void CreateAirportGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("preview", imageList[0..1]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x]))
			.ToList());
	}

	private static void CreateBridgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("preview", imageList[0..1]));
		imageTable.Groups.Add(new("base plates", imageList[1..6]));
		imageTable.Groups.Add(new("unk", imageList[6..12]));
		imageTable.Groups.Add(new("<uncategorised>", imageList[12..]));
	}

	private static void CreateBuildingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups = [.. imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x]))];

	private static void CreateCargoGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("preview", imageList[0..1]));
		imageTable.Groups.Add(new("station variations", imageList[1..]));
	}

	private static void CreateCliffEdgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("left west", imageList[0..16]));
		imageTable.Groups.Add(new("right east", imageList[16..32]));
		imageTable.Groups.Add(new("right west", imageList[32..48]));
		imageTable.Groups.Add(new("left east", imageList[48..64]));
		imageTable.Groups.Add(new("far-side slopes", imageList[64..]));
	}

	private static void CreateCompetitorGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("neutral", imageList[0..2]));
		imageTable.Groups.Add(new("happy", imageList[2..4]));
		imageTable.Groups.Add(new("worried", imageList[4..6]));
		imageTable.Groups.Add(new("thinking", imageList[6..8]));
		imageTable.Groups.Add(new("dejected", imageList[8..10]));
		imageTable.Groups.Add(new("surprised", imageList[10..12]));
		imageTable.Groups.Add(new("scared", imageList[12..14]));
		imageTable.Groups.Add(new("angry", imageList[14..16]));
		imageTable.Groups.Add(new("disgusted", imageList[16..18]));
	}

	private static void CreateDockGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("preview", [imageList[0]]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x]))
			.ToList());
	}

	private static void CreateInterfaceGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("preview", imageList[0..1]));
		imageTable.Groups.Add(new("toolbar", imageList[1..31]));
		imageTable.Groups.Add(new("build-vehicle", imageList[31..43]));
		imageTable.Groups.Add(new("toolbar", imageList[43..49]));
		imageTable.Groups.Add(new("paint", imageList[49..57]));
		imageTable.Groups.Add(new("population", imageList[57..65]));
		imageTable.Groups.Add(new("performance-index", imageList[65..73]));
		imageTable.Groups.Add(new("cargo-units", imageList[73..81]));
		imageTable.Groups.Add(new("cargo-distance", imageList[81..89]));
		imageTable.Groups.Add(new("production", imageList[89..97]));
		imageTable.Groups.Add(new("wrench", imageList[97..113]));
		imageTable.Groups.Add(new("finances", imageList[113..129]));
		imageTable.Groups.Add(new("cup", imageList[129..145]));
		imageTable.Groups.Add(new("ratings", imageList[145..161]));
		imageTable.Groups.Add(new("transported", imageList[161..168]));
		imageTable.Groups.Add(new("cogs", imageList[168..172]));
		imageTable.Groups.Add(new("toolbar", imageList[172..203]));
		imageTable.Groups.Add(new("tab-train", imageList[203..211]));
		imageTable.Groups.Add(new("tab-aircraft", imageList[211..219]));
		imageTable.Groups.Add(new("tab-bus", imageList[219..227]));
		imageTable.Groups.Add(new("tab-tram", imageList[227..235]));
		imageTable.Groups.Add(new("tab-truck", imageList[235..243]));
		imageTable.Groups.Add(new("tab-ship", imageList[243..251]));
		imageTable.Groups.Add(new("build-train", imageList[251..267]));
		imageTable.Groups.Add(new("build-aircraft", imageList[267..283]));
		imageTable.Groups.Add(new("build-bus", imageList[283..299]));
		imageTable.Groups.Add(new("build-tram", imageList[299..315]));
		imageTable.Groups.Add(new("build-truck", imageList[315..331]));
		imageTable.Groups.Add(new("build-ship", imageList[331..347]));
		imageTable.Groups.Add(new("build-industry", imageList[347..363]));
		imageTable.Groups.Add(new("build-town", imageList[363..379]));
		imageTable.Groups.Add(new("build-buildings", imageList[379..395]));
		imageTable.Groups.Add(new("build-misc-buildings", imageList[395..411]));
		imageTable.Groups.Add(new("build-extra", imageList[411..418]));
		imageTable.Groups.Add(new("train", imageList[418..426]));
		imageTable.Groups.Add(new("aircraft", imageList[426..434]));
		imageTable.Groups.Add(new("bus", imageList[434..442]));
		imageTable.Groups.Add(new("tram", imageList[442..450]));
		imageTable.Groups.Add(new("truck", imageList[450..458]));
		imageTable.Groups.Add(new("ship", imageList[458..466]));
		imageTable.Groups.Add(new("toolbar-map", imageList[466..470]));
	}

	private static void CreateScaffoldingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("type 0", imageList[0..10]));
		imageTable.Groups.Add(new("type 1", imageList[10..24]));
		imageTable.Groups.Add(new("type 2", imageList[24..36]));
	}

	private static void CreateStreetLightGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Year group {i}", [.. x]))
			.ToList());

	private static void CreateTreeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Variation {i}", [.. x]))
			.ToList());

	private static void CreateWaterGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(new("zoom 1", imageList[0..10]));
		imageTable.Groups.Add(new("zoom 2", imageList[10..20]));
		imageTable.Groups.Add(new("zoom 3", imageList[20..30]));
		imageTable.Groups.Add(new("zoom 4", imageList[30..40]));
		imageTable.Groups.Add(new("palettes", imageList[40..42]));
		imageTable.Groups.Add(new("icon-animation", imageList[42..58]));
		imageTable.Groups.Add(new("icon-interaction", imageList[58..60]));
		imageTable.Groups.Add(new("animation", imageList[60..76]));
	}
}
