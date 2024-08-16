using System.ComponentModel;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Objects
{
	//[TypeConverter(typeof(ExpandableObjectConverter))]
	//[LocoStructType(ObjectType.Airport)]
	//public class AirportNew(Dictionary<LanguageId, List<string>> name, AirportObject airport, List<G1Element32> graphics)
	//{
	//	public Dictionary<LanguageId, List<string>> Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

	//	public int16_t BuildCostFactor { get; set; } = airport.BuildCostFactor;
	//	public int16_t SellCostFactor { get; set; } = airport.SellCostFactor;
	//	public uint8_t CostIndex { get; set; } = airport.CostIndex;
	//	public uint16_t AllowedPlaneTypes { get; set; } = airport.AllowedPlaneTypes;
	//	public uint8_t NumBuildingAnimations { get; set; } = airport.NumBuildingAnimations;
	//	public int16_t NumBuildingVariations { get; set; } = airport.NumBuildingVariations;
	//	public IList<uint8_t> BuildingVariationHeights { get; } = airport.BuildingVariationHeights ?? throw new ArgumentNullException(nameof(airport.BuildingVariationHeights));
	//	public IList<BuildingPartAnimation> BuildingVariationAnimations { get; } = airport.BuildingVariationAnimations ?? throw new ArgumentNullException(nameof(airport.BuildingVariationAnimations));
	//	public IList<uint8_t[]> BuildingVariationParts { get; } = airport.BuildingVariationParts ?? throw new ArgumentNullException(nameof(airport.BuildingVariationParts));
	//	public IList<AirportBuilding> BuildingPositions { get; } = airport.BuildingPositions ?? throw new ArgumentNullException(nameof(airport.BuildingPositions));
	//	public uint32_t LargeTiles { get; set; } = airport.LargeTiles;
	//	public int8_t MinX { get; set; } = airport.MinX;
	//	public int8_t MinY { get; set; } = airport.MinY;
	//	public int8_t MaxX { get; set; } = airport.MaxX;
	//	public int8_t MaxY { get; set; } = airport.MaxY;
	//	public uint16_t DesignedYear { get; set; } = airport.DesignedYear;
	//	public uint16_t ObsoleteYear { get; set; } = airport.ObsoleteYear;
	//	public uint8_t NumMovementNodes { get; set; } = airport.NumMovementNodes;
	//	public uint8_t NumMovementEdges { get; set; } = airport.NumMovementEdges;
	//	public IList<MovementNode> MovementNodes { get; } = airport.MovementNodes ?? throw new ArgumentNullException(nameof(airport.MovementNodes));
	//	public IList<MovementEdge> MovementEdges { get; } = airport.MovementEdges ?? throw new ArgumentNullException(nameof(airport.MovementEdges));

	//	public IList<G1Element32> Graphics { get; } = graphics ?? throw new ArgumentNullException(nameof(graphics));

	//	//public const int FixedSize = 0xBA;

	//	//public static AirportNew FromDatFile(ReadOnlySpan<byte> bytes) // bytes is the dat file, minus the s5 and obj headers
	//	//{
	//	//	var x = new AirportNew();
	//	//	x.BuildCostFactor = ByteReaderT.Read_int16t(bytes, 0x02);
	//	//	x.SellCostFactor = ByteReaderT.Read_int16t(bytes, 0x04);
	//	//	x.CostIndex = ByteReaderT.Read_uint8t(bytes, 0x06);
	//	//	x.AllowedPlaneTypes = ByteReaderT.Read_uint16t(bytes, 0x10);
	//	//	x.NumBuildingAnimations = ByteReaderT.Read_uint8t(bytes, 0x12);
	//	//	x.NumBuildingVariations = ByteReaderT.Read_uint8t(bytes, 0x13);

	//	//	//x.BuildingVariationHeights = ByteReaderT.Read_Array<uint8_t>(bytes, 0x14);
	//	//	//x.BuildingVariationAnimations = ByteReaderT.Read_int16t(bytes, 0x18);
	//	//	//x.BuildingVariationParts = ByteReaderT.Read_uint8t(bytes, 0x1C);
	//	//	//x.BuildingPositions = ByteReaderT.Read_int16t(bytes, 0x9C);

	//	//	x.LargeTiles = ByteReaderT.Read_uint32t(bytes, 0xA0);
	//	//	x.MinX = ByteReaderT.Read_int8t(bytes, 0xA4);
	//	//	x.MinY = ByteReaderT.Read_int8t(bytes, 0xA5);
	//	//	x.MaxX = ByteReaderT.Read_int8t(bytes, 0xA6);
	//	//	x.MaxY = ByteReaderT.Read_int8t(bytes, 0xA7);
	//	//	x.DesignedYear = ByteReaderT.Read_uint16t(bytes, 0xA8);
	//	//	x.ObsoleteYear = ByteReaderT.Read_uint16t(bytes, 0xAA);
	//	//	x.NumMovementNodes = ByteReaderT.Read_uint8t(bytes, 0xAC);
	//	//	x.NumMovementEdges = ByteReaderT.Read_uint8t(bytes, 0xAD);

	//	//	///x.MovementNodes = ByteReaderT.Read_int16t(bytes, 0x02);
	//	//	//x.MovementEdgesMaxY = ByteReaderT.Read_int16t(bytes, 0x04);

	//	//	// move past fixed section
	//	//	bytes = bytes[FixedSize..];

	//	//	// read variable data

	//	//	// read string table
	//	//	var bytesString = 1;
	//	//	bytes = bytes[bytesString..];

	//	//	// read graphics

	//	//	return x;
	//	//}
	//}

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
		[property: LocoStructOffset(0x12)] uint8_t NumBuildingAnimations,
		[property: LocoStructOffset(0x13)] uint8_t NumBuildingVariations,
		[property: LocoStructOffset(0x14), LocoStructVariableLoad] List<uint8_t> BuildingVariationHeights,
		[property: LocoStructOffset(0x18), LocoStructVariableLoad] List<BuildingPartAnimation> BuildingVariationAnimations,
		[property: LocoStructOffset(0x1C), LocoStructVariableLoad, LocoArrayLength(AirportObject.VariationPartCount)] List<uint8_t[]> BuildingVariationParts,
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
		[property: LocoStructOffset(0xB6), LocoArrayLength(0xBA - 0xB6)] uint8_t[] pad_B6
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int VariationPartCount = 32;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// variation heights
			BuildingVariationHeights.Clear();
			BuildingVariationHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumBuildingAnimations * 1)], NumBuildingAnimations));
			remainingData = remainingData[(NumBuildingAnimations * 1)..]; // uint8_t*

			// variation animations
			BuildingVariationAnimations.Clear();
			var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
			BuildingVariationAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumBuildingAnimations * buildingAnimationSize)], typeof(BuildingPartAnimation), NumBuildingAnimations, buildingAnimationSize)
				.Cast<BuildingPartAnimation>());
			remainingData = remainingData[(NumBuildingAnimations * 2)..]; // uint16_t*

			// variation parts
			for (var i = 0; i < NumBuildingVariations; ++i)
			{
				var ptr_1C = 0;
				while (remainingData[++ptr_1C] != 0xFF)
				{ }

				BuildingVariationParts.Add(remainingData[..ptr_1C].ToArray());
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

			// variation heights
			foreach (var x in BuildingVariationHeights)
			{
				ms.WriteByte(x);
			}

			// variation animations
			foreach (var x in BuildingVariationAnimations)
			{
				ms.WriteByte(x.NumFrames);
				ms.WriteByte(x.AnimationSpeed);
			}

			// variation parts
			foreach (var x in BuildingVariationParts)
			{
				ms.Write(x);
				ms.WriteByte(0xFF);
			}

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
