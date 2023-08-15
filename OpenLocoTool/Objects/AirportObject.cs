
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	public enum AirportMovementNodeFlags : uint16_t
	{
		None = 0,
		Terminal = 1 << 0,
		TakeoffEnd = 1 << 1,
		Flag2 = 1 << 2,
		Taxiing = 1 << 3,
		InFlight = 1 << 4,
		HeliTakeoffBegin = 1 << 5,
		TakeoffBegin = 1 << 6,
		HeliTakeoffEnd = 1 << 7,
		Touchdown = 1 << 8,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x08)]
	public record MovementNode(
		[property: LocoStructProperty(0x00)] int16_t X,
		[property: LocoStructProperty(0x02)] int16_t Y,
		[property: LocoStructProperty(0x04)] int16_t Z,
		[property: LocoStructProperty(0x06)] AirportMovementNodeFlags Flags
		) : ILocoStruct
	{
		public static int StructLength => 0x08;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record MovementEdge(
		[property: LocoStructProperty(0x00)] uint8_t var_00,
		[property: LocoStructProperty(0x01)] uint8_t CurrNode,
		[property: LocoStructProperty(0x02)] uint8_t NextNode,
		[property: LocoStructProperty(0x03)] uint8_t var_03,
		[property: LocoStructProperty(0x04)] uint32_t MustBeClearEdges,    // Which edges must be clear to use the transition edge
		[property: LocoStructProperty(0x08)] uint32_t AtLeastOneClearEdges // Which edges must have at least one clear to use transition edge
		) : ILocoStruct
	{
		public static int StructLength => 0x0C;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xBA)]
	public record AirportObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x04)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x06)] uint8_t CostIndex,
		[property: LocoStructProperty(0x07)] uint8_t var_07,
		[property: LocoStructProperty(0x08)] uint32_t Image,
		[property: LocoStructProperty(0x0C)] uint32_t var_0C,
		[property: LocoStructProperty(0x10)] uint16_t AllowedPlaneTypes,
		[property: LocoStructProperty(0x12)] uint8_t NumSpriteSets,
		[property: LocoStructProperty(0x13)] uint8_t NumTiles,
		//[property: LocoStructProperty(0x14)] const uint8_t* var_14,
		//[property: LocoStructProperty(0x18)] const uint16_t* var_18,
		//[property: LocoStructProperty(0x1C)] const uint8_t* var_1C[32],
		//[property: LocoStructProperty(0x9C)] const uint32_t* var_9C,
		[property: LocoStructProperty(0xA0)] uint32_t LargeTiles,
		[property: LocoStructProperty(0xA4)] int8_t MinX,
		[property: LocoStructProperty(0xA5)] int8_t MinY,
		[property: LocoStructProperty(0xA6)] int8_t MaxX,
		[property: LocoStructProperty(0xA7)] int8_t MaxY,
		[property: LocoStructProperty(0xA8)] uint16_t DesignedYear,
		[property: LocoStructProperty(0xAA)] uint16_t ObsoleteYear,
		[property: LocoStructProperty(0xAC)] uint8_t NumMovementNodes,
		[property: LocoStructProperty(0xAD)] uint8_t NumMovementEdges,
		//[property: LocoStructProperty(0xAE)] const MovementNode* movementNodes,
		//[property: LocoStructProperty(0xB2)] const MovementEdge* movementEdges,
		[property: LocoStructProperty(0xB6), LocoArrayLength(0xBA - 0xB6)] uint8_t[] pad_B6
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.airport;
		public static int StructLength => 0xBA;
	}
}
