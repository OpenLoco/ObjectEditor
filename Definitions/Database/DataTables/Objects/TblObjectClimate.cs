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

	public class TblObjectIndustry : DbSubObject
	{
		public uint32_t FarmImagesPerGrowthStage { get; set; }
		public uint8_t MinNumBuildings { get; set; }
		public uint8_t MaxNumBuildings { get; set; }
		public uint32_t Colours { get; set; }  // bitset
		public uint32_t BuildingSizeFlags { get; set; } // flags indicating the building types size 1:large4x4 { get; set; } 0:small1x1
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }
		public uint8_t TotalOfTypeInScenario { get; set; } // Total industries of this type that can be created in a scenario Note: this is not directly comparable to total industries and varies based on scenario total industries cap settings. At low industries cap this value is ~3x the amount of industries in a scenario.
		public uint8_t CostIndex { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t ScaffoldingSegmentType { get; set; }
		public Colour ScaffoldingColour { get; set; }
		public Colour MapColour { get; set; }
		public IndustryObjectFlags Flags { get; set; }
		public uint8_t FarmTileNumImageAngles { get; set; } // How many viewing angles the farm tiles have
		public uint8_t FarmGrowthStageWithNoProduction { get; set; } // At this stage of growth (except 0) { get; set; } a field tile produces nothing
		public uint8_t FarmIdealSize { get; set; } // Max production is reached at farmIdealSize * 25 tiles
		public uint8_t FarmNumStagesOfGrowth { get; set; } // How many growth stages there are sprites for
		public uint8_t MonthlyClosureChance { get; set; }

		// could actually FK to TblObjectWall instead of using UniqueOjectId? but we'd need it to be TblObject<TblObjectWall> kind of
		public UniqueObjectId BuildingWall { get; set; } // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
		public UniqueObjectId BuildingWallEntrance { get; set; } // An alternative wall type that looks like a gate placed at random places in building perimeter

		//public uint8_t NumBuildingParts { get; set; }
		//public uint8_t NumBuildingVariations { get; set; }
		//public List<uint8_t> BuildingHeights { get; set; }    // This is the height of a building image
		//public List<BuildingPartAnimation> BuildingAnimations { get; set; }
		//public List<List<uint8_t>> AnimationSequences { get; set; } // Access with getAnimationSequence helper method
		//public List<IndustryObjectUnk38> var_38 { get; set; }    // Access with getUnk38 helper method
		//public List<List<uint8_t>> BuildingVariations { get; set; }  // Access with getBuildingParts helper method
		//public List<uint8_t> Buildings { get; set; }
		//public IndustryObjectProductionRateRange[] InitialProductionRate { get; set; }
		//public List<S5Header> ProducedCargo { get; set; } // (0xFF = null)
		//public List<S5Header> RequiredCargo { get; set; } // (0xFF = null)
		//public uint8_t var_E8 { get; set; }
		//public List<S5Header> WallTypes { get; set; } // There can be up to 4 different wall types for an industry
	}

	public class TblObjectInterface : DbSubObject
	{
		public Colour MapTooltipObjectColour { get; set; }
		public Colour MapTooltipCargoColour { get; set; }
		public Colour TooltipColour { get; set; }
		public Colour ErrorColour { get; set; }
		public Colour WindowPlayerColor { get; set; }
		public Colour WindowTitlebarColour { get; set; }
		public Colour WindowColour { get; set; }
		public Colour WindowConstructionColour { get; set; }
		public Colour WindowTerraFormColour { get; set; }
		public Colour WindowMapColour { get; set; }
		public Colour WindowOptionsColour { get; set; }
		public Colour Colour_11 { get; set; }
		public Colour TopToolbarPrimaryColour { get; set; }
		public Colour TopToolbarSecondaryColour { get; set; }
		public Colour TopToolbarTertiaryColour { get; set; }
		public Colour TopToolbarQuaternaryColour { get; set; }
		public Colour PlayerInfoToolbarColour { get; set; }
		public Colour TimeToolbarColour { get; set; }
	}

	public class TblObjectLand : DbSubObject
	{
		public uint8_t CostIndex { get; set; }
		public uint8_t NumGrowthStages { get; set; }
		public uint8_t NumImageAngles { get; set; }
		public LandObjectFlags Flags { get; set; }
		public int16_t CostFactor { get; set; }
		public uint32_t NumImagesPerGrowthStage { get; set; }
		public uint8_t DistributionPattern { get; set; }
		public uint8_t NumVariations { get; set; }
		public uint8_t VariationLikelihood { get; set; }
		public UniqueObjectId CliffEdgeHeader1 { get; set; }
		public UniqueObjectId CliffEdgeHeader2 { get; set; }
	}

	public class TblObjectLevelCrossing : DbSubObject
	{
		public int16_t CostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public uint8_t AnimationSpeed { get; set; }
		public uint8_t ClosingFrames { get; set; }
		public uint8_t ClosedFrames { get; set; }
		public uint16_t DesignedYear { get; set; }

		//public uint8_t var_0A { get; set; } // something like IdleAnimationFrames or something
	}

	public class TblObjectRegion : DbSubObject
	{
		//public uint8_t NumCargoInfluenceObjects { get; set; }
		//public object_id[] _CargoInfluenceObjectIds { get; set; }
		//public CargoInfluenceTownFilterType[] CargoInfluenceTownFilter { get; set; }
	}

	public class TblObjectRoadExtra : DbSubObject
	{
		public RoadTraitFlags RoadPieces { get; set; }
		public uint8_t PaintStyle { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
	}

	public class TblObjectRoad : DbSubObject
	{
		public RoadTraitFlags RoadPieces { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public int16_t TunnelCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public RoadObjectFlags Flags { get; set; }
		public uint8_t PaintStyle { get; set; }
		public uint8_t DisplayOffset { get; set; }
		public TownSize TargetTownSize { get; set; }

		//public uint16_t _CompatibleRoads { get; set; } // bitset
		//public uint16_t _CompatibleTracks { get; set; } // bitset

		public UniqueObjectId Tunnel { get; set; }
		public ICollection<UniqueObjectId> Bridges { get; set; }
		public ICollection<UniqueObjectId> Mods { get; set; }
		public ICollection<UniqueObjectId> RoadsAndTracks { get; set; }
		public ICollection<UniqueObjectId> Stations { get; set; }
	}

	public class TblObjectRoadStation : DbSubObject
	{
		public uint8_t PaintStyle { get; set; }
		public uint8_t Height { get; set; }
		public RoadTraitFlags RoadPieces { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public RoadStationObjectFlags Flags { get; set; }
		public uint8_t CompatibleRoadObjectCount { get; set; }
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }
		public UniqueObjectId CargoTypeId { get; set; }
		public ICollection<uint32_t> ImageOffsets { get; set; }
		public ICollection<uint8_t> _CargoOffsets { get; set; }
		public ICollection<UniqueObjectId> CompatibleRoads { get; set; }
	}


	public class TblObjectScaffolding : DbSubObject
	{
		//public ICollection<uint16_t> SegmentHeights { get; set; }
		//public ICollection<uint16_t> RoofHeights { get; set; }
	}

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

	public class TblObjectWall : DbSubObject
	{
		public uint8_t Height { get; set; }
		public WallObjectFlags1 Flags1 { get; set; }
		public WallObjectFlags2 Flags2 { get; set; }
		public uint8_t ToolId { get; set; } //  tool cursor type - not used in Locomotion - set to 0 in DB?
	}

	public class TblObjectWater : DbSubObject
	{
		public uint8_t CostIndex { get; set; }
		public int16_t CostFactor { get; set; }
		//public uint8_t var_03 { get; set; }
	}
}
