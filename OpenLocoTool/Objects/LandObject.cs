
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
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
	public record LandObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t CostIndex,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04)] uint8_t var_04,
		[property: LocoStructOffset(0x05)] LandObjectFlags Flags,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint8_t var_07,
		[property: LocoStructOffset(0x08)] int8_t CostFactor,
		[property: LocoStructOffset(0x09)] uint8_t pad_09,
		[property: LocoStructOffset(0x0A)] uint32_t Image,
		[property: LocoStructOffset(0x0E)] uint32_t var_0E,
		[property: LocoStructOffset(0x12)] uint32_t var_12,
		[property: LocoStructOffset(0x16)] uint32_t var_16,
		[property: LocoStructOffset(0x1A)] uint8_t pad_1A,
		[property: LocoStructOffset(0x1B)] uint8_t NumVariations,
		[property: LocoStructOffset(0x1C)] uint8_t VariationLikelihood,
		[property: LocoStructOffset(0x1D)] uint8_t pad_1D
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Land;
		public static int StructSize => 0x1E;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// cliff edge header
			remainingData = remainingData[(S5Header.StructLength * 1)..];

			if (Flags.HasFlag(LandObjectFlags.unk1))
			{
				remainingData = remainingData[(S5Header.StructLength * 1)..];
			}

			return remainingData;
		}
	}
}
