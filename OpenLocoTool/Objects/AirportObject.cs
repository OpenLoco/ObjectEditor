
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
		[property: LocoStructOffset(0x00)] int16_t X,
		[property: LocoStructOffset(0x02)] int16_t Y,
		[property: LocoStructOffset(0x04)] int16_t Z,
		[property: LocoStructOffset(0x06)] AirportMovementNodeFlags Flags
		) : ILocoStruct
	{
		public static int StructSize => 0x08;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record MovementEdge(
		[property: LocoStructOffset(0x00)] uint8_t var_00,
		[property: LocoStructOffset(0x01)] uint8_t CurrNode,
		[property: LocoStructOffset(0x02)] uint8_t NextNode,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04)] uint32_t MustBeClearEdges,    // Which edges must be clear to use the transition edge. should probably be some kind of flags?
		[property: LocoStructOffset(0x08)] uint32_t AtLeastOneClearEdges // Which edges must have at least one clear to use transition edge. should probably be some kind of flags?
		) : ILocoStruct
	{
		public static int StructSize => 0x0C;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xBA)]
	public record AirportObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x06)] uint8_t CostIndex,
		[property: LocoStructOffset(0x07)] uint8_t var_07,
		[property: LocoStructOffset(0x08)] uint32_t Image,
		[property: LocoStructOffset(0x0C)] uint32_t var_0C,
		[property: LocoStructOffset(0x10)] uint16_t AllowedPlaneTypes,
		[property: LocoStructOffset(0x12)] uint8_t NumSpriteSets,
		[property: LocoStructOffset(0x13)] uint8_t NumTiles,
		[property: LocoStructOffset(0x14), LocoStructVariableLoad] List<uint8_t> var_14,
		[property: LocoStructOffset(0x14), LocoStructVariableLoad] List<uint16_t> var_18,
		//[property: LocoStructOffset(0x1C), LocoStructVariableLoad, LocoArrayLength(32)] uint8_t[] var_1C, // due to problem in .net7 and lower, winforms cannot display this in property grid
		[property: LocoStructOffset(0x1C), LocoStructVariableLoad, LocoArrayLength(32)] List<uint8_t> var_1C,
		[property: LocoStructOffset(0x9C), LocoStructVariableLoad] List<uint32_t> var_9C,
		[property: LocoStructOffset(0xA0)] uint32_t LargeTiles,
		[property: LocoStructOffset(0xA4)] int8_t MinX,
		[property: LocoStructOffset(0xA5)] int8_t MinY,
		[property: LocoStructOffset(0xA6)] int8_t MaxX,
		[property: LocoStructOffset(0xA7)] int8_t MaxY,
		[property: LocoStructOffset(0xA8)] uint16_t DesignedYear,
		[property: LocoStructOffset(0xAA)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0xAC)] uint8_t NumMovementNodes,
		[property: LocoStructOffset(0xAD)] uint8_t NumMovementEdges,
		[property: LocoStructOffset(0xAE), LocoStructVariableLoad] List<MovementNode> MovementNodes,
		[property: LocoStructOffset(0xB2), LocoStructVariableLoad] List<MovementEdge> MovementEdges,
		[property: LocoStructOffset(0xB6), LocoArrayLength(0xBA - 0xB6)] uint8_t[] pad_B6
	) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Airport;
		public static int StructSize => 0xBA;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// var_14
			var_14.Clear();
			var_14.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumSpriteSets * 1)], NumSpriteSets));
			remainingData = remainingData[(NumSpriteSets * 1)..]; // uint8_t*

			// var_18
			var_18.Clear();
			var_18.AddRange(ByteReaderT.Read_Array<uint16_t>(remainingData[..(NumSpriteSets * 2)], NumSpriteSets));
			remainingData = remainingData[(NumSpriteSets * 2)..]; // uint16_t*

			// numTiles
			var_1C.Clear();
			for (var i = 0; i < NumTiles; ++i)
			{
				var_1C.Add(ByteReaderT.Read_uint8t(remainingData[0..1], 0));
				var ptr_1C = 0;
				while (remainingData[ptr_1C++] != 0xFF) ;
				remainingData = remainingData[ptr_1C..];
			}

			// var_9C
			var ptr_9C = 0;
			var_9C.Clear();
			while (remainingData[ptr_9C] != 0xFF)
			{
				var_9C.Add(ByteReaderT.Read_uint32t(remainingData[ptr_9C..(ptr_9C + 4)], 0));
				ptr_9C += 4;
			}
			ptr_9C++;
			remainingData = remainingData[ptr_9C..];

			// movement nodes
			// could use ByteReaderT.Read_Array if the MovementNode record was a byte-aligned struct
			MovementNodes.Clear();
			MovementNodes.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumMovementNodes * MovementNode.StructSize)], typeof(MovementNode), NumMovementNodes, MovementNode.StructSize)
				.Cast<MovementNode>());
			remainingData = remainingData[(NumMovementNodes * MovementNode.StructSize)..];

			// movement edges
			MovementEdges.Clear();
			MovementEdges.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumMovementEdges * MovementEdge.StructSize)], typeof(MovementEdge), NumMovementEdges, MovementEdge.StructSize)
				.Cast<MovementEdge>()
				.ToList());
			remainingData = remainingData[(NumMovementEdges * MovementEdge.StructSize)..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
