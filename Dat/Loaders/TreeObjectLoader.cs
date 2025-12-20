using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Tree;
using Definitions.ObjectModels.Types;
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

	public static ObjectType ObjectType => ObjectType.Tree;
	public static DatObjectType DatObjectType => DatObjectType.Tree;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TreeObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.InitialHeight = br.ReadByte();
			model.Height = br.ReadByte();
			model.var_04 = br.ReadByte();
			model.var_05 = br.ReadByte();
			model.NumRotations = br.ReadByte();
			model.NumGrowthStages = br.ReadByte();
			model.Flags = ((DatTreeObjectFlags)br.ReadUInt16()).Convert();
			br.SkipImageId(Constants.ImageCount); // Image sprites, not part of object definition
			br.SkipImageId(Constants.ImageCount); // Snow sprites, not part of object definition
			model.ShadowImageOffset = br.ReadUInt16();
			model.SeasonalVariants = ((DatTreeFlagsUnk)br.ReadByte()).Convert();
			model.SeasonState = br.ReadByte();
			model.CurrentSeason = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.ClearCostFactor = br.ReadInt16();
			model.Colours = br.ReadUInt32();
			model.Rating = br.ReadInt16();
			model.DemolishRatingReduction = br.ReadInt16();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TreeObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.InitialHeight);
			bw.Write(model.Height);
			bw.Write(model.var_04);
			bw.Write(model.var_05);
			bw.Write(model.NumRotations);
			bw.Write(model.NumGrowthStages);
			bw.Write((uint16_t)model.Flags.Convert()); // Convert to DatTreeObjectFlags
			bw.WriteEmptyImageId(Constants.ImageCount); // Image sprites, not part
			bw.WriteEmptyImageId(Constants.ImageCount); // Snow sprites, not part of object definition
			bw.Write(model.ShadowImageOffset);
			bw.Write((uint8_t)model.SeasonalVariants.Convert()); // Convert to Dat
			bw.Write(model.SeasonState);
			bw.Write(model.CurrentSeason);
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.ClearCostFactor);
			bw.Write(model.Colours);
			bw.Write(model.Rating);
			bw.Write(model.DemolishRatingReduction);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

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
	public static TreeObjectSeasonalVariantFlags Convert(this DatTreeFlagsUnk datTreeFlagsUnk)
		=> (TreeObjectSeasonalVariantFlags)datTreeFlagsUnk;

	public static DatTreeFlagsUnk Convert(this TreeObjectSeasonalVariantFlags treeFlagsUnk)
		=> (DatTreeFlagsUnk)treeFlagsUnk;
}
