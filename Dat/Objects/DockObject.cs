using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum DockObjectFlags : uint16_t
	{
		None = 0,
		HasShadows = 1 << 0,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	[LocoStructType(ObjectType.Dock)]
	[LocoStringTable("Name")]
	public record DockObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x06)] uint8_t CostIndex,
		[property: LocoStructOffset(0x07), LocoPropertyMaybeUnused] uint8_t var_07,
		[property: LocoStructOffset(0x08), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0C), Browsable(false)] image_id UnkImage,
		[property: LocoStructOffset(0x10)] DockObjectFlags Flags,
		[property: LocoStructOffset(0x12), LocoPropertyMaybeUnused] uint8_t NumAux01,
		[property: LocoStructOffset(0x13), LocoPropertyMaybeUnused] uint8_t NumAux02Ent, // must be 1 or 0
		[property: LocoStructOffset(0x14), LocoStructVariableLoad] List<uint8_t> var_14,
		[property: LocoStructOffset(0x18), LocoStructVariableLoad] List<uint16_t> BuildingPartAnimations,
		[property: LocoStructOffset(0x1C), LocoStructVariableLoad] List<uint8_t> BuildingVariationParts,
		[property: LocoStructOffset(0x20)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x22)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x24)] Pos2 BoatPosition
	) : ILocoStruct, ILocoStructVariableData
	{
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			var_14.Clear();
			BuildingPartAnimations.Clear();
			BuildingVariationParts.Clear();

			// var_14 - a list of uint8_t
			var_14.AddRange(remainingData[..(NumAux01 * 1)]);
			remainingData = remainingData[(NumAux01 * 1)..]; // sizeof(uint8_t)

			// var_18 - a list of uint16_t
			var bytearr = remainingData[..(NumAux01 * 2)].ToArray();
			for (var i = 0; i < NumAux01; ++i)
			{
				BuildingPartAnimations.Add(BitConverter.ToUInt16(bytearr, i * 2)); // sizeof(uint16_t)
			}

			remainingData = remainingData[(NumAux01 * 2)..]; // sizeof(uint16_t)

			// parts
			for (var i = 0; i < NumAux02Ent; ++i)
			{
				var ptr_1C = 0;
				while (remainingData[ptr_1C] != 0xFF)
				{
					BuildingVariationParts.Add(remainingData[ptr_1C]);
					ptr_1C++;
				}

				ptr_1C++;
				remainingData = remainingData[ptr_1C..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
			=> var_14
			.Concat(BuildingPartAnimations.SelectMany(BitConverter.GetBytes))
			.Concat(BuildingVariationParts)
			.Concat(new byte[] { 0xFF })
			.ToArray();
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