
using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;

namespace OpenLoco.ObjectEditor.Objects
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
	public record LandObject(
		[property: LocoStructOffset(0x02)] uint8_t CostIndex,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04), LocoPropertyMaybeUnused] uint8_t var_04,
		[property: LocoStructOffset(0x05)] LandObjectFlags Flags,
		[property: LocoStructOffset(0x06), Browsable(false)] object_id CliffEdgeHeader1,
		[property: LocoStructOffset(0x07), Browsable(false), LocoPropertyMaybeUnused] object_id CliffEdgeHeader2,
		[property: LocoStructOffset(0x08)] int8_t CostFactor,
		[property: LocoStructOffset(0x09), Browsable(false)] uint8_t pad_09,
		[property: LocoStructOffset(0x0A), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id var_0E,
		[property: LocoStructOffset(0x12), Browsable(false)] image_id CliffEdgeImage,
		[property: LocoStructOffset(0x16), Browsable(false)] image_id MapPixelImage,
		[property: LocoStructOffset(0x1A), Browsable(false)] uint8_t pad_1A,
		[property: LocoStructOffset(0x1B)] uint8_t NumVariations,
		[property: LocoStructOffset(0x1C)] uint8_t VariationLikelihood,
		[property: LocoStructOffset(0x1D), Browsable(false)] uint8_t pad_1D
		) : ILocoStruct, ILocoStructVariableData
	{
		public S5Header CliffEdgeHeader { get; set; }
		public S5Header UnkObjHeader { get; set; }

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

		public bool Validate()
		{
			if (CostIndex > 32)
			{
				return false;
			}
			if (CostFactor <= 0)
			{
				return false;
			}
			if (var_03 < 1)
			{
				return false;
			}
			if (var_03 > 8)
			{
				return false;
			}

			return (var_04 == 1 || var_04 == 2 || var_04 == 4);
		}
	}
}
