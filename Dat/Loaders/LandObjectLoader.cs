using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using static Dat.Loaders.LandObjectLoader;

namespace Dat.Loaders;

public abstract class LandObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x1E;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new LandObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.CostIndex = br.ReadByte();
			model.NumGrowthStages = br.ReadByte();
			model.NumImageAngles = br.ReadByte();
			model.Flags = ((DatLandObjectFlags)br.ReadByte()).Convert();
			br.SkipObjectId(); // CliffEdgeHeader1, not part of object definition
			br.SkipObjectId(); // CliffEdgeHeader2, not part of object
			model.CostFactor = br.ReadInt16();
			br.SkipImageId(); // Image offset, not part of object definition
			model.NumImagesPerGrowthStage = br.ReadUInt32();
			br.SkipImageId(); // CliffEdgeImage, not part of object definition
			br.SkipImageId(); // MapPixelImage, not part of object definition
			model.DistributionPattern = br.ReadByte();
			model.NumVariations = br.ReadByte();
			model.VariationLikelihood = br.ReadByte();
			br.SkipByte(); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Land), null);

			// variable
			model.CliffEdgeHeader = br.ReadS5Header();
			if (model.Flags.HasFlag(LandObjectFlags.HasUnkObjectHeader))
			{
				model.UnkObjectHeader = br.ReadS5Header();
			}

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Land, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (LandObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.CostIndex);
			bw.Write(model.NumGrowthStages);
			bw.Write(model.NumImageAngles);
			bw.Write((uint8_t)model.Flags.Convert());
			bw.WriteEmptyObjectId(); // CliffEdgeHeader1, not part of object definition
			bw.WriteEmptyObjectId(); // CliffEdgeHeader2, not part of object definition
			bw.Write(model.CostFactor);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write(model.NumImagesPerGrowthStage);
			bw.WriteEmptyImageId(); // CliffEdgeImage, not part of object definition
			bw.WriteEmptyImageId(); // MapPixelImage, not part of object definition
			bw.Write(model.DistributionPattern);
			bw.Write(model.NumVariations);
			bw.Write(model.VariationLikelihood);
			bw.Write((uint8_t)0); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5Header(model.CliffEdgeHeader);
			if (model.Flags.HasFlag(LandObjectFlags.HasUnkObjectHeader))
			{
				ArgumentNullException.ThrowIfNull(model.UnkObjectHeader); // cannot have flag set but no unk header
				bw.WriteS5Header(model.UnkObjectHeader);
			}

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatLandObjectFlags : uint8_t
	{
		None = 0,
		unk_00 = 1 << 0,
		HasUnkObjectHeader = 1 << 1,
		IsDesert = 1 << 2,
		NoTrees = 1 << 3,
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
	}
}

internal static class LandObjectFlagsConverter
{
	public static LandObjectFlags Convert(this DatLandObjectFlags datLandObjectFlags)
		=> (LandObjectFlags)datLandObjectFlags;

	public static DatLandObjectFlags Convert(this LandObjectFlags landObjectFlags)
		=> (DatLandObjectFlags)landObjectFlags;
}

[LocoStructSize(0x1E)]
[LocoStructType(DatObjectType.Land)]
internal record DatLandObject(
	[property: LocoStructOffset(0x02)] uint8_t CostIndex,
	[property: LocoStructOffset(0x03)] uint8_t NumGrowthStages,
	[property: LocoStructOffset(0x04)] uint8_t NumImageAngles,
	[property: LocoStructOffset(0x05)] DatLandObjectFlags Flags,
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
		if (Flags.HasFlag(DatLandObjectFlags.HasUnkObjectHeader))
		{
			UnkObjHeader = S5Header.Read(remainingData[..S5Header.StructLength]);
			remainingData = remainingData[S5Header.StructLength..];
		}

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		var variableDataSize = S5Header.StructLength + (Flags.HasFlag(DatLandObjectFlags.HasUnkObjectHeader) ? S5Header.StructLength : 0);
		_ = new byte[variableDataSize];
		byte[]? data = [.. CliffEdgeHeader.Write()];

		if (Flags.HasFlag(DatLandObjectFlags.HasUnkObjectHeader))
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
