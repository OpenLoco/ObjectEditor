using Dat.Data;
using Dat.Objects;
using Dat.Types;

namespace Dat.FileParsing;

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

	public static ObjectType? StructToType(ILocoStruct locoStruct)
		=> AttributeHelper.Get<LocoStructTypeAttribute>(locoStruct.GetType())?.ObjectType;
}
