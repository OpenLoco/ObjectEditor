namespace OpenLoco.Definitions.Database
{
	public class TblObjectAirport : DbSubObject;

	public class TblObjectBridge : DbSubObject;

	public class TblObjectBuilding : DbSubObject;

	public class TblObjectCargo : DbSubObject;

	public class TblObjectCliffEdge : DbSubObject;

	public class TblObjectClimate : DbSubObject
	{
		public uint8_t FirstSeason { get; set; }
		public uint8_t WinterSnowLine { get; set; }
		public uint8_t SummerSnowLine { get; set; }
		public uint8_t SeasonLength1 { get; set; }
		public uint8_t SeasonLength2 { get; set; }
		public uint8_t SeasonLength3 { get; set; }
		public uint8_t SeasonLength4 { get; set; }
	}

	public class TblObjectCompetitor : DbSubObject;

	public class TblObjectCurrency : DbSubObject;

	public class TblObjectDock : DbSubObject;

	public class TblObjectHillShapes : DbSubObject;

	public class TblObjectIndustry : DbSubObject;

	public class TblObjectInterface : DbSubObject;

	public class TblObjectLand : DbSubObject;

	public class TblObjectLevelCrossing : DbSubObject;

	public class TblObjectRegion : DbSubObject;

	public class TblObjectRoadExtra : DbSubObject;

	public class TblObjectRoad : DbSubObject;

	public class TblObjectRoadStation : DbSubObject;

	public class TblObjectScaffolding : DbSubObject;

	public class TblObjectScenarioText : DbSubObject;

	public class TblObjectSnow : DbSubObject;

	public class TblObjectSound : DbSubObject;

	public class TblObjectSteam : DbSubObject;

	public class TblObjectStreetLight : DbSubObject;

	public class TblObjectTownNames : DbSubObject;

	public class TblObjectTrackExtra : DbSubObject;

	public class TblObjectTrack : DbSubObject;

	public class TblObjectTrackSignal : DbSubObject;

	public class TblObjectTrackStation : DbSubObject;

	public class TblObjectTree : DbSubObject;

	public class TblObjectTunnel : DbSubObject;

	public class TblObjectVehicle : DbSubObject;

	public class TblObjectWall : DbSubObject;

	public class TblObjectWater : DbSubObject;
}
