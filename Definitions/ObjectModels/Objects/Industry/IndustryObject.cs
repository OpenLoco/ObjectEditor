using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Industry;

public class IndustryObject : ILocoStruct, IHasBuildingComponents
{
	public uint32_t FarmImagesPerGrowthStage { get; set; }
	public BuildingComponentsModel BuildingComponents { get; set; } = new();
	[Length(4, 4)]
	public List<List<uint8_t>> AnimationSequences { get; set; } = []; // Access with getAnimationSequence helper method
	public List<IndustryObjectUnk38> var_38 { get; set; } = []; // Access with getUnk38 helper method
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

	[Length(2, 2)]
	public List<IndustryObjectProductionRateRange> InitialProductionRate { get; set; } = []; // 2 entries, min and max production rate

	[Length(2, 2)]
	public List<ObjectModelHeader> ProducedCargo { get; set; } = []; // Cargo produced by this industry

	[Length(3, 3)]
	public List<ObjectModelHeader> RequiredCargo { get; set; } = []; // Cargo required by this industry

	public Colour MapColour { get; set; }
	public IndustryObjectFlags Flags { get; set; }
	public uint8_t var_E8 { get; set; } // Unused, but must be 0 or 1
	public uint8_t FarmTileNumImageAngles { get; set; } // How many viewing angles the farm tiles have
	public uint8_t FarmGrowthStageWithNoProduction { get; set; } // At this stage of growth (except 0) { get; set; } a field tile produces nothing
	public uint8_t FarmNumFields { get; set; } // Max production is reached at farmIdealSize * 25 tiles
	public uint8_t FarmNumStagesOfGrowth { get; set; } // How many growth stages there are sprites for
	public List<ObjectModelHeader> WallTypes { get; set; } = []; // Wall types that can be built around this industry

	public ObjectModelHeader? BuildingWall { get; set; } // Wall types that can be built around this industry
	public ObjectModelHeader? BuildingWallEntrance { get; set; } // Wall types that can be built around this industry
	public uint8_t MonthlyClosureChance { get; set; }
	public List<uint8_t> Buildings { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var bcValidationContext = new ValidationContext(BuildingComponents);
		foreach (var result in BuildingComponents.Validate(bcValidationContext))
		{
			yield return result;
		}

		if (MaxNumBuildings < MinNumBuildings)
		{
			yield return new ValidationResult("MaxNumBuildings must be greater than or equal to MinNumBuildings", [nameof(MaxNumBuildings), nameof(MinNumBuildings)]);
		}

		if (TotalOfTypeInScenario is 0 or > 32)
		{
			yield return new ValidationResult("TotalOfTypeInScenario must be between 1 and 32", [nameof(TotalOfTypeInScenario)]);
		}

		// it costs money to setll an Industry
		//if (SellCostFactor >= 0)
		//{
		//	yield return new ValidationResult($"{nameof(SellCostFactor)} must be less than 0 {nameof(SellCostFactor)}", [nameof(SellCostFactor)]);
		//}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0", [nameof(BuildCostFactor)]);
		}

		// 230/256 = ~90%
		if (-SellCostFactor > BuildCostFactor * 230 / 256)
		{
			yield return new ValidationResult($"-{nameof(SellCostFactor)} must be at least -90% of {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (var_E8 > 8)
		{
			yield return new ValidationResult("var_E8 must be between 0 and 8", [nameof(var_E8)]);
		}

		if (FarmTileNumImageAngles is not 1 or 2 or 4)
		{
			yield return new ValidationResult("FarmTileNumImageAngles must be 1, 2, or 4", [nameof(FarmTileNumImageAngles)]);
		}

		if (FarmGrowthStageWithNoProduction is not 0xFF and > 7)
		{
			yield return new ValidationResult("FarmGrowthStageWithNoProduction must be between 0 and 7, or 0xFF", [nameof(FarmGrowthStageWithNoProduction)]);
		}

		if (FarmNumStagesOfGrowth > 8)
		{
			yield return new ValidationResult("FarmNumStagesOfGrowth must be between 1 and 8", [nameof(FarmNumStagesOfGrowth)]);
		}

		if (InitialProductionRate[0].Min > 100)
		{
			yield return new ValidationResult("InitialProductionRate[0].Min must be less than or equal to 100", [nameof(InitialProductionRate)]);
		}

		if (InitialProductionRate[1].Min > 100)
		{
			yield return new ValidationResult("InitialProductionRate[1].Min must be less than or equal to 100", [nameof(InitialProductionRate)]);
		}
	}
}
