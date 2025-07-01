using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectAirport : DbSubObject
	{
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint16_t AllowedPlaneTypes { get; set; }
		public uint32_t LargeTiles { get; set; }
		public int8_t MinX { get; set; }
		public int8_t MinY { get; set; }
		public int8_t MaxX { get; set; }
		public int8_t MaxY { get; set; }
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }

		//public uint8_t var_07 { get; set; }
		//public uint8_t NumBuildingParts { get; set; }
		//public uint8_t NumBuildingVariations { get; set; }
		//public List<uint8_t> BuildingHeights { get; set; }
		//public List<BuildingPartAnimation> BuildingAnimations { get; set; }
		//public List<List<uint8_t>> BuildingVariations { get; set; }
		//public List<AirportBuilding> BuildingPositions { get; set; }
		//public uint8_t NumMovementNodes { get; set; }
		//public uint8_t NumMovementEdges { get; set; }
		//public List<MovementNode> MovementNodes { get; set; }
		//public List<MovementEdge> MovementEdges { get; set; }
		//public uint8_t[] var_B6 { get; set; }
	}

	public class TblObjectBridge : DbSubObject
	{
		public BridgeObjectFlags Flags { get; set; }
		public uint16_t ClearHeight { get; set; }
		public int16_t DeckDepth { get; set; }
		public uint8_t SpanLength { get; set; }
		public uint8_t PillarSpacing { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public MicroZ MaxHeight { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BaseCostFactor { get; set; }
		public int16_t HeightCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
		public uint16_t DesignedYear { get; set; }

		//public uint8_t CompatibleTrackObjectCount { get; set; }
		//public uint8_t CompatibleRoadObjectCount { get; set; }
		// how to store in DB? just json? base64 encoded?
		//public ICollection<UniqueObjectId> CompatibleTrackObjects { get; set; }
		//public ICollection<UniqueObjectId> CompatibleRoadObjects { get; set; }
	}

	public class TblObjectBuilding : DbSubObject
	{
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }
		public BuildingObjectFlags Flags { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint16_t SellCostFactor { get; set; }
		public int16_t DemolishRatingReduction { get; set; }
		public uint8_t ScaffoldingSegmentType { get; set; }
		public Colour ScaffoldingColour { get; set; }
		public uint32_t Colours { get; set; }
		public uint8_t GeneratorFunction { get; set; }
		public uint8_t AverageNumberOnMap { get; set; }

		//public uint8_t NumBuildingParts { get; set; }
		//public uint8_t NumBuildingVariations { get; set; }
		//List<uint8_t> BuildingHeights { get; set; }
		//List<BuildingPartAnimation> BuildingAnimations { get; set; }
		//List<List<uint8_t>> BuildingVariations { get; set; }
		//public uint8_t[] ProducedQuantity { get; set; }
		//List<S5Header> ProducedCargo { get; set; }
		//List<S5Header> RequiredCargo { get; set; }
		//List<uint8_t> var_A6 { get; set; }
		//List<uint8_t> var_A8 { get; set; }
		//public uint8_t var_AC { get; set; }
		//public uint8_t NumElevatorSequences { get; set; }
		//List<uint8_t[]> _ElevatorHeightSequences // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
	}

	public class TblObjectCargo : DbSubObject
	{
		public uint16_t CargoTransferTime { get; set; }
		public CargoCategory CargoCategory { get; set; }
		public CargoObjectFlags Flags { get; set; }
		public uint8_t NumPlatformVariations { get; set; }
		public uint8_t StationCargoDensity { get; set; }
		public uint8_t PremiumDays { get; set; }
		public uint8_t MaxNonPremiumDays { get; set; }
		public uint16_t MaxPremiumRate { get; set; }
		public uint16_t PenaltyRate { get; set; }
		public uint16_t PaymentFactor { get; set; }
		public uint8_t PaymentIndex { get; set; }
		public uint8_t UnitSize { get; set; }

		//uint16_t var_02 { get; set; }
	}

	public class TblObjectCliffEdge : DbSubObject
	{
		// no data
	}

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

	public class TblObjectCompetitor : DbSubObject
	{
		public CompetitorNamePrefix AvailableNamePrefixes { get; set; } // bitset
		public CompetitorPlaystyle AvailablePlaystyles { get; set; } // bitset
		public uint32_t Emotions { get; set; } // bitset
		public uint8_t Intelligence { get; set; }
		public uint8_t Aggressiveness { get; set; }
		public uint8_t Competitiveness { get; set; }
		//public uint8_t var_37 { get; set; }
	}

	public class TblObjectCurrency : DbSubObject
	{
		public uint8_t Separator { get; set; }
		public uint8_t Factor { get; set; }
	}

	public class TblObjectDock : DbSubObject
	{
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public DockObjectFlags Flags { get; set; }
		public uint8_t NumBuildingPartAnimations { get; set; }
		public uint8_t NumBuildingVariationParts { get; set; } // must be 1 or 0
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }

		// these map to [Pos2 BoatPosition] in the Dock object
		public coord_t BoatPositionX { get; set; }
		public coord_t BoatPositionY { get; set; }

		// public uint8_t var_07 { get; set; } // probably padding
		// public List<uint8_t> BuildingPartHeights { get; set; }
		// public List<uint16_t> BuildingPartAnimations { get; set; }
		// public List<uint8_t> BuildingVariationParts { get; set; }
	}

	public class TblObjectHillShapes : DbSubObject
	{
		public uint8_t HillHeightMapCount { get; set; }
		public uint8_t MountainHeightMapCount { get; set; }
		public HillShapeFlags Flags { get; set; }
	}

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
