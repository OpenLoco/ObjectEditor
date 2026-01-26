using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Building;

public class BuildingObject : ILocoStruct, IHasBuildingComponents
{
	public static class Constants
	{
		public const int MaxVariationsCount = 32;
		public const int MaxHeightsCount = 64;
		public const int MaxAnimationsCount = 64;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 2;
		public const int MaxElevatorHeightSequencesCount = 4;
	}

	public BuildingComponents BuildingComponents { get; set; } = new();
	public uint32_t Colours { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public BuildingObjectFlags Flags { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint16_t SellCostFactor { get; set; }
	public uint8_t ScaffoldingSegmentType { get; set; }
	public Colour ScaffoldingColour { get; set; }
	public uint8_t GeneratorFunction { get; set; } // for misc buildings generator function, otherwise its a set of flags representing town densities the building can be built in
	public uint8_t AverageNumberOnMap { get; set; }
	public List<uint8_t> ProducedQuantity { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargo { get; set; } = [];
	public List<ObjectModelHeader> RequiredCargo { get; set; } = [];
	public uint8_t var_A6 { get; set; }
	public uint8_t var_A7 { get; set; }
	public uint8_t var_A8 { get; set; }
	public uint8_t var_A9 { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public uint8_t var_AC { get; set; }
	public List<uint8_t[]> ElevatorHeightSequences { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var bcValidationContext = new ValidationContext(BuildingComponents);
		foreach (var result in BuildingComponents.Validate(bcValidationContext))
		{
			yield return result;
		}

		if (ProducedQuantity.Count != 2)
		{
			yield return new ValidationResult($"{nameof(ProducedQuantity)} must have exactly 2 entries.", [nameof(ProducedQuantity)]);
		}

		if (Flags.HasFlag(BuildingObjectFlags.MiscBuilding))
		{
			if (GeneratorFunction >= 4)
			{
				yield return new ValidationResult($"Buildings with the {nameof(BuildingObjectFlags.MiscBuilding)} flag can only use generator functions < 4.", [nameof(GeneratorFunction)]);
			}
		}

		if (var_AC != 0xFF)
		{
			// Max of 8 different building categories
			if (var_AC >= 8)
			{
				yield return new ValidationResult($"{nameof(var_AC)} must either be 255 or < 8.", [nameof(var_AC)]);
			}
		}
	}
}
