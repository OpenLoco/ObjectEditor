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

namespace ObjectService.RouteHandlers;

public static class ObjectTypeMapping
{
	public static Type TypeToStruct(ObjectType objectType)
		=> objectType switch
		{
			ObjectType.Airport => typeof(AirportObject),
			ObjectType.Bridge => typeof(BridgeObject),
			ObjectType.Building => typeof(BuildingObject),
			ObjectType.Cargo => typeof(CargoObject),
			ObjectType.CliffEdge => typeof(CliffEdgeObject),
			ObjectType.Climate => typeof(ClimateObject),
			ObjectType.Competitor => typeof(CompetitorObject),
			ObjectType.Currency => typeof(CurrencyObject),
			ObjectType.Dock => typeof(DockObject),
			ObjectType.HillShapes => typeof(HillShapesObject),
			ObjectType.Industry => typeof(IndustryObject),
			ObjectType.InterfaceSkin => typeof(InterfaceSkinObject),
			ObjectType.Land => typeof(LandObject),
			ObjectType.LevelCrossing => typeof(LevelCrossingObject),
			ObjectType.Region => typeof(RegionObject),
			ObjectType.RoadExtra => typeof(RoadExtraObject),
			ObjectType.Road => typeof(RoadObject),
			ObjectType.RoadStation => typeof(RoadStationObject),
			ObjectType.Scaffolding => typeof(ScaffoldingObject),
			ObjectType.ScenarioText => typeof(ScenarioTextObject),
			ObjectType.Snow => typeof(SnowObject),
			ObjectType.Sound => typeof(SoundObject),
			ObjectType.Steam => typeof(SteamObject),
			ObjectType.StreetLight => typeof(StreetLightObject),
			ObjectType.TownNames => typeof(TownNamesObject),
			ObjectType.TrackExtra => typeof(TrackExtraObject),
			ObjectType.Track => typeof(TrackObject),
			ObjectType.TrackSignal => typeof(TrackSignalObject),
			ObjectType.TrackStation => typeof(TrackStationObject),
			ObjectType.Tree => typeof(TreeObject),
			ObjectType.Tunnel => typeof(TunnelObject),
			ObjectType.Vehicle => typeof(VehicleObject),
			ObjectType.Wall => typeof(WallObject),
			ObjectType.Water => typeof(WaterObject),
			_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
		};
}
