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
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.Currency:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Steam:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.CliffEdge:
				CreateCliffEdgeGroups(imageList, imageTable);
				break;
			case ObjectType.Water:
				CreateWaterGroups(imageList, imageTable);
				break;
			case ObjectType.Land:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TownNames:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.Cargo:
				CreateCargoGroups(imageList, imageTable);
				break;
			case ObjectType.Wall:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TrackSignal:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.LevelCrossing:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.StreetLight:
				CreateStreetLightGroups(imageList, imageTable);
				break;
			case ObjectType.Tunnel:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Bridge:
				CreateBridgeGroups(imageList, imageTable);
				break;
			case ObjectType.TrackStation:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TrackExtra:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Track:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.RoadStation:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.RoadExtra:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Road:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Airport:
				CreateAirportGroups(imageList, imageTable);
				break;
			case ObjectType.Dock:
				CreateDockGroups(imageList, imageTable);
				break;
			case ObjectType.Vehicle:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Tree:
				CreateTreeGroups(imageList, imageTable);
				break;
			case ObjectType.Snow:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Climate:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.HillShapes:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Building:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Industry:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Region:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Competitor:
				CreateCompetitorGroups(imageList, imageTable);
				break;
			case ObjectType.ScenarioText:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
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
		imageTable.Groups.Add(("preview", imageList[0..1]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList());
	}

	private static void CreateBridgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("base plates", imageList[1..6]));
		imageTable.Groups.Add(("unk", imageList[6..12]));
		imageTable.Groups.Add(("<uncategorised>", imageList[12..]));
	}

	private static void CreateBuildingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups = imageList
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList();

	private static void CreateCargoGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("station variations", imageList[1..]));
	}

	private static void CreateCliffEdgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("left west", imageList[0..16]));
		imageTable.Groups.Add(("right east", imageList[16..32]));
		imageTable.Groups.Add(("right west", imageList[32..48]));
		imageTable.Groups.Add(("left east", imageList[48..64]));
		imageTable.Groups.Add(("far-side slopes", imageList[64..]));
	}

	private static void CreateCompetitorGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("neutral", imageList[0..2]));
		imageTable.Groups.Add(("happy", imageList[2..4]));
		imageTable.Groups.Add(("worried", imageList[4..6]));
		imageTable.Groups.Add(("thinking", imageList[6..8]));
		imageTable.Groups.Add(("dejected", imageList[8..10]));
		imageTable.Groups.Add(("surprised", imageList[10..12]));
		imageTable.Groups.Add(("scared", imageList[12..14]));
		imageTable.Groups.Add(("angry", imageList[14..16]));
		imageTable.Groups.Add(("disgusted", imageList[16..18]));
	}

	private static void CreateDockGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", [imageList[0]]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList());
	}

	private static void CreateInterfaceGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("toolbar", imageList[1..31]));
		imageTable.Groups.Add(("build-vehicle", imageList[31..43]));
		imageTable.Groups.Add(("toolbar", imageList[43..49]));
		imageTable.Groups.Add(("paint", imageList[49..57]));
		imageTable.Groups.Add(("population", imageList[57..65]));
		imageTable.Groups.Add(("performance-index", imageList[65..73]));
		imageTable.Groups.Add(("cargo-units", imageList[73..81]));
		imageTable.Groups.Add(("cargo-distance", imageList[81..89]));
		imageTable.Groups.Add(("production", imageList[89..97]));
		imageTable.Groups.Add(("wrench", imageList[97..113]));
		imageTable.Groups.Add(("finances", imageList[113..129]));
		imageTable.Groups.Add(("cup", imageList[129..145]));
		imageTable.Groups.Add(("ratings", imageList[145..161]));
		imageTable.Groups.Add(("transported", imageList[161..168]));
		imageTable.Groups.Add(("cogs", imageList[168..172]));
		imageTable.Groups.Add(("toolbar", imageList[172..203]));
		imageTable.Groups.Add(("tab-train", imageList[203..211]));
		imageTable.Groups.Add(("tab-aircraft", imageList[211..219]));
		imageTable.Groups.Add(("tab-bus", imageList[219..227]));
		imageTable.Groups.Add(("tab-tram", imageList[227..235]));
		imageTable.Groups.Add(("tab-truck", imageList[235..243]));
		imageTable.Groups.Add(("tab-ship", imageList[243..251]));
		imageTable.Groups.Add(("build-train", imageList[251..267]));
		imageTable.Groups.Add(("build-aircraft", imageList[267..283]));
		imageTable.Groups.Add(("build-bus", imageList[283..299]));
		imageTable.Groups.Add(("build-tram", imageList[299..315]));
		imageTable.Groups.Add(("build-truck", imageList[315..331]));
		imageTable.Groups.Add(("build-ship", imageList[331..347]));
		imageTable.Groups.Add(("build-industry", imageList[347..363]));
		imageTable.Groups.Add(("build-town", imageList[363..379]));
		imageTable.Groups.Add(("build-buildings", imageList[379..395]));
		imageTable.Groups.Add(("build-misc-buildings", imageList[395..411]));
		imageTable.Groups.Add(("build-extra", imageList[411..418]));
		imageTable.Groups.Add(("train", imageList[418..426]));
		imageTable.Groups.Add(("aircraft", imageList[426..434]));
		imageTable.Groups.Add(("bus", imageList[434..442]));
		imageTable.Groups.Add(("tram", imageList[442..450]));
		imageTable.Groups.Add(("truck", imageList[450..458]));
		imageTable.Groups.Add(("ship", imageList[458..466]));
		imageTable.Groups.Add(("toolbar-map", imageList[466..470]));
	}

	private static void CreateScaffoldingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("type 0", imageList[0..10]));
		imageTable.Groups.Add(("type 1", imageList[10..24]));
		imageTable.Groups.Add(("type 2", imageList[24..36]));
	}

	private static void CreateStreetLightGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => ($"Year group {i}", x.ToList()))
			.ToList());

	private static void CreateTreeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => ($"Variation {i}", x.ToList()))
			.ToList());

	private static void CreateWaterGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("zoom 1", imageList[0..10]));
		imageTable.Groups.Add(("zoom 2", imageList[10..20]));
		imageTable.Groups.Add(("zoom 3", imageList[20..30]));
		imageTable.Groups.Add(("zoom 4", imageList[30..40]));
		imageTable.Groups.Add(("palettes", imageList[40..42]));
		imageTable.Groups.Add(("icon-animation", imageList[42..58]));
		imageTable.Groups.Add(("icon-interaction", imageList[58..60]));
		imageTable.Groups.Add(("animation", imageList[60..76]));
	}
}
