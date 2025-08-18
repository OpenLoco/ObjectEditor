using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Industry;

public class IndustryObject : ILocoStruct
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
	public List<IndustryObjectProductionRateRange> InitialProductionRate { get; set; } = []; // 2 entries, min and max production rate
	public Colour MapColour { get; set; }
	public IndustryObjectFlags Flags { get; set; }
	public uint8_t var_E8 { get; set; } // Unused, but must be 0 or 1
	public uint8_t FarmTileNumImageAngles { get; set; } // How many viewing angles the farm tiles have
	public uint8_t FarmGrowthStageWithNoProduction { get; set; } // At this stage of growth (except 0) { get; set; } a field tile produces nothing
	public uint8_t FarmIdealSize { get; set; } // Max production is reached at farmIdealSize * 25 tiles
	public uint8_t FarmNumStagesOfGrowth { get; set; } // How many growth stages there are sprites for
	public uint8_t MonthlyClosureChance { get; set; }

	public List<ObjectModelHeader> ProducedCargo { get; set; } = []; // Cargo produced by this industry
	public List<ObjectModelHeader> RequiredCargo { get; set; } = []; // Cargo required by this industry
	public List<ObjectModelHeader> WallTypes { get; set; } = []; // Wall types that can be built around this industry
	public ObjectModelHeader BuildingWall { get; set; } // Wall types that can be built around this industry
	public ObjectModelHeader BuildingWallEntrance { get; set; } // Wall types that can be built around this industry

	public List<uint8_t> BuildingHeights { get; set; } = []; // This is the height of a building image

	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];

	public List<List<uint8_t>> AnimationSequences { get; set; } = []; // Access with getAnimationSequence helper method

	public List<IndustryObjectUnk38> var_38 { get; set; } = []; // Access with getUnk38 helper method

	public List<List<uint8_t>> BuildingVariations { get; set; } = []; // Access with getBuildingParts helper method

	public bool Validate()
	{
		if (BuildingHeights.Count == 0 || BuildingAnimations.Count == 0)
		{
			return false;
		}

		if (BuildingVariations.Count is 0 or > 31)
		{
			return false;
		}

		if (MaxNumBuildings < MinNumBuildings)
		{
			return false;
		}

		if (TotalOfTypeInScenario is 0 or > 32)
		{
			return false;
		}

		// 230/256 = ~90%
		if (-SellCostFactor > BuildCostFactor * 230 / 256)
		{
			return false;
		}

		if (var_E8 > 8)
		{
			return false;
		}

		switch (FarmTileNumImageAngles)
		{
			case 1:
			case 2:
			case 4:
				break;
			default:
				return false;
		}

		if (FarmGrowthStageWithNoProduction is not 0xFF and > 7)
		{
			return false;
		}

		if (FarmNumStagesOfGrowth > 8)
		{
			return false;
		}

		if (InitialProductionRate[0].Min > 100)
		{
			return false;
		}

		return InitialProductionRate[1].Min <= 100;
	}
}
