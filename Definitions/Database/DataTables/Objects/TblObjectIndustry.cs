using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectIndustry : DbSubObject, IConvertibleToTable<TblObjectIndustry, IndustryObject>
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

		// FK to linked objects
		//public TblObjectWall BuildingWall { get; set; } // Selection of wall types isn't completely random from the 4 it is biased into 2 groups of 2 (wall and entrance)
		//public TblObjectWall BuildingWallEntrance { get; set; } // An alternative wall type that looks like a gate placed at random places in building perimeter

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

		public static TblObjectIndustry FromObject(TblObject tbl, IndustryObject obj)
			=> new()
			{
				Parent = tbl,
				FarmImagesPerGrowthStage = obj.FarmImagesPerGrowthStage,
				MinNumBuildings = obj.MinNumBuildings,
				MaxNumBuildings = obj.MaxNumBuildings,
				Colours = obj.Colours,
				BuildingSizeFlags = obj.BuildingSizeFlags,
				DesignedYear = obj.DesignedYear,
				ObsoleteYear = obj.ObsoleteYear,
				TotalOfTypeInScenario = obj.TotalOfTypeInScenario,
				CostIndex = obj.CostIndex,
				BuildCostFactor = obj.BuildCostFactor,
				SellCostFactor = obj.SellCostFactor,
				ScaffoldingSegmentType = obj.ScaffoldingSegmentType,
				ScaffoldingColour = obj.ScaffoldingColour,
				MapColour = obj.MapColour,
				Flags = obj.Flags,
				FarmTileNumImageAngles = obj.FarmTileNumImageAngles,
				FarmGrowthStageWithNoProduction = obj.FarmGrowthStageWithNoProduction,
				FarmIdealSize = obj.FarmIdealSize,
				FarmNumStagesOfGrowth = obj.FarmNumStagesOfGrowth,
				MonthlyClosureChance = obj.MonthlyClosureChance,
				//BuildingWall = obj.BuildingWall, ?? how to do ?? needs to look up the object in DB from the dat name+checksum
				//BuildingWallEntrance = obj.BuildingWallEntrance, ??
			};
	}
}
