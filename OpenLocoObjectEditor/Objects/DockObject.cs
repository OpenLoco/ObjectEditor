using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Types;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum DockObjectFlags : uint16_t
	{
		None = 0,
		unk01 = 1 << 0,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	[LocoStructType(ObjectType.Dock)]
	[LocoStringTable("Name")]
	public class DockObject(
		int16_t buildCostFactor,
		int16_t sellCostFactor,
		uint8_t costIndex,
		uint8_t var_07,
		DockObjectFlags flags,
		uint8_t numAux01,
		uint8_t numAux02Ent,
		uint16_t designedYear,
		uint16_t obsoleteYear,
		Pos2 boatPosition)
		: ILocoStruct, ILocoStructVariableData
	{

		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x04)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x06)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x07), LocoPropertyMaybeUnused] public uint8_t var_07 { get; set; } = var_07;
		//[LocoStructOffset(0x08)] image_id Image { get; set; }
		//[LocoStructOffset(0x0C)] public image_id UnkImage { get; set; };
		[LocoStructOffset(0x10)] public DockObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x12), LocoPropertyMaybeUnused] public uint8_t NumAux01 { get; set; } = numAux01;
		[LocoStructOffset(0x13), LocoPropertyMaybeUnused] public uint8_t NumAux02Ent { get; set; } = numAux02Ent;
		//LocoStructOffset(0x14)] const uint8_t* var_14 { get; set; }
		//LocoStructOffset(0x18)] const uint16_t* var_18 { get; set; }
		//LocoStructOffset(0x1C)] const uint8_t* var_1C[1] { get; set; } // odd that this is size 1 but that is how its used
		[LocoStructOffset(0x20)] public uint16_t DesignedYear { get; set; } = designedYear;
		[LocoStructOffset(0x22)] public uint16_t ObsoleteYear { get; set; } = obsoleteYear;
		[LocoStructOffset(0x24)] public Pos2 BoatPosition { get; set; } = boatPosition;

		[LocoPropertyMaybeUnused] public List<uint8_t> UnknownAuxData1A { get; set; } = [];
		[LocoPropertyMaybeUnused] public List<uint16_t> UnknownAuxData1B { get; set; } = [];
		[LocoPropertyMaybeUnused] public List<uint8_t> UnknownAuxData2 { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			UnknownAuxData1A.Clear();
			UnknownAuxData1B.Clear();
			UnknownAuxData2.Clear();

			// var_14 - a list of uint8_t
			UnknownAuxData1A.AddRange(remainingData[..(NumAux01 * 1)]);
			remainingData = remainingData[(NumAux01 * 1)..]; // sizeof(uint8_t)

			// var_18 - a list of uint16_t
			var bytearr = remainingData[..(NumAux01 * 2)].ToArray();
			for (var i = 0; i < NumAux01; ++i)
			{
				UnknownAuxData1B.Add(BitConverter.ToUInt16(bytearr, i * 2)); // sizeof(uint16_t)
			}

			remainingData = remainingData[(NumAux01 * 2)..]; // sizeof(uint16_t)

			// parts
			for (var i = 0; i < NumAux02Ent; ++i)
			{
				var ptr_1C = 0;
				while (remainingData[ptr_1C] != 0xFF)
				{
					UnknownAuxData2.Add(remainingData[ptr_1C]);
					ptr_1C++;
				}

				ptr_1C++;
				remainingData = remainingData[ptr_1C..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
			=> UnknownAuxData1A
			.Concat(UnknownAuxData1B.SelectMany(BitConverter.GetBytes))
			.Concat(UnknownAuxData2)
			.Concat(new byte[] { 0xFF })
			.ToArray();
	}
}
