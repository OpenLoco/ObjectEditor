using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum RoadStationFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		Passenger = 1 << 1,
		Freight = 1 << 2,
		RoadEnd = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x6E)]
	public record RoadStationObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x03)] uint8_t pad_03,
		[property: LocoStructOffset(0x04)] uint16_t RoadPieces,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] RoadStationFlags Flags,
		[property: LocoStructOffset(0x0C)] uint32_t Image,
		[property: LocoStructOffset(0x10), LocoArrayLength(4)] uint32_t[] var_10,
		[property: LocoStructOffset(0x20)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x21), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructOffset(0x28)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x2A)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x2C)] uint8_t CargoType,
		[property: LocoStructOffset(0x2D)] uint8_t pad_2D
		//[property: LocoStructProperty(0x2E)] const std::byte* CargoOffsetBytes[4][4]
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.roadStation;
		public static int StructLength => 0x6E;
	}
}
