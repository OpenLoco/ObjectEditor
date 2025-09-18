using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x11)]
public record ScenarioObjective(
	[property: LocoStructOffset(0x00)] ObjectiveType Type,
	[property: LocoStructOffset(0x01)] ObjectiveFlags Flags,
	[property: LocoStructOffset(0x02)] uint32_t CompanyValue,
	[property: LocoStructOffset(0x06)] uint32_t MonthlyVehicleProfit,
	[property: LocoStructOffset(0x0A)] uint8_t PerformanceIndex,
	[property: LocoStructOffset(0x0B)] uint8_t DeliveredCargoType,
	[property: LocoStructOffset(0x0C)] uint32_t DeliveredCargoAmount,
	[property: LocoStructOffset(0x10)] uint8_t TimeLimitYears)
	: ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
