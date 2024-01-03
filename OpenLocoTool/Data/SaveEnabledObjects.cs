using System;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Data
{
	public static class SaveEnabledObjects
	{
		public static HashSet<ObjectType> Types =
		[
			ObjectType.Airport,
			ObjectType.Bridge,
			//ObjectType.Building,
			ObjectType.Cargo,
			ObjectType.CliffEdge,
			ObjectType.Climate,
			ObjectType.Competitor,
			ObjectType.Currency,
			ObjectType.Dock,
			ObjectType.HillShapes,
			//ObjectType.Industry,
			ObjectType.InterfaceSkin,
			ObjectType.Land,
			ObjectType.LevelCrossing,
			ObjectType.Region,
			ObjectType.RoadExtra,
			ObjectType.Road,
			ObjectType.RoadStation,
			ObjectType.Scaffolding,
			ObjectType.ScenarioText,
			ObjectType.Snow,
			//ObjectType.Sound,
			ObjectType.Steam,
			ObjectType.StreetLight,
			ObjectType.TownNames,
			ObjectType.TrackExtra,
			ObjectType.Track,
			ObjectType.TrainSignal,
			ObjectType.TrainStation,
			ObjectType.Tree,
			ObjectType.Tunnel,
			//ObjectType.Vehicle,
			ObjectType.Wall,
			ObjectType.Water,
		];
	}
}
