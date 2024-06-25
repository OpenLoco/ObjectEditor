using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x11)]
	class ScenarioObjective
	{
		[LocoStructOffset(0x00)] public ObjectiveType type { get; set; }   // 0x000418 (0x00526230)
		[LocoStructOffset(0x01)] public ObjectiveFlags flags { get; set; } // 0x000419 (0x00526231)
		[LocoStructOffset(0x02)] public uint32_t companyValue { get; set; }          // 0x00041A (0x00526232)
		[LocoStructOffset(0x06)] public uint32_t monthlyVehicleProfit { get; set; }  // 0x00041E (0x00526236)
		[LocoStructOffset(0x0A)] public uint8_t performanceIndex { get; set; }       // 0x000422 (0x0052623A)
		[LocoStructOffset(0x0B)] public uint8_t deliveredCargoType { get; set; }     // 0x000423 (0x0052623B)
		[LocoStructOffset(0x0C)] public uint32_t deliveredCargoAmount { get; set; }  // 0x000424 (0x0052623C)
		[LocoStructOffset(0x10)] public uint8_t timeLimitYears { get; set; }         // 0x000428 (0x00526240)
	}
}
