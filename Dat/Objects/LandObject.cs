using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects
{
	[Flags]
	public enum LandObjectFlags : uint8_t
	{
		None = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		IsDesert = 1 << 2,
		NoTrees = 1 << 3,
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	[LocoStructType(ObjectType.Land)]
	[LocoStringTable("Name")]
	public record LandObject(
		[property: LocoStructOffset(0x02)] uint8_t CostIndex,
		[property: LocoStructOffset(0x03)] uint8_t NumGrowthStages,
		[property: LocoStructOffset(0x04)] uint8_t NumImageAngles,
		[property: LocoStructOffset(0x05)] LandObjectFlags Flags,
		[property: LocoStructOffset(0x06), Browsable(false)] object_id CliffEdgeHeader1,
		[property: LocoStructOffset(0x07), Browsable(false), LocoPropertyMaybeUnused] object_id CliffEdgeHeader2,
		[property: LocoStructOffset(0x08)] int16_t CostFactor,
		[property: LocoStructOffset(0x0A), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0E), Browsable(false)] uint32_t NumImagesPerGrowthStage,
		[property: LocoStructOffset(0x12), Browsable(false)] image_id CliffEdgeImage,
		[property: LocoStructOffset(0x16), Browsable(false)] image_id MapPixelImage,
		[property: LocoStructOffset(0x1A)] uint8_t DistributionPattern,
		[property: LocoStructOffset(0x1B)] uint8_t NumVariations,
		[property: LocoStructOffset(0x1C)] uint8_t VariationLikelihood,
		[property: LocoStructOffset(0x1D), Browsable(false)] uint8_t pad_1D
		) : ILocoStruct, ILocoStructVariableData, IImageTableNameProvider
	{
		public S5Header CliffEdgeHeader { get; set; }
		public S5Header UnkObjHeader { get; set; }

		public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
		{
			// cliff edge header
			CliffEdgeHeader = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];

			// unused obj
			if (Flags.HasFlag(LandObjectFlags.unk_01))
			{
				UnkObjHeader = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> SaveVariable()
		{
			var variableDataSize = S5Header.StructLength + (Flags.HasFlag(LandObjectFlags.unk_01) ? S5Header.StructLength : 0);
			_ = new byte[variableDataSize];
			byte[]? data = [.. CliffEdgeHeader.Write()];

			if (Flags.HasFlag(LandObjectFlags.unk_01))
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

			if (NumGrowthStages < 1)
			{
				return false;
			}

			if (NumGrowthStages > 8)
			{
				return false;
			}

			return NumImageAngles is 1 or 2 or 4;
		}

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "kFlatSE" },
			{ 1, "toolbar_terraform_land" },
		};
	}
}
