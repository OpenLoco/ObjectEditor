using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum DockObjectFlags : uint16_t
	{
		None = 0,
		unk01 = 1 << 0,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	public record DockObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x04)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x06)] uint8_t CostIndex,
		[property: LocoStructProperty(0x07)] uint8_t var_07,
		[property: LocoStructProperty(0x08)] uint32_t Image,
		[property: LocoStructProperty(0x0C)] uint32_t var_0C,
		[property: LocoStructProperty(0x10)] DockObjectFlags Flags,
		[property: LocoStructProperty(0x12)] uint8_t NumAux01,
		//[property: LocoStructProperty(0x13)] uint8_t numAux02Ent,   // must be 1 or 0
		//[property: LocoStructProperty(0x14)] const uint8_t* var_14,
		//[property: LocoStructProperty(0x18)] const uint16_t* var_18,
		//[property: LocoStructProperty(0x1C)] const uint8_t* var_1C[1], // odd that this is size 1 but that is how its used
		[property: LocoStructProperty(0x20)] uint16_t DesignedYear,
		[property: LocoStructProperty(0x22)] uint16_t ObsoleteYear,
		[property: LocoStructProperty(0x24)] Pos2 BoatPosition
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.dock;
		public static int StructLength => 0x28;
	}
}
