using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Types.SCV5
{
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
		public bool Validate() => true;
	}
}
