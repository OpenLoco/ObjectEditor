using OpenLocoTool.Headers;

namespace OpenLocoTool.Data
{
	public static class SaveEnabledObjects
	{
		public static HashSet<ObjectType> Types =
		[
			ObjectType.Bridge,
			ObjectType.CliffEdge,
			ObjectType.Climate,
			ObjectType.Competitor,
			ObjectType.Currency,
			ObjectType.HillShapes,
			ObjectType.InterfaceSkin,
			ObjectType.Land,
			ObjectType.LevelCrossing,
			ObjectType.Region,
			ObjectType.Scaffolding,
			ObjectType.ScenarioText,
			ObjectType.Snow,
			ObjectType.StreetLight,
			ObjectType.Tree,
		];
	}
}
