
using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum LandObjectFlags : uint8_t
	{
		None = 0,
		unk0 = 1 << 0,
		unk1 = 1 << 1,
		IsDesert = 1 << 2,
		NoTrees = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	[LocoStructType(ObjectType.Land)]
	[LocoStringTable("Name")]
	public class LandObject(
		uint8_t costIndex,
		uint8_t var_03,
		uint8_t var_04,
		LandObjectFlags flags,
		int8_t costFactor,
		uint8_t numVariations,
		uint8_t variationLikelihood) : ILocoStruct, ILocoStructVariableData
	{
		[LocoStructOffset(0x02)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x03)] public uint8_t var_03 { get; set; } = var_03;
		[LocoStructOffset(0x04), LocoPropertyMaybeUnused] public uint8_t var_04 { get; set; } = var_04;
		[LocoStructOffset(0x05)] public LandObjectFlags Flags { get; set; } = flags;
		//[LocoStructOffset(0x06)] public object_index CliffEdgeHeader1 { get; set; }
		//[LocoStructOffset(0x07)] public object_index CliffEdgeHeader2 { get; set; } // unused
		[LocoStructOffset(0x08)] public int8_t CostFactor { get; set; } = costFactor;
		//[LocoStructOffset(0x09)] public uint8_t pad_09 { get; set; }
		//[LocoStructOffset(0x0A)] public image_id Image { get; set; }
		//[LocoStructOffset(0x0E)] public image_id var_0E { get; set; }
		//[LocoStructOffset(0x12)] public image_id CliffEdgeImage { get; set; }
		//[LocoStructOffset(0x16)] public image_id MapPixelImage { get; set; }
		//[LocoStructOffset(0x1A)] public uint8_t pad_1A { get; set; }
		[LocoStructOffset(0x1B)] public uint8_t NumVariations { get; set; } = numVariations;
		[LocoStructOffset(0x1C)] public uint8_t VariationLikelihood { get; set; } = variationLikelihood;
		//[LocoStructOffset(0x1D)] public uint8_t pad_1D { get; set; }

		public S5Header? CliffEdgeHeader { get; set; }
		public S5Header? UnkObjHeader { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// cliff edge header
			CliffEdgeHeader = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];

			// unused obj
			if (Flags.HasFlag(LandObjectFlags.unk1))
			{
				UnkObjHeader = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			var variableDataSize = S5Header.StructLength + (Flags.HasFlag(LandObjectFlags.unk1) ? S5Header.StructLength : 0);

			var data = new byte[variableDataSize];
			data = [.. CliffEdgeHeader.Write()];

			if (Flags.HasFlag(LandObjectFlags.unk1))
			{
				UnkObjHeader.Write().CopyTo(data.AsSpan()[S5Header.StructLength..]);
			}

			return data;
		}
	}
}
