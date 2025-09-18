using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Dock;

public class DockObject : ILocoStruct, IHasBuildingComponents
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_07 { get; set; } // probably padding, not used in the game
	public DockObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public Pos2 BoatPosition { get; set; }

	public BuildingComponentsModel BuildingComponents { get; set; } = new();

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		var bcValidationContext = new ValidationContext(BuildingComponents);
		foreach (var result in BuildingComponents.Validate(bcValidationContext))
		{
			yield return result;
		}

		if (CostIndex > 32)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be between 0 and 32 (inclusive).", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"{nameof(SellCostFactor)} must be between 0 and -{BuildCostFactor} (inclusive).", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0.", [nameof(BuildCostFactor)]);
		}
	}
}
