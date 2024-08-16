using System.ComponentModel;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum BuildingObjectFlags : uint8_t
	{
		None = 0,
		LargeTile = 1 << 0, // 2x2 tile
		MiscBuilding = 1 << 1,
		Indestructible = 1 << 2,
		IsHeadquarters = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xBE)]
	[LocoStructType(ObjectType.Building)]
	[LocoStringTable("Name")]
	public record BuildingObject(
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
			[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
			[property: LocoStructOffset(0x06)] uint8_t NumParts,
			[property: LocoStructOffset(0x07)] uint8_t NumVariations,
			[property: LocoStructOffset(0x08), LocoStructVariableLoad, LocoArrayLength(4)] List<uint8_t> PartHeights,
			[property: LocoStructOffset(0x0C), LocoStructVariableLoad, LocoArrayLength(2)] List<BuildingPartAnimation> PartAnimations,
			[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(BuildingObject.VariationPartCount)] List<uint8_t[]> VariationParts,
			[property: LocoStructOffset(0x90)] uint32_t Colours,
			[property: LocoStructOffset(0x94)] uint16_t DesignedYear,
			[property: LocoStructOffset(0x96)] uint16_t ObsoleteYear,
			[property: LocoStructOffset(0x98)] BuildingObjectFlags Flags,
			[property: LocoStructOffset(0x99)] uint8_t ClearCostIndex,
			[property: LocoStructOffset(0x9A)] uint16_t ClearCostFactor,
			[property: LocoStructOffset(0x9C)] uint8_t ScaffoldingSegmentType,
			[property: LocoStructOffset(0x9D)] Colour ScaffoldingColour,
			[property: LocoStructOffset(0x9E)] uint8_t GeneratorFunction,
			[property: LocoStructOffset(0x9F)] uint8_t AverageNumberOnMap,
			[property: LocoStructOffset(0xA0), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> _ProducedQuantity,
			[property: LocoStructOffset(0xA2), LocoStructVariableLoad, LocoArrayLength(BuildingObject.MaxProducedCargoType), Browsable(false)] List<object_id> _ProducedCargoType,
			[property: LocoStructOffset(0xA4), LocoStructVariableLoad, LocoArrayLength(BuildingObject.MaxRequiredCargoType), Browsable(false)] List<object_id> _RequiredCargoType,
			[property: LocoStructOffset(0xA6), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A6,
			[property: LocoStructOffset(0xA8), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A8,
			[property: LocoStructOffset(0xAA)] int16_t DemolishRatingReduction,
			[property: LocoStructOffset(0xAC)] uint8_t var_AC,
			[property: LocoStructOffset(0xAD)] uint8_t NumAnimatedElevators
		//[property: LocoStructOffset(0xAE), LocoStructVariableLoad, LocoArrayLength(4)] uint8_t[][] var_AE // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
		) : ILocoStruct, ILocoStructVariableData
	{
		public const int VariationPartCount = 32;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 2;

		public List<S5Header> ProducedCargo { get; set; } = [];
		public List<S5Header> RequiredCargo { get; set; } = [];

		public List<uint8_t[]> ElevatorAnimationHeights { get; set; } = [];

		// known issues:
		// HOSPITL1.dat - loads without error but graphics are bugged
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// variation heights
			PartHeights.Clear();
			PartHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumParts * 1)], NumParts));
			remainingData = remainingData[(NumParts * 1)..]; // uint8_t*

			// variation animations
			PartAnimations.Clear();
			var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
			PartAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumParts * buildingAnimationSize)], typeof(BuildingPartAnimation), NumParts, buildingAnimationSize)
				.Cast<BuildingPartAnimation>());
			remainingData = remainingData[(NumParts * 2)..]; // uint16_t*

			// variation parts
			VariationParts.Clear();
			for (var i = 0; i < NumVariations; ++i)
			{
				var ptr_10 = 0;
				while (remainingData[++ptr_10] != 0xFF)
				{
					;
				}

				VariationParts.Add(remainingData[..ptr_10].ToArray());
				ptr_10++;
				remainingData = remainingData[ptr_10..];
			}

			// produced cargo
			ProducedCargo.Clear();
			ProducedCargo = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxProducedCargoType);
			remainingData = remainingData[(S5Header.StructLength * MaxProducedCargoType)..];

			// required cargo
			RequiredCargo.Clear();
			RequiredCargo = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxRequiredCargoType);
			remainingData = remainingData[(S5Header.StructLength * MaxRequiredCargoType)..];

			// animation sequences
			ElevatorAnimationHeights.Clear();
			for (var i = 0; i < NumAnimatedElevators; ++i)
			{
				var size = BitConverter.ToUInt16(remainingData[..2]);
				remainingData = remainingData[2..];

				ElevatorAnimationHeights.Add(remainingData[..size].ToArray());
				remainingData = remainingData[size..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			var ms = new MemoryStream();

			// variation heights
			foreach (var x in PartHeights)
			{
				ms.WriteByte(x);
			}

			// variation animations
			foreach (var x in PartAnimations)
			{
				ms.WriteByte(x.NumFrames);
				ms.WriteByte(x.AnimationSpeed);
			}

			// variation parts
			foreach (var x in VariationParts)
			{
				ms.Write(x);
				ms.WriteByte(0xFF);
			}

			// produced cargo
			foreach (var obj in ProducedCargo.Fill(MaxProducedCargoType, S5Header.NullHeader))
			{
				ms.Write(obj.Write());
			}

			// required cargo
			foreach (var obj in RequiredCargo.Fill(MaxRequiredCargoType, S5Header.NullHeader))
			{
				ms.Write(obj.Write());
			}

			foreach (var unk in ElevatorAnimationHeights)
			{
				ms.Write(BitConverter.GetBytes((uint16_t)unk.Length));
				foreach (var x in unk)
				{
					ms.Write(BitConverter.GetBytes((uint16_t)x));
				}
			}

			return ms.ToArray();
		}

		public bool Validate()
			=> NumParts is not 0 and not > 63
			&& NumVariations is not 0 and <= 31;
	}
}
