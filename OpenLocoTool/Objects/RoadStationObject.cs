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
	public record RoadStationObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t PaintStyle,
		[property: LocoStructProperty(0x03)] uint8_t pad_03,
		[property: LocoStructProperty(0x04)] uint16_t RoadPieces,
		[property: LocoStructProperty(0x06)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x08)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x0A)] uint8_t CostIndex,
		[property: LocoStructProperty(0x0B)] RoadStationFlags Flags,
		[property: LocoStructProperty(0x0C)] uint32_t Image,
		[property: LocoStructProperty(0x10), LocoArrayLength(4)] uint32_t[] var_10,
		[property: LocoStructProperty(0x20)] uint8_t NumCompatible,
		[property: LocoStructProperty(0x21), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructProperty(0x28)] uint16_t DesignedYear,
		[property: LocoStructProperty(0x2A)] uint16_t ObsoleteYear,
		[property: LocoStructProperty(0x2C)] uint8_t CargoType,
		[property: LocoStructProperty(0x2D)] uint8_t pad_2D
		//[property: LocoStructProperty(0x2E)] const std::byte* CargoOffsetBytes[4][4]
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.roadStation;
		public static int ObjectStructSize => 0x6E;
	}
}
