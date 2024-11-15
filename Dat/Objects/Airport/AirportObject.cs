using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xBA)]
	[LocoStructType(ObjectType.Airport)]
	[LocoStringTable("Name")]
	public record AirportObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x06)] uint8_t CostIndex,
		[property: LocoStructOffset(0x07)] uint8_t var_07,
		[property: LocoStructOffset(0x08), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0C), Browsable(false)] image_id ImageOffset,
		[property: LocoStructOffset(0x10)] uint16_t AllowedPlaneTypes,
		[property: LocoStructOffset(0x12)] uint8_t NumBuildingParts,
		[property: LocoStructOffset(0x13)] uint8_t NumBuildingVariations,
		[property: LocoStructOffset(0x14), LocoStructVariableLoad, LocoArrayLength(AirportObject.BuildingHeightCount)] List<uint8_t> BuildingHeights,
		[property: LocoStructOffset(0x18), LocoStructVariableLoad, LocoArrayLength(AirportObject.BuildingAnimationCount)] List<BuildingPartAnimation> BuildingAnimations,
		[property: LocoStructOffset(0x1C), LocoStructVariableLoad, LocoArrayLength(AirportObject.BuildingVariationCount)] List<List<uint8_t>> BuildingVariations,
		[property: LocoStructOffset(0x9C), LocoStructVariableLoad] List<AirportBuilding> BuildingPositions,
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
		[property: LocoStructOffset(0xB6), LocoArrayLength(0xBA - 0xB6)] uint8_t[] var_B6
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// variation heights
			BuildingHeights.Clear();
			BuildingHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumBuildingParts * 1)], NumBuildingParts));
			remainingData = remainingData[(NumBuildingParts * 1)..]; // uint8_t*

			// variation animations
			BuildingAnimations.Clear();
			var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
			BuildingAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumBuildingParts * buildingAnimationSize)], typeof(BuildingPartAnimation), NumBuildingParts, buildingAnimationSize)
				.Cast<BuildingPartAnimation>());
			remainingData = remainingData[(NumBuildingParts * 2)..]; // uint16_t*

			// variation parts
			for (var i = 0; i < NumBuildingVariations; ++i)
			{
				var ptr_1C = 0;
				while (remainingData[++ptr_1C] != 0xFF)
				{ }

				BuildingVariations.Add(remainingData[..ptr_1C].ToArray().ToList());
				ptr_1C++;
				remainingData = remainingData[ptr_1C..];
			}

			// building positions
			var ptr_9C = 0;
			BuildingPositions.Clear();
			var airportBuildingSize = ObjectAttributes.StructSize<AirportBuilding>();
			while (remainingData[ptr_9C] != 0xFF)
			{
				var position = ByteReader.ReadLocoStruct<AirportBuilding>(remainingData[ptr_9C..(ptr_9C + airportBuildingSize)]);
				BuildingPositions.Add(position);
				ptr_9C += airportBuildingSize;
			}

			ptr_9C++;
			remainingData = remainingData[ptr_9C..];

			// movement nodes
			MovementNodes.Clear();
			var nodeSize = ObjectAttributes.StructSize<MovementNode>();
			var edgeSize = ObjectAttributes.StructSize<MovementEdge>();
			MovementNodes.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumMovementNodes * nodeSize)], typeof(MovementNode), NumMovementNodes, nodeSize)
				.Cast<MovementNode>());
			remainingData = remainingData[(NumMovementNodes * nodeSize)..];

			// movement edges
			MovementEdges.Clear();
			MovementEdges.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumMovementEdges * edgeSize)], typeof(MovementEdge), NumMovementEdges, edgeSize)
				.Cast<MovementEdge>()
				.ToList());

			return remainingData[(NumMovementEdges * edgeSize)..];
		}

		public ReadOnlySpan<byte> Save()
		{
			var ms = new MemoryStream();

			// heights
			foreach (var x in BuildingHeights)
			{
				ms.WriteByte(x);
			}

			// animations
			foreach (var x in BuildingAnimations)
			{
				ms.WriteByte(x.NumFrames);
				ms.WriteByte(x.AnimationSpeed);
			}

			// variations
			foreach (var x in BuildingVariations)
			{
				ms.Write(x.ToArray());
				ms.WriteByte(0xFF);
			}

			// positions
			foreach (var x in BuildingPositions)
			{
				ms.WriteByte(x.Index);
				ms.WriteByte(x.Rotation);
				ms.WriteByte((byte)x.X);
				ms.WriteByte((byte)x.Y);
			}

			ms.WriteByte(0xFF);

			// movement nodes
			foreach (var x in MovementNodes)
			{
				ms.Write(BitConverter.GetBytes(x.X));
				ms.Write(BitConverter.GetBytes(x.Y));
				ms.Write(BitConverter.GetBytes(x.Z));
				ms.Write(BitConverter.GetBytes((uint16_t)x.Flags));
			}

			// movement edges
			foreach (var x in MovementEdges)
			{
				ms.WriteByte(x.var_00);
				ms.WriteByte(x.CurrNode);
				ms.WriteByte(x.NextNode);
				ms.WriteByte(x.var_03);
				ms.Write(BitConverter.GetBytes(x.MustBeClearEdges));
				ms.Write(BitConverter.GetBytes(x.AtLeastOneClearEdges));
			}

			return ms.ToArray();
		}

		public bool Validate()
		{
			if (CostIndex > 32)
			{
				return false;
			}

			if (-SellCostFactor > BuildCostFactor)
			{
				return false;
			}

			return BuildCostFactor > 0;
		}
	}
}
