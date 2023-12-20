using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoTool.Data
{
	public static class ObjectTypeFixedSize
	{
		public static int GetSize(ObjectType objectType)
			=> objectType switch
			{
				ObjectType.Airport => AirportObject.StructSize,
				ObjectType.Bridge => BridgeObject.StructSize,
				ObjectType.Building => BuildingObject.StructSize,
				ObjectType.Cargo => CargoObject.StructSize,
				ObjectType.CliffEdge => CliffEdgeObject.StructSize,
				ObjectType.Climate => ClimateObject.StructSize,
				ObjectType.Competitor => CompetitorObject.StructSize,
				ObjectType.Currency => CurrencyObject.StructSize,
				ObjectType.Dock => DockObject.StructSize,
				ObjectType.HillShapes => HillShapesObject.StructSize,
				ObjectType.Industry => IndustryObject.StructSize,
				ObjectType.InterfaceSkin => InterfaceSkinObject.StructSize,
				ObjectType.Land => LandObject.StructSize,
				ObjectType.LevelCrossing => LevelCrossingObject.StructSize,
				ObjectType.Region => RegionObject.StructSize,
				ObjectType.RoadExtra => RoadExtraObject.StructSize,
				ObjectType.Road => RoadObject.StructSize,
				ObjectType.RoadStation => RoadStationObject.StructSize,
				ObjectType.Scaffolding => ScaffoldingObject.StructSize,
				ObjectType.ScenarioText => ScenarioTextObject.StructSize,
				ObjectType.Snow => SnowObject.StructSize,
				ObjectType.Sound => SoundObject.StructSize,
				ObjectType.Steam => SteamObject.StructSize,
				ObjectType.StreetLight => StreetLightObject.StructSize,
				ObjectType.TownNames => TownNamesObject.StructSize,
				ObjectType.TrackExtra => TrackExtraObject.StructSize,
				ObjectType.Track => TrackObject.StructSize,
				ObjectType.TrainSignal => TrainSignalObject.StructSize,
				ObjectType.TrainStation => TrainStationObject.StructSize,
				ObjectType.Tree => TreeObject.StructSize,
				ObjectType.Tunnel => TunnelObject.StructSize,
				ObjectType.Vehicle => VehicleObject.StructSize,
				ObjectType.Wall => WallObject.StructSize,
				ObjectType.Water => WaterObject.StructSize,
				_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
			};
	}
}
