using OpenLocoTool.Headers;

namespace OpenLocoTool.Data
{
	public static class SaveEnabledObjects
	{
		public static HashSet<ObjectType> Types =
		[
			ObjectType.Bridge,
			ObjectType.Cargo,
			ObjectType.CliffEdge,
			ObjectType.Climate,
			ObjectType.Competitor,
			ObjectType.Currency,
			ObjectType.Dock,
			ObjectType.HillShapes,
			ObjectType.InterfaceSkin,
			ObjectType.Land,
			ObjectType.LevelCrossing,
			ObjectType.Region,
			ObjectType.RoadExtra,
			ObjectType.Scaffolding,
			ObjectType.ScenarioText,
			ObjectType.Snow,
			ObjectType.Steam,
			ObjectType.StreetLight,
			ObjectType.TrackExtra,
			ObjectType.Tree,
			ObjectType.Wall,
			ObjectType.Water,
		];
	}
}
