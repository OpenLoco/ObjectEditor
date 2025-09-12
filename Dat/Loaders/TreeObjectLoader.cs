using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Tree;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using static Dat.Loaders.TreeObjectLoader;

namespace Dat.Loaders;
public abstract class TreeObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int ImageCount = 6;
	}

	public static class StructSizes
	{
		public const int Dat = 0x4C;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TreeObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.Clearance = br.ReadByte();
			model.Height = br.ReadByte();
			model.var_04 = br.ReadByte();
			model.var_05 = br.ReadByte();
			model.NumRotations = br.ReadByte();
			model.NumGrowthStages = br.ReadByte();
			model.Flags = ((DatTreeObjectFlags)br.ReadUInt16()).Convert();
			br.SkipImageId(Constants.ImageCount); // Image sprites, not part of object definition
			br.SkipImageId(Constants.ImageCount); // Snow sprites, not part of object definition
			model.ShadowImageOffset = br.ReadUInt16();
			model.var_3C = ((DatTreeFlagsUnk)br.ReadByte()).Convert();
			model.SeasonState = br.ReadByte();
			model.Season = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.ClearCostFactor = br.ReadInt16();
			model.Colours = br.ReadUInt32();
			model.Rating = br.ReadInt16();
			model.DemolishRatingReduction = br.ReadInt16();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Tree), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Tree, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TreeObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.Clearance);
			bw.Write(model.Height);
			bw.Write(model.var_04);
			bw.Write(model.var_05);
			bw.Write(model.NumRotations);
			bw.Write(model.NumGrowthStages);
			bw.Write((uint16_t)model.Flags.Convert()); // Convert to DatTreeObjectFlags
			bw.WriteEmptyImageId(Constants.ImageCount); // Image sprites, not part
			bw.WriteEmptyImageId(Constants.ImageCount); // Snow sprites, not part of object definition
			bw.Write(model.ShadowImageOffset);
			bw.Write((uint8_t)model.var_3C.Convert()); // Convert to Dat
			bw.Write(model.SeasonState);
			bw.Write(model.Season);
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.ClearCostFactor);
			bw.Write(model.Colours);
			bw.Write(model.Rating);
			bw.Write(model.DemolishRatingReduction);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatTreeObjectFlags : uint16_t
	{
		None = 0,
		HasSnowVariation = 1 << 0,
		unk_01 = 1 << 1,
		HighAltitude = 1 << 2,
		LowAltitude = 1 << 3,
		RequiresWater = 1 << 4,
		unk_05 = 1 << 5,
		DroughtResistant = 1 << 6,
		HasShadow = 1 << 7,
	}

	[Flags]
	internal enum DatTreeFlagsUnk : uint8_t
	{
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
		unk_03 = 1 << 3,
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
	}
}

internal static class TreeObjectFlagsConverter
{
	public static TreeObjectFlags Convert(this DatTreeObjectFlags datTreeObjectFlags)
		=> (TreeObjectFlags)datTreeObjectFlags;

	public static DatTreeObjectFlags Convert(this TreeObjectFlags treeObjectFlags)
		=> (DatTreeObjectFlags)treeObjectFlags;
}

internal static class TreeFlagsUnkConverter
{
	public static TreeFlagsUnk Convert(this DatTreeFlagsUnk datTreeFlagsUnk)
		=> (TreeFlagsUnk)datTreeFlagsUnk;

	public static DatTreeFlagsUnk Convert(this TreeFlagsUnk treeFlagsUnk)
		=> (DatTreeFlagsUnk)treeFlagsUnk;
}

[LocoStructSize(0x4C)]
[LocoStructType(DatObjectType.Tree)]
internal record DatTreeObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t Clearance,
	[property: LocoStructOffset(0x03)] uint8_t Height,
	[property: LocoStructOffset(0x04)] uint8_t var_04,
	[property: LocoStructOffset(0x05)] uint8_t var_05,
	[property: LocoStructOffset(0x06)] uint8_t NumRotations,
	[property: LocoStructOffset(0x07)] uint8_t NumGrowthStages,
	[property: LocoStructOffset(0x08)] DatTreeObjectFlags Flags,
	[property: LocoStructOffset(0x0A), LocoArrayLength(6), Browsable(false)] image_id[] Sprites,
	[property: LocoStructOffset(0x22), LocoArrayLength(6), Browsable(false)] image_id[] SnowSprites,
	[property: LocoStructOffset(0x3A), Browsable(false)] uint16_t ShadowImageOffset,
	[property: LocoStructOffset(0x3C)] DatTreeFlagsUnk var_3C, // something with images
	[property: LocoStructOffset(0x3D)] uint8_t SeasonState,
	[property: LocoStructOffset(0x3E)] uint8_t Season,
	[property: LocoStructOffset(0x3F)] uint8_t CostIndex,
	[property: LocoStructOffset(0x40)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x42)] int16_t ClearCostFactor,
	[property: LocoStructOffset(0x44)] uint32_t Colours,
	[property: LocoStructOffset(0x48)] int16_t Rating,
	[property: LocoStructOffset(0x4A)] int16_t DemolishRatingReduction);
